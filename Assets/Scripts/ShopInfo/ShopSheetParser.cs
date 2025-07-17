using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShopSheetParser
{
    public static List<LocalizationText> LocalizationTextParse(ValueRange sheet)
    {
        var result = new List<LocalizationText>();
        var rows = sheet?.Values;

        if (rows == null || rows.Count < 2)
        {
            Debug.LogWarning("로컬라이제이션 시트에 데이터가 충분하지 않습니다.");
            return result;
        }

        // 헤더 스킵 (row[0] = "Num", "Key", "Korean", "English", "Japanese", "Chinese")
        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];

            // Num과 Key가 필수
            if (row.Count < 3 || string.IsNullOrWhiteSpace(row[0]?.ToString()) || string.IsNullOrWhiteSpace(row[1]?.ToString()))
                continue;

            var data = new LocalizationText();

            // Num
            if (int.TryParse(row[0].ToString(), out int num))
                data.Num = num;
            else
                continue;

            // Key
            data.Key = row[1].ToString();

            // 언어별 텍스트 (index 기준: Korean = 2, English = 3, Japanese = 4, Chinese = 5)
            data.Text[(int)Language.Korean] = GetCell(row, 2);
            data.Text[(int)Language.English] = GetCell(row, 3);
            data.Text[(int)Language.Japanese] = GetCell(row, 4);
            data.Text[(int)Language.Chinese] = GetCell(row, 5);

            result.Add(data);
        }

        Debug.Log($"✅ Localization 텍스트 {result.Count}개 파싱 완료");
        return result;
    }
    public static List<BaseShopInfoData> ShopDataParse(ValueRange sheet)
    {
        var result = new List<BaseShopInfoData>();
        var rows = sheet?.Values;

        if (rows == null || rows.Count < 3)
        {
            Debug.LogWarning("시트에 데이터가 충분하지 않습니다.");
            return result;
        }

        for (int i = 2; i < rows.Count; i++)
        {
            var row = rows[i];

            // ShopID 가 비어있으면 생략
            if (row.Count <= (int)ShopDataRowKey.ShopID || string.IsNullOrWhiteSpace(row[(int)ShopDataRowKey.ShopID]?.ToString()))
                continue;
            if (!int.TryParse(row[(int)ShopDataRowKey.ShopID].ToString(), out int shopId))
                continue;

            // 언어별 ShopName
            string[] shopName = new string[(int)Language.EndOfIndex];
            shopName[(int)Language.Korean] = GetCell(row, (int)ShopDataRowKey.ShopName_Kr);
            shopName[(int)Language.English] = GetCell(row, (int)ShopDataRowKey.ShopName_En);
            shopName[(int)Language.Japanese] = GetCell(row, (int)ShopDataRowKey.ShopName_Jp);
            shopName[(int)Language.Chinese] = GetCell(row, (int)ShopDataRowKey.ShopName_Ch);

            // 언어별 Address
            string[] address = new string[(int)Language.EndOfIndex];
            address[(int)Language.Korean] = GetCell(row, (int)ShopDataRowKey.Address_Kr);
            address[(int)Language.English] = GetCell(row, (int)ShopDataRowKey.Address_En);
            address[(int)Language.Japanese] = GetCell(row, (int)ShopDataRowKey.Address_Jp);
            address[(int)Language.Chinese] = GetCell(row, (int)ShopDataRowKey.Address_Ch);

            // 언어별 HashTag
            string[] hashTag = new string[(int)Language.EndOfIndex];
            hashTag[(int)Language.Korean] = GetCell(row, (int)ShopDataRowKey.HashTag_Kr);
            hashTag[(int)Language.English] = GetCell(row, (int)ShopDataRowKey.HashTag_En);
            hashTag[(int)Language.Japanese] = GetCell(row, (int)ShopDataRowKey.HashTag_Jp);
            hashTag[(int)Language.Chinese] = GetCell(row, (int)ShopDataRowKey.HashTag_Ch);

            // 언어별 ShopDescription
            string[] shopDescription = new string[(int)Language.EndOfIndex];
            shopDescription[(int)Language.Korean] = GetCell(row, (int)ShopDataRowKey.Description_Kr);
            shopDescription[(int)Language.English] = GetCell(row, (int)ShopDataRowKey.Description_En);
            shopDescription[(int)Language.Japanese] = GetCell(row, (int)ShopDataRowKey.Description_Jp);
            shopDescription[(int)Language.Chinese] = GetCell(row, (int)ShopDataRowKey.Description_Ch);

            // 언어별 BaseCategory
            string[] baseCategoryString = new string[(int)Language.EndOfIndex];
            baseCategoryString[(int)Language.Korean] = GetCell(row, (int)ShopDataRowKey.BaseCategory_Kr);
            baseCategoryString[(int)Language.English] = GetCell(row, (int)ShopDataRowKey.BaseCategory_En);
            baseCategoryString[(int)Language.Japanese] = GetCell(row, (int)ShopDataRowKey.BaseCategory_Jp);
            baseCategoryString[(int)Language.Chinese] = GetCell(row, (int)ShopDataRowKey.BaseCategory_Ch);

            // 언어별 SecondCategory
            string[] secondCategoryString = new string[(int)Language.EndOfIndex];
            secondCategoryString[(int)Language.Korean] = GetCell(row, (int)ShopDataRowKey.SecondCategory_Kr);
            secondCategoryString[(int)Language.English] = GetCell(row, (int)ShopDataRowKey.SecondCategory_En);
            secondCategoryString[(int)Language.Japanese] = GetCell(row, (int)ShopDataRowKey.SecondCategory_Jp);
            secondCategoryString[(int)Language.Chinese] = GetCell(row, (int)ShopDataRowKey.SecondCategory_Ch);

            // 언어별 AICategory
            string[] aiCategoryString = new string[(int)Language.EndOfIndex];
            aiCategoryString[(int)Language.Korean] = GetCell(row, (int)ShopDataRowKey.AICategory_Kr);
            aiCategoryString[(int)Language.English] = GetCell(row, (int)ShopDataRowKey.AICategory_En);
            aiCategoryString[(int)Language.Japanese] = GetCell(row, (int)ShopDataRowKey.AICategory_Jp);
            aiCategoryString[(int)Language.Chinese] = GetCell(row, (int)ShopDataRowKey.AICategory_Ch);

            string openingTime = GetCell(row, (int)ShopDataRowKey.OpeningTime);
            string contactNumber = GetCell(row, (int)ShopDataRowKey.ContactNumber);
            string naverLink = GetCell(row, (int)ShopDataRowKey.NaverLink);

            // 객체 생성 및 초기화
            var shop = new BaseShopInfoData
                (
                shopId, shopName, address, hashTag, shopDescription, baseCategoryString,
                secondCategoryString, aiCategoryString, openingTime, contactNumber, naverLink
                );

            // 이미지 할당 (예외 안전)
            if (ResourceManager.Instance.ShopSpritesDic.TryGetValue(shop.ShopName[(int)Language.Korean], out var images))
                shop.spriteImage = images.ToArray();
            else
            {
                Debug.Log($"[ShopSheetParser] {shop.ShopName[(int)Language.Korean]}의 이미지를 찾을 수 없습니다.");
            }


            result.Add(shop);
        }

        return result;
    }
    public static List<AICategory> AICategoryParser(ValueRange sheet)
    {
        var result = new List<AICategory>();

        var rows = sheet?.Values;

        if (rows == null || rows.Count < 2)
        {
            Debug.LogWarning("시트에 데이터가 충분하지 않습니다.");
            return result;
        }

        for (int i = 0; i < rows.Count; i++)    // 처음부터 바로 데이터라서 0으로 시작
        {
            var row = rows[i];

            if (row.Count < 2 || string.IsNullOrWhiteSpace(row[0]?.ToString()))
                continue;

            if (!int.TryParse(row[0].ToString(), out int num))
                continue;

            // AICategory 객체 생성 및 초기화
            var aiCategory = new AICategory { Num = num };


            for (int j = 0; j < row.Count && j < (int)Language.EndOfIndex; j++)
            {
                aiCategory.AICategoryString[j] = GetCell(row, j + 1);
            }
            result.Add(aiCategory);
        }
        return result;
    }
    public static List<PalaceData> PalaceDataParser(ValueRange sheet)
    {
        var result = new List<PalaceData>();
        var rows = sheet?.Values;

        if (rows == null || rows.Count < 2)
        {
            Debug.LogWarning("시트에 데이터가 충분하지 않습니다.");
            return result;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var palaceData = new PalaceData();

            // 1열: Num
            palaceData.Num = int.TryParse(GetCell(row, 0), out int num) ? num : 0;

            // 2~5열: PalaceNameString[0~3]
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                palaceData.PalaceNameString[lang] = GetCell(row, 1 + lang);
            }

            // 6~9열: PalaceAddressString[0~3]
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                palaceData.PalaceAddressString[lang] = GetCell(row, 5 + lang);
            }

            // 10~13열: HashTagString[0~3]
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                palaceData.HashTagString[lang] = GetCell(row, 9 + lang);
            }
            // 14~17열: DescriptionString[0~3]
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                palaceData.DescriptionString[lang] = GetCell(row, 13 + lang);
            }

            // 업무시간
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                palaceData.OpeningTime[lang] = GetCell(row, 17 + lang);
            }

            // 볼거리
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                palaceData.Attraction[lang] = GetCell(row, 21 + lang);
            }

            // 입장료
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                palaceData.Fee[lang] = GetCell(row, 25 + lang);
            }

            palaceData.ContactNum = GetCell(row, 29);

            result.Add(palaceData);
        }
        return result;
    }
    public static List<EventData> EventDataParser(ValueRange sheet)
    {
        var result = new List<EventData>();
        var rows = sheet?.Values;

        if (rows == null || rows.Count < 2)
        {
            Debug.LogWarning("시트에 데이터가 충분하지 않습니다.");
            return result;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var eventData = new EventData();

            // 1열: Num
            eventData.Num = int.TryParse(GetCell(row, 0), out int num) ? num : 0;

            eventData.EventState = int.TryParse(GetCell(row, 1), out int state) ? (EventState)state : 0;

            // 3~6열: 이벤트 이름
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                eventData.EventNameString[lang] = GetCell(row, 2 + lang);
            }

            // 7~10열: AddressString[0~3]
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                eventData.EventAddressString[lang] = GetCell(row, 6 + lang);
            }

            // 10~13열: HashTagString[0~3]
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                eventData.HashTagString[lang] = GetCell(row, 10 + lang);
            }
            // 14~17열: DescriptionString[0~3]
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                eventData.DescriptionString[lang] = GetCell(row, 14 + lang);
            }

            // 주최사
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                eventData.Host[lang] = GetCell(row, 18 + lang);
            }

            // 관람 연령
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                eventData.Age[lang] = GetCell(row, 22 + lang);
            }

            // 입장료
            for (int lang = 0; lang < (int)Language.EndOfIndex; lang++)
            {
                eventData.Fee[lang] = GetCell(row, 26 + lang);
            }

            eventData.OpeningTime = GetCell(row, 30);   // 운영시간
            eventData.ContactNum = GetCell(row, 31);    // 전화번호
            eventData.Period = GetCell(row, 32);        // 기간
            eventData.ImageUrl = GetCell(row, 33);      // 이미지URL

            result.Add(eventData);
        }
        return result;
    }

    private static string GetCell(IList<object> row, int index)
    {
        return (row.Count > index && row[index] != null) ? row[index].ToString() : string.Empty;
    }
}

[Serializable]
public class AICategory
{
    public int Num;
    public string[] AICategoryString = new string[(int)Language.EndOfIndex];
    // 행정보 1. Num , 2. Korean, 3. English, 4. Japanese, 5. Chinese
}

[Serializable]
public class PalaceData
{
    public int Num; // 번호
    public string[] PalaceNameString = new string[(int)Language.EndOfIndex]; // 업체명
    public string[] PalaceAddressString = new string[(int)Language.EndOfIndex]; // 주소
    public string[] HashTagString = new string[(int)Language.EndOfIndex]; // 해시태그
    public string[] DescriptionString = new string[(int)Language.EndOfIndex]; // 정보
    public string[] OpeningTime = new string[(int)Language.EndOfIndex]; // 운영시간
    public string[] Attraction = new string[(int)Language.EndOfIndex]; // 볼거리
    public string[] Fee = new string[(int)Language.EndOfIndex]; // 입장료

    public string ContactNum; // 연락처
}

[Serializable]
public class EventData
{
    public int Num; // 번호
    public EventState EventState; // 0 종료, 1 오늘, 2 예정된 , 3 종료

    public string[] EventNameString = new string[(int)Language.EndOfIndex]; // 업체명
    public string[] EventAddressString = new string[(int)Language.EndOfIndex]; // 주소
    public string[] HashTagString = new string[(int)Language.EndOfIndex]; // 해시태그
    public string[] DescriptionString = new string[(int)Language.EndOfIndex]; // 정보
    public string[] Host = new string[(int)Language.EndOfIndex];
    public string[] Age = new string[(int)Language.EndOfIndex]; // 볼거리
    public string[] Fee = new string[(int)Language.EndOfIndex]; // 입장료

    public string OpeningTime;
    public string ContactNum; // 연락처
    public string Period; // 기간

    public string ImageUrl;

    public Sprite ThumbNailImage;
}

[Serializable]
public class LocalizationText
{
    public int Num;
    public string Key;
    public string[] Text = new string[(int)Language.EndOfIndex];
}
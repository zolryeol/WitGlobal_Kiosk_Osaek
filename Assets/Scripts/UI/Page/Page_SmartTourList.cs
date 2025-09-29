using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
///  스마트 투어 리스트 페이지
///  기존 ShopList와 통합하려했으나 추후 예외가 생길것으로 예상되어 따로 분리
/// </summary>

public class Page_SmartTourList : MonoBehaviour
{
    public List<SecondCategoryButton> SecondCategoryButtons = new();
    public List<SecondCategoryButton> SecondCategoryButtons_SeriviceArea = new();
    Transform body;
    Transform contentParent;
    Transform categoryButtonParent;
    Transform categoryButtonParent_ServiceArea;

    public List<ShopContent> SmartTourContentList = new();
    public void Init()
    {
        body = CommonFunction.FindDeepChild(this.gameObject, "Body").transform;
        categoryButtonParent = CommonFunction.FindDeepChild(body.gameObject, "ButtonsParent").transform;

        // Attraction 카테고리 세팅
        for (int i = 0; i < 5; i++)
        {
            var b = categoryButtonParent.GetChild(i).GetComponent<SecondCategoryButton>();
            b.SecondCategoryButtonIndex = i + 1; // 데이터에 인덱스는 1부터 시작
            SecondCategoryButtons.Add(b);
            b.onClick.AddListener(() => FetchingContent(UIManager.Instance.NowSelectedKoreaMapName, b.SecondCategoryButtonIndex));
            UIManager.Instance.SecondCategorieButtons.Add(b);
        }

        categoryButtonParent_ServiceArea = CommonFunction.FindDeepChild(body.gameObject, "ButtonsParent_ServiceArea").transform;
        // ServiceArea 카테고리 세팅
        for (int i = 0; i < 18; i++)
        {
            var b = categoryButtonParent_ServiceArea.GetChild(i).GetComponent<SecondCategoryButton>();
            b.SecondCategoryButtonIndex = i + 1; // 데이터에 인덱스는 1부터 시작
            SecondCategoryButtons_SeriviceArea.Add(b);
            b.onClick.AddListener(() => FetchingContent(b.SecondCategoryButtonIndex, null));
            UIManager.Instance.SecondCategorieButtons.Add(b);
        }

        contentParent = CommonFunction.FindDeepChild(body.gameObject, "Content").transform;

        CreateContentInstance();
    }
    public void CreateContentInstance()
    {
        int maxValue = new[]
{
    LoadManager.Instance.TraditionalMarketList.Count,
    LoadManager.Instance.AttractionList.Count,
    LoadManager.Instance.ServiceAreaList.Count
        }.Max();

        var childCount = contentParent.childCount;

        var addedCount = maxValue - childCount;

        for (int i = 0; i < addedCount; i++)
        {
            var content = Instantiate(PrefabManager.Instance.ContentItemInfoSmartTourPrafab, contentParent);
            content.SetActive(false);
            SmartTourContentList.Add(content.GetComponent<ShopContent>());
        }

        Debug.Log($"<color=blue>스마트관광 콘텐츠 {maxValue} 개 인스턴싱</color>");
    }

    public void FetchingContent(string districtName) // 지역으로 바꾸어야함;
    {

        var um = UIManager.Instance.NowSelectedETC;

        IEnumerable<BaseShopInfoData> _target = Enumerable.Empty<BaseShopInfoData>();

        switch (um)
        {
            case Category_ETC.TraditionalMarket:
                _target = LoadManager.Instance.TraditionalMarketList;
                break;
            case Category_ETC.Attraction:
                _target = LoadManager.Instance.AttractionList;
                break;
            case Category_ETC.ServiceArea:
                _target = LoadManager.Instance.ServiceAreaList;
                break;
            default:
                Debug.LogError("스마트투어 리스트 페이지에서 카테고리 에러");
                break;
        }

        var firstContents = _target
            .Where(t => t.SecondCategoryString[(int)Language.Korean] == districtName).ToList();


        List<BaseShopInfoData> secondContents = new();

        if (firstContents.Count == 0)
        {
            firstContents = _target
                .Where(t => t.BaseCategoryString[(int)Language.Korean] == UIManager.Instance.NowSelectedKoreaMapName)
                .ToList();
        }
        else
        {
            secondContents = _target
                .Where(t => t.BaseCategoryString[(int)Language.Korean] == firstContents[0].BaseCategoryString[(int)Language.Korean])
                .Except(firstContents)
                .ToList();
        }

        if (firstContents.Count == 0)
        {
            Debug.Log("콘텐츠가 없다...");
        }

        var finalList = firstContents.Concat(secondContents).ToList();


        Debug.Log("콘텐츠 갯수 = " + finalList.Count);


        for (int i = 0; i < SmartTourContentList.Count; ++i)
        {
            if (i < finalList.Count)
            {
                SmartTourContentList[i].gameObject.SetActive(true);
                SmartTourContentList[i].FetchContent(finalList[i]);
            }
            else
            {
                SmartTourContentList[i].gameObject.SetActive(false);
            }
        }

        //InitScrollbarValue(FoodAndShopScrollbar);
    }
    public void FetchingContent(string districtName, int categoryIndex) //  관광지전용;
    {
        var _target = LoadManager.Instance.AttractionList;

        var firstFilter = _target
    .Where(t => t.SecondCategoryString[(int)Language.Korean] == districtName).ToList();

        var _targetContents = firstFilter
        .Where(t =>
        {
            string str = t.Category[(int)Language.Korean];
            int idx = str.IndexOf('_');
            string prefix = (idx >= 0) ? str.Substring(0, idx) : str;
            return prefix == (categoryIndex).ToString();
        })
        .ToList();

        for (int i = 0; i < SmartTourContentList.Count; ++i)
        {
            if (i < _targetContents.Count)
            {
                SmartTourContentList[i].gameObject.SetActive(true);
                SmartTourContentList[i].FetchContent(_targetContents[i]);
            }
            else
            {
                SmartTourContentList[i].gameObject.SetActive(false);
            }
        }

        //InitScrollbarValue(FoodAndShopScrollbar);
    }

    public void FetchingContent(int categoryIndex = 0, string districtName = null) //  휴게소전용;
    {
        var _target = LoadManager.Instance.ServiceAreaList;
        string provinceName;

        if (districtName != null)
        {
            provinceName = districtName;
        }
        else
        {
            provinceName = GetProvinceNameByIndex(categoryIndex);
        }

        var firstFilter = _target
    .Where(t => t.BaseCategoryString[(int)Language.Korean] == provinceName).ToList();

        for (int i = 0; i < SmartTourContentList.Count; ++i)
        {
            if (i < firstFilter.Count)
            {
                SmartTourContentList[i].gameObject.SetActive(true);
                SmartTourContentList[i].FetchContent(firstFilter[i]);
            }
            else
            {
                SmartTourContentList[i].gameObject.SetActive(false);
            }
        }
        //InitScrollbarValue(FoodAndShopScrollbar);
    }

    public void OnCategoryButton(bool _active = false)
    {
        categoryButtonParent.gameObject.SetActive(_active);

        if (_active == false) return;

        // 카테고리 텍스트 갱신
        var nowLang = UIManager.Instance.NowLanguage;
        var attractionList = LoadManager.Instance.AttractionList;

        foreach (var btn in SecondCategoryButtons)
        {
            // 버튼의 Index
            string cateIndex = btn.SecondCategoryButtonIndex.ToString();

            // 현재 언어 기준으로 매칭되는 문자열 찾기
            var matched = attractionList
                .Select(t => t.Category[(int)nowLang])      // "1_문화재" 같은 문자열 꺼내기
                .FirstOrDefault(c => c.Split('_')[0] == cateIndex);

            if (!string.IsNullOrEmpty(matched))
            {
                // "_" 뒤 문자열만 잘라내기
                string displayText = matched.Split('_')[1];

                // 버튼 자식의 TextMeshProUGUI 갱신
                btn.UpdateLocalizedString(displayText);
            }
        }
    }
    public void OnCategoryButtonServiceArea(bool _active = false)
    {
        categoryButtonParent_ServiceArea.gameObject.SetActive(_active);

        if (_active == false) return;

        var nowLang = UIManager.Instance.NowLanguage;
        var serviceAreaList = LoadManager.Instance.ServiceAreaList;

        foreach (var btn in SecondCategoryButtons_SeriviceArea)
        {
            string province = GetProvinceNameByIndex(btn.SecondCategoryButtonIndex);

            // province 에 해당하는 데이터 찾기
            var match = serviceAreaList
                .FirstOrDefault(t => t.BaseCategoryString[(int)Language.Korean] == province);

            btn.UpdateLocalizedString(match.BaseCategoryString[(int)nowLang]);
        }
    }
    string GetProvinceNameByIndex(int _index)
    {
        string province = _index switch
        {
            1 => "서울",
            2 => "경기도",
            3 => "인천",
            4 => "대전",
            5 => "세종",
            6 => "대구",
            7 => "전주",
            8 => "광주",
            9 => "울산",
            10 => "부산",
            11 => "충청북도",
            12 => "충청남도",
            13 => "경상북도",
            14 => "경상남도",
            15 => "전라북도",
            16 => "전라남도",
            17 => "강원도",
            18 => "제주도",
            _ => null,
        };
        return province;
    }

    public int GetProvinceIndexByName(string _name)
    {
        int index = _name switch
        {
            "서울" => 1,
            "경기도" => 2,
            "인천" => 3,
            "대전" => 4,
            "세종" => 5,
            "대구" => 6,
            "전주" => 7,
            "광주" => 8,
            "울산" => 9,
            "부산" => 10,
            "충청북도" => 11,
            "충청남도" => 12,
            "경상북도" => 13,
            "경상남도" => 14,
            "전라북도" => 15,
            "전라남도" => 16,
            "강원도" => 17,
            "제주도" => 18,
            _ => -1,
        };
        return index;
    }
}
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AIListApiDataFetcher : MonoBehaviour
{
    // 첫번째 열 데이터들
    [SerializeField]
    private TextMeshProUGUI[] firstNo;
    [SerializeField]
    private TextMeshProUGUI[] firstRowCategory;
    [SerializeField]
    private TextMeshProUGUI[] firstRowName;
    [SerializeField]
    private TextMeshProUGUI[] firstRowCategoryName;
    [SerializeField]
    private Image[] firstRowImage;
    [SerializeField]
    private TextMeshProUGUI[] firstRowHashtag;
    [SerializeField]
    private RawImage firstQRCode;

    // 두번째 열 데이터들
    [SerializeField]
    private TextMeshProUGUI[] secondNo;
    [SerializeField]
    private TextMeshProUGUI[] secondRowCategory;
    [SerializeField]
    private TextMeshProUGUI[] secondRowName;
    [SerializeField]
    private TextMeshProUGUI[] secondRowCategoryName;
    [SerializeField]
    private Image[] secondRowImage;
    [SerializeField]
    private TextMeshProUGUI[] secondRowHashtag;
    [SerializeField]
    private RawImage secondQRCode;

    // 세번째 열 데이터들
    [SerializeField]
    private TextMeshProUGUI[] thirdNo;
    [SerializeField]
    private TextMeshProUGUI[] thirdRowCategory;
    [SerializeField]
    private TextMeshProUGUI[] thirdRowName;
    [SerializeField]
    private TextMeshProUGUI[] thirdRowCategoryName;
    [SerializeField]
    private Image[] thirdRowImage;
    [SerializeField]
    private TextMeshProUGUI[] thirdRowHashtag;
    [SerializeField]
    private RawImage thirdQRCode;

    [SerializeField]
    private Sprite noImage;

    [SerializeField]
    private Sprite qr_preImage;

    private HttpClient client;


    bool qrFlag1 = true;
    bool qrFlag2 = true;
    bool qrFlag3 = true;



    private void Awake()
    {
        client = new HttpClient();
    }

    async void Start()
    {
        // 총 3번 반복, 각 호출에 대하여 A, B, C코스가 리턴
        for(int i = 0; i < 3; i++)
        {
            // 언어 설정
            string language = LanguageService.apiLanguageParse().ToLower();

            // 카테고리 값
            int categoryIndex = PlayerPrefs.GetInt($"AISelectCategory{i + 1}");
            string aiCategoryStr = CategoryController.GetCategory(MenuName.AI, categoryIndex, language);
            PlayerPrefs.SetString("aiCategory", aiCategoryStr);
            Debug.Log("i 번째 카테고리 : " + aiCategoryStr);
            // API URL
            string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

            // POST로 보낼 데이터 생성
            var postData = new
            {
                main = "인사 뭐하지",
                lang = language,
                input = "",
                ca2 = aiCategoryStr,
                store_no = "",
                visitor = PlayerPrefs.GetInt("AISelectVisit"),
                hour = PlayerPrefs.GetInt("AISelectTime")
            };

            Debug.LogError($"post: {postData}");

            string jsonData = JsonConvert.SerializeObject(postData);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // API 호출
            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {                
                string result = await response.Content.ReadAsStringAsync();

                // JSON 데이터를 AIListData 객체로 역직렬화
                var aiData = JsonConvert.DeserializeObject<AIListData>(result);
                
                // AI 리스트 뿌려주기
                int count = 0;  // 데이터 순번 표시(0은 A, 1은 B, 2는 C)
                foreach (var ai in aiData.response)
                {
                    // 번호, 카테고리, 이름, 이미지, 해시태그 뿌려주기
                    // i가 0이면 첫번째 열로, 1면 두번째 열로, 2면 세번째 열로
                    // count = 코스

                    // 1열 설정
                    if (i == 0)
                    {
                        if (qrFlag1) {
                            //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.address)))}&flag=ai&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.name)))}&apn=com.witdiocianapp", firstQRCode);
                            firstQRCode.texture = qr_preImage.texture;

                            qrFlag1 = false;
                        }
                        Debug.Log($"i{i}count{count}");
                        // 번호 설정
                        firstNo[count].text = ai.store_no;

                        //Debug.LogError($"store_no: {ai.store_no}");

                        // 카테고리 설정
                        //firstRowCategory[count].text = ai.ca2.Split('-')[1];
                        firstRowCategory[count].text = ai.ca2;

                        // 이름 설정
                        firstRowName[count].text = ai.name;

                        // 카테고리 & 이름설정
                        //firstRowCategoryName[count].text = $"{ai.name} <color=#999999><size=24>  •   {ai.ca2.Split('-')[1]}</size></color>";
                        firstRowCategoryName[count].text = $"{ai.name} <color=#999999><size=24>  •   {ai.ca2}</size></color>";

                        for (int j = 0; j < 2; j++)
                        {
                            // 해시태그 설정
                            string hashtag = "";
                            string[] hashtagArr = ai.keyword.Split(", ");
                            for (int k = 0; k < hashtagArr.Length; k++)
                            {
                                hashtag += "#" + hashtagArr[k] + "  ";
                            }
                            firstRowHashtag[count * 2 + j].text = hashtag;

                            // 이미지 설정
                            string imageUrl = ai.img_url_1;
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                StartCoroutine(ImageLoader.LoadImages(imageUrl, firstRowImage[count * 2 + j], noImage));
                            }
                            else
                            {
                                firstRowImage[count * 2 + j].sprite = noImage;
                            }
                        }
                    }
                    // 2열 설정
                    else if(i == 1)
                    {
                        if (qrFlag2)
                        {
                            //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.address)))}&flag=ai&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.name)))}&apn=com.witdiocianapp", secondQRCode);
                            secondQRCode.texture = qr_preImage.texture;
                            qrFlag2 = false;
                        }
                        // 번호 설정
                        secondNo[count].text = ai.store_no;

                        // 카테고리 설정
                        //secondRowCategory[count].text = ai.ca2.Split('-')[1];
                        secondRowCategory[count].text = ai.ca2;

                        // 이름 설정
                        secondRowName[count].text = ai.name;

                        // 카테고리 & 이름설정
                        //secondRowCategoryName[count].text = $"{ai.name} <color=#999999><size=24>  •   {ai.ca2.Split('-')[1]}</size></color>";
                        secondRowCategoryName[count].text = $"{ai.name} <color=#999999><size=24>  •   {ai.ca2}</size></color>";

                        for (int j = 0; j < 2; j++)
                        {
                            // 해시태그 설정
                            string hashtag = "";
                            string[] hashtagArr = ai.keyword.Split(", ");
                            for (int k = 0; k < hashtagArr.Length; k++)
                            {
                                hashtag += "#" + hashtagArr[k] + "  ";
                            }
                            secondRowHashtag[count * 2 + j].text = hashtag;

                            // 이미지 설정
                            string imageUrl = ai.img_url_1;
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                StartCoroutine(ImageLoader.LoadImages(imageUrl, secondRowImage[count * 2 + j], noImage));
                            }
                            else
                            {
                                secondRowImage[count * 2 + j].sprite = noImage;
                            }
                        }
                    }
                    // 3열 설정
                    else if (i == 2)
                    {
                        if (qrFlag3)
                        {
                            //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.address)))}&flag=ai&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.name)))}&apn=com.witdiocianapp", thirdQRCode);                            
                            thirdQRCode.texture = qr_preImage.texture;
                            qrFlag3 = false;
                        }
                        // 번호 설정
                        thirdNo[count].text = ai.store_no;

                        // 카테고리 설정
                        //thirdRowCategory[count].text = ai.ca2.Split('-')[1];
                        thirdRowCategory[count].text = ai.ca2;

                        // 이름 설정
                        thirdRowName[count].text = ai.name;

                        // 카테고리 & 이름설정
                        //thirdRowCategoryName[count].text = $"{ai.name} <color=#999999><size=24>  •   {ai.ca2.Split('-')[1]}</size></color>";
                        thirdRowCategoryName[count].text = $"{ai.name} <color=#999999><size=24>  •   {ai.ca2}</size></color>";

                        for (int j = 0; j < 2; j++)
                        {
                            // 해시태그 설정
                            string hashtag = "";
                            string[] hashtagArr = ai.keyword.Split(", ");
                            for (int k = 0; k < hashtagArr.Length; k++)
                            {
                                hashtag += "#" + hashtagArr[k] + "  ";
                            }
                            thirdRowHashtag[count * 2 + j].text = hashtag;

                            // 이미지 설정
                            string imageUrl = ai.img_url_1;
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                StartCoroutine(ImageLoader.LoadImages(imageUrl, thirdRowImage[count * 2 + j], noImage));
                            }
                            else
                            {
                                thirdRowImage[count * 2 + j].sprite = noImage;
                            }
                        }
                    }


                    count++;
                }
            }
        }
    }
}
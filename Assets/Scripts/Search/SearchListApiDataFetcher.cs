using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SearchListApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private GameObject searchInfoPrefab;
    [SerializeField]
    private Sprite noImage;

    // 프리팹 생성될 위치
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private Sprite qr_preImage;

    private HttpClient client;

    private void Awake()
    {
        client = new HttpClient();
    }

    async void Start()
    {
        // 검색어 가져오기
        string searchKeyword = PlayerPrefs.GetString("searchKeyword");

        searchKeyword = searchKeyword.Replace("\u200b", "");  // Zero-width space 제거

        // 언어 설정
        string language = LanguageService.apiLanguageParse();

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // POST로 보낼 데이터 생성, 카테고리검색인지 이름검색인지 분기처리
        object postData = null;

        postData = new
        {
            //main = "검색 인사동",
            main = "",
            lang = language,
            input = searchKeyword,
            ca2 = "",
            store_no = ""
        };

        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 SearchData 객체로 역직렬화
            var searchData = JsonConvert.DeserializeObject<SearchListData>(result);

            // 검색결과 리스트 뿌려주기
            foreach (var search in searchData.response)
            {
                // 프리팹 인스턴스 생성
                GameObject instance = Instantiate(searchInfoPrefab, targetTransform);

                // 검색결과 번호 설정
                instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = search.no;
               
                // 이름 및 카테고리 설정
                instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{search.store}  <color=#FE6C50>•</color>  <size=24><color=#999999>{search.category}</color></size>";
               
                // 주소 설정
                instance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = search.address;
              
                // 시간 설정
                string time = search.time.Equals("Null") ? "-" : search.time;
                instance.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = time;

                // 해시태그 설정
                string[] keywordArr = search.keyword.Split(", ");
                string hashtag = "";
                for (int i = 0; i < keywordArr.Length; i++)
                {
                    hashtag += $"#{keywordArr[i]} ";
                }
                instance.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = hashtag;

                // 사진 뿌려주기
                Image[] images = instance.GetComponentsInChildren<Image>();
                images = Array.FindAll(images, image => image.gameObject != instance);
                for (int i = 1; i <= images.Length; i++)
                {
                    // QR은 뿌려주면 안됨
                    if (i == images.Length) continue;

                    string fieldName = $"img_url_{i}";
                    string imageUrl = (string)search.GetType().GetProperty(fieldName)?.GetValue(search, null);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        StartCoroutine(ImageLoader.LoadImages(imageUrl, images[i - 1], noImage));
                    }
                    else
                    {
                        images[i - 1].sprite = noImage;
                    }
                }

                RawImage qrImage = instance.transform.GetChild(6).GetComponentInChildren<RawImage>();
                qrImage.texture = qr_preImage.texture;

                if (search.link != null)
                {
                    // QR 코드 생성
                    //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(search.address)))}&flag=search&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(search.store)))}&apn=com.witdiocianapp", instance.transform.GetChild(6).GetComponentInChildren<RawImage>());
                    QRCreator.CreateQR($"{search.link}", instance.transform.GetChild(6).GetComponentInChildren<RawImage>());
                }
            }
        }
        else
        {
            Debug.LogError("POST 요청 실패: " + response.StatusCode);
        }
        languageChange();
    }

    private void languageChange()
    {
        //LanguageService ls = new LanguageService();
        //ls.museumSceneInit();
        //string currentLanguage = LanguageService.getCurrentLanguage();
        //Dictionary<string, Dictionary<string, string>> res = ls.languageChange("Museum");

        //string[] keysArray = res.Keys.ToArray();
        //foreach (string key in keysArray)
        //{
        //    Debug.Log(key);
        //    // key와 동일한 이름의 GameObject를 찾음
        //    GameObject targetObject = GameObject.Find(key);
        //    if (targetObject != null)
        //    {
        //        // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
        //        TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

        //        if (textComponent != null)
        //        {
        //            // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
        //            // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
        //            textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용                    
        //            TMP_FontAsset font = LanguageService.getFontAsset(currentLanguage);
        //            textComponent.font = font;
        //            textComponent.fontSize = 28;
        //            if (textComponent.name == "Text01")
        //            {
        //                textComponent.fontSize = 40;
        //                textComponent.fontStyle = FontStyles.Bold;
        //            }
        //            else if (font.name == "JPCN")
        //            {
        //                textComponent.fontStyle = FontStyles.Bold;
        //            }
        //            else if (currentLanguage == "EN") textComponent.fontSize = 20;
        //            else textComponent.fontStyle = FontStyles.Bold;


        //        }
        //        else
        //        {
        //            Debug.LogWarning($"TextMeshProUGUI component not found in {key}");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning($"GameObject with name {key} not found");
        //    }
        //}
    }

}
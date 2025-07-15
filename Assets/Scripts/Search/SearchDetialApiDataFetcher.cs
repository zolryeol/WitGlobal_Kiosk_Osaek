using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SearchDetailApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private Sprite noImage;

    [SerializeField]
    private Sprite qr_preImage;

    [SerializeField]
    private TextMeshProUGUI searchName;
    [SerializeField]
    private TextMeshProUGUI searchAddress;
    [SerializeField]
    private TextMeshProUGUI searchTime;
    [SerializeField]
    private TextMeshProUGUI searchTelno;
    [SerializeField]
    private TextMeshProUGUI searchDescription;
    [SerializeField]
    private TextMeshProUGUI searchHashtag;
    [SerializeField]
    private ScoreController scoreController;

    private HttpClient client;

    private void Awake()
    {
        client = new HttpClient();
    }

    async void Start()
    {
        languageChange();

        // 언어 설정
        string language = LanguageService.apiLanguageParse();

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // 상점번호 가져오기
        string searchNo = PlayerPrefs.GetString("searchNo");
        
        // POST로 보낼 데이터 생성
        var postData = new
        {
            main = "",
            lang = language,
            ca2 = "",
            input = "",
            store_no = searchNo
        };
        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 SearchData 객체로 역직렬화
            var searchData = JsonConvert.DeserializeObject<SearchDetailData>(result);
            var search = searchData.response;

            // 사진 설정
            for (int i = 1; i <= images.Length; i++)
            {
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

            // 검색결과 이름 및 카테고리 설정
            searchName.text = $"{search.name} <color=#FE6C50>•</color>  <size=24><color=#999999>{search.ca2}</color></size>";

            // 검색결과 주소
            searchAddress.text = search.address;

            // 검색결과 시간
            string time = search.time.Equals("NaN") ? "-" : search.time;
            searchTime.text = time;

            // 검색결과 전화번호
            string telNo = search.tel_no.Equals("NaN") ? "-" : search.tel_no;
            searchTelno.text = telNo;

            // 검색결과 설명
            string description = search.intro.Equals("NaN") ? "-" : search.intro;
            searchDescription.text = description;

            // 검색결과 해시태그
            string hashtag = "";
            if (!string.IsNullOrEmpty(search.keyword))
            {
                string[] keywordArr = search.keyword.Split(", ");
                for (int i = 0; i < keywordArr.Length; i++)
                {
                    hashtag += $"#{keywordArr[i]} ";
                }
            }
            searchHashtag.text = hashtag;

            // 검색결과 별점
            scoreController.UpdateScore(search.naver_score);

            RawImage qrImage = GameObject.Find("searchQR").GetComponentInChildren<RawImage>();
            qrImage.texture = qr_preImage.texture;

            if (search.link != null)
            {
                // QR 코드 생성
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(search.address)))}&flag=search&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(search.name)))}&apn=com.witdiocianapp", GameObject.Find("searchQR").GetComponentInChildren<RawImage>());
                QRCreator.CreateQR($"{search.link}", GameObject.Find("searchQR").GetComponentInChildren<RawImage>());
            }
        }
        else
        {
            Debug.LogError("POST 요청 실패: " + response.StatusCode);
        }
    }

    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.detailSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("Detail");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray)
        {
            Debug.Log(key);
            // key와 동일한 이름의 GameObject를 찾음
            GameObject targetObject = GameObject.Find(key);
            if (targetObject != null)
            {
                // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
                    // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
                    textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용    
                }
                else
                {
                    Debug.LogWarning($"TextMeshProUGUI component not found in {key}");
                }
            }
            else
            {
                Debug.LogWarning($"GameObject with name {key} not found");
            }
        }
    }
}
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

public class AIDetailApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private Sprite noImage;

    [SerializeField]
    private Sprite qr_preImage;

    [SerializeField]
    private TextMeshProUGUI aiName;
    [SerializeField]
    private TextMeshProUGUI aiAddress;
    [SerializeField]
    private TextMeshProUGUI aiTime;
    [SerializeField]
    private TextMeshProUGUI aiTelno;
    [SerializeField]
    private TextMeshProUGUI aiDescription;
    [SerializeField]
    private TextMeshProUGUI aiHashtag;
    [SerializeField]
    private ScoreController scoreController;    



    private HttpClient client;

    private void Awake()
    {
        client = new HttpClient();        
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
            // key와 동일한 이름의 GameObject를 찾음
            GameObject targetObject = GameObject.Find(key);
            Debug.Log(targetObject);
            if (targetObject != null)
            {
                // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                Debug.Log(textComponent.text);

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
    async public void UpdateDetail(string aiNo)
    {

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // 언어 설정
        string language = LanguageService.apiLanguageParse();

        // POST로 보낼 데이터 생성
        var postData = new
        {
            main = "",
            lang = language,
            input = "",
            ca2 = "",
            store_no = aiNo
        };

        Debug.LogError($"post : {postData}");

        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 AIDetailData 객체로 역직렬화
            var aiData = JsonConvert.DeserializeObject<AIDetailData>(result);
            var ai = aiData.response;

            // 상점 사진 설정
            for (int i = 1; i <= images.Length; i++)
            {
                string fieldName = $"img_url_{i}";
                string imageUrl = (string)ai.GetType().GetProperty(fieldName)?.GetValue(ai, null);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    StartCoroutine(ImageLoader.LoadImages(imageUrl, images[i - 1], noImage));
                }
                else
                {
                    images[i - 1].sprite = noImage;
                }
            }

            // AI 이름 및 카테고리 설정
            //aiName.text = $"{ai.name} <color=#FE6C50>•</color>  <size=24><color=#999999>{ai.ca2.Split('-')[1]}</color></size>";
            aiName.text = $"{ai.name} <color=#FE6C50>•</color>  <size=24><color=#999999>{ai.ca2}</color></size>";

            // AI 주소
            aiAddress.text = ai.address;

            // AI 시간
            string time = ai.time.Equals("NaN") ? "-" : ai.time;
            aiTime.text = time;
            Debug.LogError($"time\n{time}");
            aiTime.ForceMeshUpdate();

            // AI 전화번호
            string telNo = ai.tel_no.Equals("NaN") ? "-" : ai.tel_no;
            aiTelno.text = telNo;

            // AI 설명
            string description = ai.intro.Equals("NULL") ? "-" : ai.intro;
            aiDescription.text = description;

            // AI 해시태그
            string hashtag = "";
            if (!string.IsNullOrEmpty(ai.keyword))
            {
                string[] keywordArr = ai.keyword.Split(", ");
                for (int i = 0; i < keywordArr.Length; i++)
                {
                    hashtag += $"#{keywordArr[i]} ";
                }
            }
            aiHashtag.text = hashtag;

            // AI 별점
            scoreController.UpdateScore(ai.naver_score);


            RawImage qr_image = GameObject.Find("QRRaw").GetComponentInChildren<RawImage>();
            qr_image.texture = qr_preImage.texture;

            if (ai.link != null)
            {
                // QR 코드 생성
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.address)))}&flag=ai&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(ai.name)))}&apn=com.witdiocianapp", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());
                QRCreator.CreateQR($"{ai.link}", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());
            }
            languageChange();
        }
        else
        {
            Debug.LogError("POST 요청 실패: " + response.StatusCode);
        }
    }
}
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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpDetailApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private Sprite noImage;

    [SerializeField]
    private Sprite qr_preImage;

    [SerializeField]
    private TextMeshProUGUI helpName;
    [SerializeField]
    private TextMeshProUGUI helpAddress;
    [SerializeField]
    private TextMeshProUGUI helpTime;
    [SerializeField]
    private TextMeshProUGUI helpTelno;
    [SerializeField]
    private TextMeshProUGUI helpDescription;
    [SerializeField]
    private TextMeshProUGUI helpHashtag;
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

        // 카테고리 설정
        string helpCategoryStr = PlayerPrefs.GetString("helpCategory");

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // 언어 설정
        string language = LanguageService.apiLanguageParse().ToLower();

        // 상점번호 가져오기
        string helpNo = PlayerPrefs.GetString("helpNo");
        Debug.Log("helpNo : " + helpNo);
        // POST로 보낼 데이터 생성
        var postData = new
        {
            main = "",
            lang = language,
            ca2 = "",
            input = "",
            store_no = helpNo
        };
        Debug.LogError($"url : {url}\n post : {postData}");
        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 HelpDetailData 객체로 역직렬화
            var helpData = JsonConvert.DeserializeObject<HelpDetailData>(result);
            var help = helpData.response;

            Debug.LogError($"help.name : {help.name}\nhelp.address : {help.address}\nhelp.time : {help.time}");

            // 도와줘 사진 설정
            for (int i = 1; i <= images.Length; i++)
            {
                string fieldName = $"img_url_{i}";
                string imageUrl = (string)help.GetType().GetProperty(fieldName)?.GetValue(help, null);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    StartCoroutine(ImageLoader.LoadImages(imageUrl, images[i - 1], noImage));
                }
                else
                {
                    images[i - 1].sprite = noImage;
                }
            }

            // 도와줘 이름 및 카테고리 설정
            helpName.text = $"{help.name} <color=#FE6C50>•</color>  <size=24><color=#999999>{helpCategoryStr}</color></size>";

            // 도와줘 주소
            helpAddress.text = help.address;

            // 도와줘 시간
            string time = help.time.Equals("NaN") ? "-" : help.time;
            helpTime.text = time;

            // 도와줘 전화번호
            string telNo = help.tel_no.Equals("NaN") ? "-" : help.tel_no;
            helpTelno.text = telNo;

            // 도와줘 설명
            string description = help.intro.Equals("NaN") ? "-" : help.intro;
            helpDescription.text = description;

            // 도와줘 해시태그
            string hashtag = "";
            if (!string.IsNullOrEmpty(help.keyword))
            {
                string[] keywordArr = help.keyword.Split(", ");
                for (int i = 0; i < keywordArr.Length; i++)
                {
                    hashtag += $"#{keywordArr[i]} ";
                }
            }
            helpHashtag.text = hashtag;

            // 도와줘 별점
            scoreController.UpdateScore(help.naver_score);
            
            RawImage qrImage = GameObject.Find("helpQR").GetComponentInChildren<RawImage>();
            qrImage.texture = qr_preImage.texture;

            if (help.link != null)
            {
                // QR 뿌리기
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(help.address)))}&flag=help&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(help.name)))}&apn=com.witdiocianapp", GameObject.Find("helpQR").GetComponentInChildren<RawImage>());
                QRCreator.CreateQR($"{help.link}", GameObject.Find("helpQR").GetComponentInChildren<RawImage>());
            }
        }
        else
        {
            Debug.LogError("POST 요청 실패: " + response.StatusCode);
        }

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
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
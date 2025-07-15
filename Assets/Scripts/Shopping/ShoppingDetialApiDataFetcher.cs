using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShoppingDetailApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private Sprite noImage;

    [SerializeField]
    private Sprite qr_preImage;

    [SerializeField]
    private TextMeshProUGUI shoppingName;
    [SerializeField]
    private TextMeshProUGUI shoppingAddress;
    [SerializeField]
    private TextMeshProUGUI shoppingTime;
    [SerializeField]
    private TextMeshProUGUI shoppingTelno;
    [SerializeField]
    private TextMeshProUGUI shoppingDescription;
    [SerializeField]
    private TextMeshProUGUI shoppingHashtag;
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
        string shoppingCategoryStr = PlayerPrefs.GetString("shoppingCategory");

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // 언어 설정
        string language = LanguageService.apiLanguageParse();

        // 상점번호 가져오기
        string shoppingNo = PlayerPrefs.GetString("shoppingNo");
        
        // POST로 보낼 데이터 생성
        var postData = new
        {
            main = "",
            lang = language,
            ca2 = "",
            input = "",
            store_no = shoppingNo
        };
        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 ShoppingDetailData 객체로 역직렬화
            var shoppingData = JsonConvert.DeserializeObject<ShoppingDetailData>(result);
            var shopping = shoppingData.response;

            // 상점 사진 설정
            for (int i = 1; i <= images.Length; i++)
            {
                string fieldName = $"img_url_{i}";
                string imageUrl = (string)shopping.GetType().GetProperty(fieldName)?.GetValue(shopping, null);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    StartCoroutine(ImageLoader.LoadImages(imageUrl, images[i - 1], noImage));
                }
                else
                {
                    images[i - 1].sprite = noImage;
                }
            }

            // 상점 이름 및 카테고리 설정
            shoppingName.text = $"{shopping.name} <color=#FE6C50>•</color>  <size=24><color=#999999>{shoppingCategoryStr}</color></size>";

            // 상점 주소
            shoppingAddress.text = shopping.address;

            // 상점 시간
            string time = shopping.time.Equals("NaN") ? "-" : shopping.time;
            shoppingTime.text = time;

            // 상점 전화번호
            string telNo = shopping.tel_no.Equals("NaN") ? "-" : shopping.tel_no;
            shoppingTelno.text = telNo;

            // 상점 설명
            string description = shopping.intro.Equals("NaN") ? "-" : shopping.intro;
            shoppingDescription.text = description;

            // 상점 해시태그
            string hashtag = "";
            if (!string.IsNullOrEmpty(shopping.keyword))
            {
                string[] keywordArr = shopping.keyword.Split(", ");
                for (int i = 0; i < keywordArr.Length; i++)
                {
                    hashtag += $"#{keywordArr[i]} ";
                }
            }
            shoppingHashtag.text = hashtag;

            // 상점 별점
            scoreController.UpdateScore(shopping.naver_score);

            RawImage qrImage = GameObject.Find("ShoppingQR").GetComponentInChildren<RawImage>();
            qrImage.texture = qr_preImage.texture;

            if (shopping.link != null)
            {
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(shopping.address)))}&flag=shopping&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(shopping.name)))}&hashtag={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(hashtag)))}&apn=com.witdiocianapp", GameObject.Find("ShoppingQR").GetComponentInChildren<RawImage>());
                QRCreator.CreateQR($"{shopping.link}", GameObject.Find("ShoppingQR").GetComponentInChildren<RawImage>());
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
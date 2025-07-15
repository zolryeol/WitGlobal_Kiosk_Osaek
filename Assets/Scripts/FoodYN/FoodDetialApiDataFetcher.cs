using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class FoodDetailApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private Sprite noImage;

    [SerializeField]
    private Sprite qr_preImage;

    [SerializeField]
    private TextMeshProUGUI foodName;
    [SerializeField]
    private TextMeshProUGUI foodAddress;
    [SerializeField]
    private TextMeshProUGUI foodTime;
    [SerializeField]
    private TextMeshProUGUI foodTelno;
    [SerializeField]
    private TextMeshProUGUI foodDescription;
    [SerializeField]
    private TextMeshProUGUI foodHashtag;
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
        string foodCategoryStr = PlayerPrefs.GetString("foodCategory");

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // 언어 설정
        string language = LanguageService.apiLanguageParse().ToLower();

        // 상점번호 가져오기
        string foodNo = PlayerPrefs.GetString("foodNo");
        
        // POST로 보낼 데이터 생성
        var postData = new
        {
            main = "",
            lang = language,
            ca2 = "",
            input = "",
            store_no = foodNo
        };

        Debug.LogError($"post : {postData}");

        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            //Debug.Log(response.IsSuccessStatusCode + " 성공?");
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 FoodData 객체로 역직렬화
            var foodData = JsonConvert.DeserializeObject<FoodDetailData>(result);
            var food = foodData.response;

            //Debug.LogError($"Detail : {food}");

            // 음식점 사진 설정
            for (int i = 1; i <= images.Length; i++)
            {
                string fieldName = $"img_url_{i}";
                string imageUrl = (string)food.GetType().GetProperty(fieldName)?.GetValue(food, null);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    StartCoroutine(ImageLoader.LoadImages(imageUrl, images[i - 1], noImage));
                }
                else
                {
                    images[i - 1].sprite = noImage;
                }
            }

            // 음식점 이름 및 카테고리 설정
            foodName.text = $"{food.name} <color=#FE6C50>•</color>  <size=24><color=#999999>{foodCategoryStr}</color></size>";

            // 음식점 주소
            foodAddress.text = food.address;

            // 음식점 시간
            string time = food.time.Equals("Null") ? "-" : food.time;
            foodTime.text = time;

            // 음식점 전화번호
            string telNo = food.tel_no.Equals("Null") ? "-" : food.tel_no;
            foodTelno.text = telNo;

            // 음식점 설명
            string description = food.intro.Equals("Null") ? "-" : food.intro;
            foodDescription.text = description;

            // 음식점 해시태그
            string hashtag = "";
            if (!string.IsNullOrEmpty(food.keyword))
            {
                string[] keywordArr = food.keyword.Split(", ");
                for (int i = 0; i < keywordArr.Length; i++)
                {
                    hashtag += $"#{keywordArr[i]} ";
                }
            }
            foodHashtag.text = hashtag;

            // 음식점 별점
            scoreController.UpdateScore(food.naver_score);
            RawImage qrImage = GameObject.Find("FoodDetailQR").GetComponentInChildren<RawImage>();
            qrImage.texture = qr_preImage.texture;

            if (food.link != null)
            {
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(food.address)))}&flag=food&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(food.name)))}&hashtag={Uri.EscapeDataString(hashtag)}&apn=com.witdiocianapp", GameObject.Find("FoodDetailQR").GetComponentInChildren<RawImage>());
                QRCreator.CreateQR($"{food.link}", GameObject.Find("FoodDetailQR").GetComponentInChildren<RawImage>());
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
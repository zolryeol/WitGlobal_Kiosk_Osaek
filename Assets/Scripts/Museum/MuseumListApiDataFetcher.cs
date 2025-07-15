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
using UnityEngine.UI;

public class MuseumListApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private GameObject museumInfoPrefab;
    [SerializeField]
    private Sprite test;
    [SerializeField]
    private Sprite noImage;

    private HttpClient client;

    private void Awake()
    {
        client = new HttpClient();
    }

    async void Start()
    {
        // 언어 설정
        string language = LanguageService.apiLanguageParse();
        
        // 카테고리 및 검색어
        string museumCategoryStr = "";
        string museumSearchName = "";

        // 카테고리로 검색시에만 카테고리 설정
        if (PlayerPrefs.GetInt("isCategorySearch") == 1)
        {
            int museumCategory = PlayerPrefs.GetInt("museumCategory");
            museumCategoryStr = CategoryController.GetCategory(MenuName.Museum, museumCategory, language);
        }
        // 이름검색시 이름설정
        else
        {
            museumSearchName = PlayerPrefs.GetString("museumSearchName");
        }

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // POST로 보낼 데이터 생성, 카테고리검색인지 이름검색인지 분기처리
        object postData = null;

        // 카테고리 검색시
        if(PlayerPrefs.GetInt("isCategorySearch") == 1)
        {
            postData = new
            {
                main = "인사동 미술관",
                lang = language,
                ca2 = museumCategoryStr,
                input = "",
                store_no = ""
            };
        }
        // 이름 검색시
        else
        {
            postData = new
            {
                main = "인사동 미술관",
                lang = language,
                ca2 = "",
                input = museumSearchName.Remove(museumSearchName.Length - 1),
                store_no = ""
            };
        }
        Debug.LogError($"post : {postData}");
        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 MuseumData 객체로 역직렬화
            var museumData = JsonConvert.DeserializeObject<MuseumListData>(result);
            Debug.Log(museumData.response[1]);

            // 미술관 리스트 뿌려주기
            foreach (var museum in museumData.response)
            {
                //Debug.LogError($"museum.no : {museum.no}\nmuseum.store : {museum.store}\nmuseum.address : {museum.address}\nmuseum.time : {museum.time}");

                // 프리팹 인스턴스 생성
                GameObject instance = Instantiate(museumInfoPrefab, this.transform);

                // 미술관 번호 설정
                instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = museum.no;

                // 썸네일 설정
                string imageUrl = museum.img_url_1;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    StartCoroutine(ImageLoader.LoadImages(imageUrl, instance.transform.GetChild(1).GetComponent<Image>(), noImage));
                }
                else
                {
                    instance.transform.GetChild(1).GetComponent<Image>().sprite = noImage;
                }

                // 이름 및 카테고리 설정
                instance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{museum.store}  <color=#FE6C50>•</color>  <size=24><color=#999999>{museumCategoryStr}</color></size>";
               
                // 주소 및 시간 설정
                string time = museum.time.Equals("NaN") ? "-" : museum.time;
                instance.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = museum.address + $"\n{time}";
               
                // 해시태그 설정
                string[] keywordArr = museum.keyword.Split(", ");
                string hashtag = "";
                for (int i = 0; i < keywordArr.Length; i++)
                {
                    hashtag += $"#{keywordArr[i]} ";
                }
                instance.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = hashtag;
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
        LanguageService ls = new LanguageService();
        ls.museumSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("MuseumList");

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
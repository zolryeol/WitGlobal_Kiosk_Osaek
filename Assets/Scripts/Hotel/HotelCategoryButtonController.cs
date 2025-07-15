using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotelCategoryButtonController : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;            // 버튼 목록
    [SerializeField]
    private Sprite selectedImage;        // 버튼 선택 이미지
    [SerializeField]
    private Sprite normalImage;          // 버튼 해제 이미지
    [SerializeField]
    private Color selectedColor;         // 버튼 선택 텍스트컬러
    [SerializeField]
    private Color normalColor;           // 버튼 해제 텍스트컬러
    [SerializeField]
    private TMP_FontAsset selectedFont;           // 버튼 선택 폰트
    [SerializeField]
    private TMP_FontAsset normalFont;             // 버튼 해제 폰트

    [SerializeField]
    private GameObject hotelInfoPrefab;
    [SerializeField]
    private Sprite noImage;
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private Sprite qr_preImage;

    private HttpClient client;

    private void Awake()
    {
        client = new HttpClient();
    }

    private void Start()
    {
        // 각 버튼에 클릭 이벤트를 할당
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // 캡처한 인덱스
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // 첫번째 버튼 클릭돼야함
        OnButtonClick(0);

        // 번역
        languageChange();

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    // 버튼 클릭 시 이벤트
    private void OnButtonClick(int index)
    {
        new VideoController().OnChangeVideo("2", "lodgment");
        // 다른 모든 버튼 이벤트 초기화
        ResetButton();
        ChangeButton(index);
    }

    // 클릭된 버튼의 텍스트 및 스프라이트를 변경
    private void ChangeButton(int index)
    {
        TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = selectedColor;
        buttonText.font = selectedFont;
        buttons[index].GetComponent<Image>().sprite = selectedImage;

        // 클릭한 카테고리의 데이터 불러오기
        FetchHotelList(index);
    }

    // 버튼들의 텍스트 및 스프라이트 초기화
    private void ResetButton()
    {
        foreach (Button button in buttons)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.color = normalColor;
            buttonText.font = normalFont;
            button.GetComponent<Image>().sprite = normalImage;
        }
    }

    // API호출
    async private void FetchHotelList(int index)
    {
        // 기존 값 삭제
        foreach (Transform child in targetTransform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // 언어 설정
        string language = LanguageService.apiLanguageParse().ToLower();

        // 카테고리 값
        string hotelCategoryStr = CategoryController.GetCategory(MenuName.Hotel, index, language);
        PlayerPrefs.SetString("hotelCategory", hotelCategoryStr);

        string CategoryKo = CategoryController.GetCategory(MenuName.Hotel, index, "ko");
        PlayerPrefs.SetString("Category", CategoryKo);

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // POST로 보낼 데이터 생성
        var postData = new
        {
            //main = "도와줘",
            main = "인사동 숙박",
            lang = language,
            ca2 = hotelCategoryStr,
            input = ""
        };
        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 HotelListData 객체로 역직렬화
            var hotelData = JsonConvert.DeserializeObject<HotelListData>(result);
            
            // 도와줘 리스트 뿌려주기
            foreach (var hotel in hotelData.response)
            {
                // 프리팹 인스턴스 생성
                GameObject instance = Instantiate(hotelInfoPrefab, targetTransform);

                // 도와줘 번호 설정
                instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hotel.no;

                // 이름 및 카테고리 설정
                instance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{hotel.store}  <color=#FE6C50>•</color>  <size=24><color=#999999>{hotelCategoryStr}</color></size>";

                // 주소 및 시간 설정
                instance.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = hotel.address;

                RawImage qrImage = instance.transform.GetChild(5).GetComponentInChildren<RawImage>();
                qrImage.texture = qr_preImage.texture;

                if (hotel.link != null)
                {
                    // QR 뿌리기
                    //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(hotel.address)))}&flag=hotel&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(hotel.store)))}&apn=com.witdiocianapp", instance.transform.GetChild(5).GetComponentInChildren<RawImage>());
                    QRCreator.CreateQR($"{hotel.link}", instance.transform.GetChild(5).GetComponentInChildren<RawImage>());
                }

                // 호텔 해시태그
                string hashtag = "";
                if (!string.IsNullOrEmpty(hotel.keyword))
                {
                    Debug.Log(hotel.keyword);
                    string[] keywordArr = hotel.keyword.Split(", ");
                    for (int i = 0; i < keywordArr.Length; i++)
                    {
                        hashtag += $"#{keywordArr[i]} ";
                    }
                }
                
                instance.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = hashtag;

                // 썸네일 설정
                string imageUrl = hotel.img_url_1;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    StartCoroutine(ImageLoader.LoadImages(imageUrl, instance.transform.GetChild(1).GetComponent<Image>(), noImage));
                }
                else
                {
                    instance.transform.GetChild(1).GetComponent<Image>().sprite = noImage;
                }
            }
        }
    }

    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.hotelListSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("hotelList");

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

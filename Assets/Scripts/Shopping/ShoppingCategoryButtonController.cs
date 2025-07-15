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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShoppingCategoryButtonController : MonoBehaviour
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
    private GameObject shoppingInfoPrefab;
    [SerializeField]
    private Sprite noImage;
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private Sprite qr_preImage;

    private HttpClient client;
    bool videoFlag = false;

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
        FetchShoppingList(index);
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

        if (videoFlag) new VideoController().OnChangeVideo("2", "shopping");
        videoFlag = true;

        languageChange();
    }

    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.shoppingSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("Shopping");

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

    // API호출
    async private void FetchShoppingList(int index)
    {
        // 기존 값 삭제
        foreach (Transform child in targetTransform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // 언어 설정
        string language = LanguageService.apiLanguageParse();

        // 카테고리 값
        string shoppingCategoryStr = CategoryController.GetCategory(MenuName.Shopping, index, language);
        PlayerPrefs.SetString("shoppingCategory", shoppingCategoryStr);

        string CategoryKo = CategoryController.GetCategory(MenuName.Shopping, index, "ko");
        PlayerPrefs.SetString("Category", CategoryKo);

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.insa_api;

        // POST로 보낼 데이터 생성
        var postData = new
        {
            main = "인사 뭐사지",
            lang = language,
            ca2 = shoppingCategoryStr,
            input = "",
            store_no = ""
        };

        Debug.LogError($"post : {postData}");

        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 ShoppingListData 객체로 역직렬화
            var shoppingData = JsonConvert.DeserializeObject<ShoppingListData>(result);
            
            // 상점 리스트 뿌려주기
            foreach (var shopping in shoppingData.response)
            {
                //Debug.LogError($"shopping.no : {shopping.no}\nshopping.store : {shopping.store}\nshopping.address : {shopping.address}\n");
                // 프리팹 인스턴스 생성
                GameObject instance = Instantiate(shoppingInfoPrefab, targetTransform);

                // 식당 번호 설정
                instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = shopping.no;

                // 이름 및 카테고리 설정
                instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{shopping.store}  <color=#FE6C50>•</color>  <size=24><color=#999999>{shoppingCategoryStr}</color></size>";

                // 주소 설정
                instance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = shopping.address;

                // 시간 설정
                string time = shopping.time.Equals("NaN") ? "-" : shopping.time;
                instance.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = time;

                // 해시태그 설정
                string[] keywordArr = shopping.keyword.Split(", ");
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

                RawImage qrImage = instance.transform.GetChild(6).GetComponentInChildren<RawImage>();
                qrImage.texture = qr_preImage.texture;

                if (shopping.link != null)
                {
                    //QR코드 생성
                    //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(shopping.address)))}&flag=shopping&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(shopping.store)))}&hashtag={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(hashtag)))}&apn=com.witdiocianapp", instance.transform.GetChild(6).GetComponentInChildren<RawImage>());
                    QRCreator.CreateQR($"{shopping.link}", instance.transform.GetChild(6).GetComponentInChildren<RawImage>());
                }
            }
        }
    }
}

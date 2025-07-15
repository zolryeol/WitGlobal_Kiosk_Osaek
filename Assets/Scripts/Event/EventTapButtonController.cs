using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Net.Http;

public class EventTapButtonController : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;                     // 버튼 목록
    [SerializeField]
    private Sprite selectedImage;                 // 버튼 선택 이미지
    [SerializeField]
    private Sprite normalImage;                   // 버튼 해제 이미지
    [SerializeField]
    private Color selectedColor;                  // 버튼 선택 텍스트컬러
    [SerializeField]
    private Color normalColor;                    // 버튼 해제 텍스트컬러
    [SerializeField]
    private TMP_FontAsset selectedFont;           // 버튼 선택 폰트
    [SerializeField]
    private TMP_FontAsset normalFont;             // 버튼 해제 폰트

    [SerializeField]
    private GameObject eventInfoPrefab;           // 이벤트 리스트 프리팹
    [SerializeField]
    private Transform targetTransform;            // 이벤트 프리팹 위치할곳
    [SerializeField]
    private Sprite noImage;

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
        languageChange();
    }
    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.eventSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("event");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray)
        {
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
    private void Update()
    {
        // 터치가 하나 이상 발생했는지 확인
        // 터치가 시작될 때 (화면을 처음 터치했을 때)
        if (Input.touchCount > 0)
        {

            Debug.Log("터치했음!!!@@@");
            Touch touch = Input.GetTouch(0);            
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("터치했음!!!");

                new VideoController().OnChangeVideo("2", "event");
            }
        }
    }

    // 버튼 클릭 시 이벤트
    private void OnButtonClick(int index)
    {
        // 다른 모든 버튼 이벤트 초기화
        ResetButton();
        ChangeButton(index);
        UpdateContentAPI(index);
    }

    // 클릭된 버튼의 텍스트 폰트, 색상 및 스프라이트를 변경
    private void ChangeButton(int index)
    {
        TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = selectedColor;
        buttonText.font = selectedFont;
        buttons[index].GetComponent<Image>().sprite = selectedImage;

        Debug.LogError($"Button : {index} Click!!!!!");
    }

    // 버튼들의 텍스트 색상 및 스프라이트 초기화
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

    private IEnumerator ServerLoadImage(string url, Image image)
    {
        Debug.LogError("ServerLoadImage corutine 시작");
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("이미지 URL이 비어있습니다.");
            yield break;
        }

        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;

                // Texture2D를 Sprite로 변환
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                // UI Image에 Sprite 적용
                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("이미지 로드 실패: " + request.error);
            }
        }
    }

    async void UpdateContentAPI(int index)
    {
        // 기존 값 삭제
        foreach (Transform child in targetTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
               
        string category = "";

        switch (index)
        {
            case 0:
                category = "오늘의 행사";
                break;
            case 1:
                category = "이번주 행사";
                break;
            case 2:
                category = "예정 행사";
                break;
            case 3:
                category = "지난 행사";
                break;
            default:
                category = "오늘의 행사";
                break;
        }
        PlayerPrefs.SetString("eventCategory", category);


        // 언어 설정
        string language = LanguageService.apiLanguageParse();

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.search_event_api;

        // POST로 보낼 데이터 생성
        var postData = new
        {
            location = "insa",
            lang = language,
            index = index,
            detail = false
        };

        Debug.LogError($"url : {url}\n post : {postData}");

        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 FoodListData 객체로 역직렬화
            var eventData = JsonConvert.DeserializeObject<EventListData>(result);

            // 도와줘 리스트 뿌려주기
            foreach (var data in eventData.response)
            {

                //Debug.LogError($"event.id : {data.event_id} event.name : {data.event_name}" +
                //    $" event.address : {data.event_address} vent.date : {data.event_date}" +
                //    $" event.telno : {data.event_telno}");

                //프리팹 인스턴스 생성
                GameObject instance = Instantiate(eventInfoPrefab, targetTransform);

                // 이벤트 번호 설정
                instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{data.event_id}";

                // 이름 및 카테고리 설정
                instance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{data.event_name}  <color=#FE6C50>•</color>  <size=24><color=#999999>{category}</color></size>";

                // 주소 설정
                instance.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{data.event_address}";

                // 날짜 설정
                instance.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{data.event_date}";

                // 번호 설정
                instance.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{data.event_telno}";

                // 해시태그 설정
                instance.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"{data.event_hashtag}";

                // 사진 설정
                string filePath = $"{data.image_path}";
                //Debug.LogError($"filePath : {filePath}");

                RawImage qrImage = instance.transform.GetChild(7).GetComponentInChildren<RawImage>();
                qrImage.texture = qr_preImage.texture;

                if (data.link != null)
                //if (!string.IsNullOrWhiteSpace(data.link))
                {
                    // QR 뿌리기
                    // QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(address)))}&flag=event&restnm={Convert.ToBase64String(Encoding.UTF8.GetBytes(storeNm))}&apn=com.witdiocianapp", instance.transform.GetChild(7).GetComponentInChildren<RawImage>());
                    QRCreator.CreateQR($"{data.link}", instance.transform.GetChild(7).GetComponentInChildren<RawImage>());
                }

                // 사진이 존재할시 사진 설정
                Image image = instance.transform.GetChild(1).GetComponent<Image>();

                string serverImagePath = filePath;
                //Debug.LogError($"ServerURL : {serverImagePath}");
                StartCoroutine(ServerLoadImage(serverImagePath, image));

                //if (System.IO.File.Exists(filePath))
                //{
                //    // 파일을 읽고 Texture2D 생성
                //    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                //    Texture2D texture = new Texture2D(2, 2);
                //    texture.LoadImage(fileData); // 파일 데이터로 텍스처 생성

                //    // Texture2D를 Sprite로 변환
                //    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                //    // 이미지 설정
                //    image.sprite = sprite;
                //}
                //else
                //{
                //    image.sprite = noImage;
                //}

            }

        }
        else
        {
            Debug.LogError($"API Error");
        }
    }


}

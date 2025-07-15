using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventDetailApiDataFetcher : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite noImage;
    [SerializeField]
    private Sprite qr_preImage;

    [SerializeField]
    private TextMeshProUGUI eventName;
    [SerializeField]
    private TextMeshProUGUI eventAddress;
    [SerializeField]
    private TextMeshProUGUI eventDate;
    [SerializeField]
    private TextMeshProUGUI eventTelno;
    [SerializeField]
    private TextMeshProUGUI eventAge;
    [SerializeField]
    private TextMeshProUGUI eventFee;
    [SerializeField]
    private TextMeshProUGUI eventDescription;
    [SerializeField]
    private TextMeshProUGUI eventHashtag;

    private HttpClient client;
    private void Awake()
    {
        client = new HttpClient();
    }
       
    void Start()
    {
        languageChange();
        updateDetailAPI();
    }

    async void updateDetailAPI()
    {
        // 이벤트 id 및 카테고리 가져오기
        string eventId = PlayerPrefs.GetString("eventId");
        string eventCategory = PlayerPrefs.GetString("eventCategory");

        // 언어 설정
        string language = LanguageService.apiLanguageParse();

        // API URL
        string url = GlobalManager.Instance.domain + GlobalManager.Instance.search_event_api;

        // POST로 보낼 데이터 생성
        var postData = new
        {
            location = "",
            lang = language,
            index = eventId,
            detail = true
        };

        Debug.LogError($"post : {postData}");

        string jsonData = JsonConvert.SerializeObject(postData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // API 호출
        HttpResponseMessage response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();

            // JSON 데이터를 FoodListData 객체로 역직렬화
            var eventData = JsonConvert.DeserializeObject<EventDetailData>(result);
            var data = eventData.response;

            //이름 및 카테고리 설정
            eventName.text = $"{data.event_name}  <color=#FE6C50>•</color>  <size=24><color=#999999>{eventCategory}</color></size>";

            // 주소 설정
            eventAddress.text = $"{data.event_address}";

            // 날짜 설정
            eventDate.text = $"{data.event_date}";

            // 전화번호 설정
            eventTelno.text = $"{data.event_telno}";

            // 연령제한 설정
            eventAge.text = $"{data.event_age}";

            // 입장료 설정
            eventFee.text = $"{data.event_fee}";

            // 해시태그 설정
            eventHashtag.text = $"{data.event_hashtag}";

            // 설명 설정
            eventDescription.text = $"{data.event_description}";

            // 사진 설정
            string filePath = $"{data.image_path}"; 


            RawImage qrImage = GameObject.Find("EventQR").GetComponentInChildren<RawImage>();
            qrImage.texture = qr_preImage.texture;

            if(data.link != null)
            {
                // QR 뿌리기
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(row["event_address"].ToString())))}&flag=event&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(row["event_name"].ToString())))}&hashtag={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(row["event_hashtag"].ToString())))}&apn=com.witdiocianapp", GameObject.Find("EventQR").GetComponentInChildren<RawImage>());
                QRCreator.CreateQR($"{data.link}", GameObject.Find("EventQR").GetComponentInChildren<RawImage>());
            }


            // 사진이 존재할시 사진 설정
            string serverImagePath = filePath;
            //Debug.LogError($"ServerURL : {serverImagePath}");
            StartCoroutine(ServerLoadImage(serverImagePath, image));

            //if (System.IO.File.Exists(filePath))
            //{

            //    Debug.LogError($"File Exits : {filePath}");
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
            //    Debug.LogError($"File is Not Exits : {filePath}");
            //    image.sprite = noImage;
            //}

        }
        else
        {
            Debug.LogError($"API Error");
        }
    }
       

    private IEnumerator ServerLoadImage(string url, Image image)
    {
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
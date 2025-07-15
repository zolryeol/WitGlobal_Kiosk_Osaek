using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UpdatePhoto;

public class UpdatePhoto : MonoBehaviour
{
    // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 표占시듸옙 占쏙옙치占쏙옙
    [SerializeField]
    private Image[] images;

    // QR표시용 이미지
    [SerializeField]
    private RawImage qrRawImage;
    // QR표시용 이미지(대형)
    [SerializeField]
    private RawImage qrFocusRawImage;

    // @시연 이미지(합성사진)
    [SerializeField]
    private Sprite[] testPhoto;

    [System.Serializable]
    public class KioskConfig
    {
        public string kioskName;
    }


    private void Start()
    {

        // 사진 정보 찾아오고 씬에 갱신 및 QR생성
        // @시연하기위해 일단 주석
        // StartCoroutine(UpdatePhotoInfo());
        // TestUpdatePhoto();

        // KJH Server 로 이미지 파일 전송
        StartCoroutine(UploadFileCoroutine());

        TestUpdatePhoto2();

        languageChange();
        
    }

    private IEnumerator UploadFileCoroutine()
    {
        // 실행 파일과 동일한 경로에서 config.json 파일 읽기
        string configPath = Path.Combine(Application.dataPath, "../config.json");

        if (!File.Exists(configPath))
        {
            Debug.LogError($"Config file not found at: {configPath}");
            yield break;
        }

        // JSON 파일 읽기
        string jsonContent = File.ReadAllText(configPath);
        KioskConfig config = JsonUtility.FromJson<KioskConfig>(jsonContent);

        // 키오스크 이름 가져오기
        string kioskName = config.kioskName;

        Debug.LogError($"TEST KioskName : {kioskName}");

        string filePath = PlayerPrefs.GetString("arFilePath");
        //filePath = "D:\\JH_Project\\insa_kiosk\\kiosk_image\\20241106_141024.png";
        //Debug.Log($"filePath : {filePath}");

        string fileName = Path.GetFileName(filePath);

        while (!File.Exists(filePath))
        {
            yield return new WaitForSeconds(0.1f);
        }

        string uploadURL = GlobalManager.Instance.domain + GlobalManager.Instance.upload_api;

        byte[] fileData = System.IO.File.ReadAllBytes(filePath); // 파일을 byte 배열로 읽음
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName, "application/octet-stream");


        //JSON에서 가져온 키오스크 이름 추가
        form.AddField("kiosk_name", kioskName);

        UnityWebRequest www = UnityWebRequest.Post(uploadURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("File uploaded successfully");

            // 서버에서 반환된 JSON 응답을 파싱하여 download_url을 추출
            string responseText = www.downloadHandler.text;
            var jsonResponse = JsonUtility.FromJson<UploadResponse>(responseText);

            // download_url을 받아서 QR 코드 생성
            string downloadUrl = jsonResponse.download_url;
            string qrUrl = GlobalManager.Instance.domain + downloadUrl;

            Debug.LogError($"QR URL : {qrUrl}");

            // QR 코드 생성
            QRCreator.CreateQR(qrUrl, qrRawImage);
            QRCreator.CreateQR(qrUrl, qrFocusRawImage);

        }
        else
        {
            Debug.LogError("File upload failed: " + www.error);
        }





    }

    // 서버에서 반환하는 JSON 응답을 매핑할 클래스
    [System.Serializable]
    public class UploadResponse
    {
        public string message;
        public string file_path;
        public string download_url; // 서버에서 반환하는 download_url
    }



    //private IEnumerator nodeServerImgSave(byte[] imageBytes, string path)
    //{        
    //    WWWForm form = new WWWForm();
    //    form.AddBinaryData("image", imageBytes, path.Substring(path.LastIndexOf("/")+1), "image/png");

    //    UnityWebRequest www = UnityWebRequest.Post("http://wit.inno-t.shop/api/kiosk/ImageSave", form);
    //    yield return www.SendWebRequest(); // 占쌘뤄옙틴占쏙옙占쏙옙 호占쏙옙
        
    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        Debug.Log("Image successfully uploaded");
    //    }
    //}



    // @시연용 이미지 띄우기
    private void TestUpdatePhoto()
    {
        // � �Ѻ��� ���õƴ��� ��������
        int testHanbokIndex = PlayerPrefs.GetInt("testHanbokIndex");

        // �̹��� ����

        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = testPhoto[testHanbokIndex];
        }


        // 선택된 이미지에 따른 QR이미지 설정

        string qrUrl = "";
        string encodedFileName = "";
        if (testHanbokIndex == 0)
        { // 임근님
            encodedFileName = Uri.EscapeDataString("photo1.png");
            
        }
        else if (testHanbokIndex == 1)
        { // 선비옷
            encodedFileName = Uri.EscapeDataString("photo2.png");            
        }
        else
        { // 준비중 스프라이트 띄우기

        }
        //qrUrl = "http://wit.inno-t.shop/api/kiosk/download/" + encodedFileName;
        //QRCreator.CreateQR(qrUrl, qrRawImage);
        //QRCreator.CreateQR(qrUrl, qrFocusRawImage);        

    }

    private void TestUpdatePhoto2()
    {
        string filePath = PlayerPrefs.GetString("arFilePath");
        //filePath = "D:\\JH_Project\\insa_kiosk\\kiosk_image\\20241106_141024.png";
        // 파일이 존재하는지 확인
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found at " + filePath);
        }

        // 이미지 파일을 바이트 배열로 읽기
        byte[] fileData = File.ReadAllBytes(filePath);

        // 바이트 배열을 이용하여 Texture2D 생성
        Texture2D texture = new Texture2D(2, 2); // 기본 크기 설정
        texture.LoadImage(fileData); // 이미지 로드

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = sprite;
        }

        //images[0].sprite = sprite;

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = null;
        }
        SceneManager.LoadScene("main");
    }


    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.aRResultSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("aRResult");

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
}

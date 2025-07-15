using System.Collections;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using TMPro.Examples;
using UnityEngine.Networking;
using static ARController;
using System.Text;
using System.Diagnostics;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;


public class ARController : MonoBehaviour
{
    // 2024.10.22 KJH ADD Elgato FaceCam Pro
    private WebCamDevice faceCamPro;
    private WebCamTexture faceCamProTexture;
    private Texture2D croppedTexture;

    [SerializeField]
    private UnityEngine.UI.RawImage rawImage;

    [SerializeField]
    private GameObject guideImage;

    [SerializeField]
    private GameObject photoText;

    // 카운트다운 이미지
    [SerializeField]
    private GameObject countDown;
    [SerializeField]
    private Sprite[] countDownSprites;

    // 캡처영역 좌표
    [SerializeField]
    private float startX;
    [SerializeField] 
    private float startY;
    [SerializeField] 
    private float endX;
    [SerializeField] 
    private float endY;

    // 파일 저장 경로
    [SerializeField]
    private string folderPath;

    // 카메라 화살표
    [SerializeField]
    private GameObject cameraFocus;

    // 10초 로딩
    [SerializeField]
    private GameObject loadingProgress;
    [SerializeField]
    private TextMeshProUGUI loadingCountText;


    //[SerializeField]
    //private string lastImagePath;

    public class JsonData
    {
        public string base64_image;
        public int option;
    }

    private void Start()
    {
        // 2024.10.22
        // KJH ADD

        // 이미지 저장 경로 초기화
        PlayerPrefs.SetString("arFilePath", "");

        // initElgato
        initElgato();

        guideImage.SetActive(true);

        // 사진찍기 시작
        StartCoroutine("ShootCamera");
    }

    // Elgato 초기화
    private void initElgato()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            faceCamPro = WebCamTexture.devices[0];
            UnityEngine.Debug.Log("Selected Camera: " + faceCamPro.name);

            faceCamProTexture = new WebCamTexture(faceCamPro.name, 2160, 3840);
            
            faceCamProTexture.Play();
            rawImage.uvRect = new Rect(0, 1, 1, -1);

            UnityEngine.Debug.Log("Camera is Playing: " + faceCamProTexture.isPlaying);

            //Debug.Log("Requested Resolution: 2160x3840");
            //Debug.Log("Actual Resolution: " + faceCamProTexture.width + "x" + faceCamProTexture.height);
        }
    
        else
        {
            UnityEngine.Debug.LogError("No WebCam Devices found.");
        }
    }
   
    private void Update()
    {
        try
        {    
            if (faceCamProTexture != null && faceCamProTexture.isPlaying)                
            {
                //Debug.Log("Camera texture is not null and Camera is playing.");
                rawImage.texture = faceCamProTexture;
                
            }
            else
            {
                //Debug.LogWarning("Camera texture is not playing.");
            }
        }
        catch (Exception e)
        {
            SceneManager.LoadSceneAsync("main");
        }
    }

    // Kinect 정지
    private void OnDestroy()
    {        
        //if(faceCamProTexture != null && faceCamProTexture.isPlaying)
        {
            faceCamProTexture.Stop();
            faceCamProTexture = null;
            UnityEngine.Debug.LogWarning("Camera texture is stop.");
        }
        
    }

    // 사진 찍기
    private IEnumerator ShootCamera()
    {
        // 5초 세기
        yield return new WaitForSeconds(2.0f);

        // 카운트다운
        int count = 10;
        while (count > 0)
        {
            // 맨 앞으로 설정
            countDown.transform.SetAsLastSibling();
            // 카운트다운 아이콘 출력
            countDown.GetComponent<UnityEngine.UI.Image>().sprite = countDownSprites[count-1];

            // 1초 기다리기
            yield return new WaitForSeconds(1.0f);
            count--;
        }

        

        /*// @시연용 일단 사진저장은 빼기
        // 사진 로컬에 저장 하기
        Camera.main.targetDisplay = 1;
        Camera.main.Render();
        yield return new WaitForEndOfFrame();

        int photoWidth = (int)endX - (int)startX;
        int photoHeight = (int)endY - (int)startY;

        Rect pixeslRect = new Rect(startX, startY, photoWidth, photoHeight);
        Texture2D texture2D = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);

        texture2D.ReadPixels(pixeslRect, 0, 0);
        texture2D.Apply();

        byte[] byteArray = texture2D.EncodeToPNG();
        string filePath = $"{folderPath}/{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";
        File.WriteAllBytes(filePath, byteArray);
        Debug.LogError($"FilePath : " + filePath)*/
        ;


        int cropX = 0;
        int cropWidth = faceCamProTexture.width - 2 * cropX;

        // 새로운 텍스처 생성 (잘라낸 크기)
        croppedTexture = new Texture2D(cropWidth, faceCamProTexture.height);

        // 웹캠 텍스처에서 잘라낸 부분을 복사
        Color[] pixels = faceCamProTexture.GetPixels(cropX, 0, cropWidth, faceCamProTexture.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        // 리사이즈
        //Texture2D resizedTexture = ResizeTexture(croppedTexture, 1218, 1814);

        // 파일로 저장
        byte[] bytes = croppedTexture.EncodeToPNG();
        string filePath = $"{folderPath}/{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";
        File.WriteAllBytes(filePath, bytes);
        //Debug.LogError("Captured image saved to: " + filePath);

        //====================================================================//
        // 합성
        // filePath, 옷 옵션 보내기 -> return 이미지 경로 다운로드 
        // 선택된 한복 index
        int testHanbokIndex = PlayerPrefs.GetInt("testHanbokIndex");

        StartCoroutine(PostRequest(filePath, testHanbokIndex));

        //====================================================================// 

        // canvas씬 화살표 지우기
        cameraFocus.SetActive(false);

        // @인사영상 띄워놓고 10초세기
        VideoController videoController = GameObject.FindObjectOfType<VideoController>();
        if (videoController != null)
        {
            videoController.OnChangeVideo("5", "insaAr");
        }

        yield return new WaitForSeconds(0.3f);

        loadingProgress.SetActive(true);
        rawImage.gameObject.SetActive(false);
        guideImage.SetActive(false);
        photoText.SetActive(false);
        countDown.SetActive(false);

        int loadingCount = 60;
        while(loadingCount > 0)
        {
            loadingCountText.text = loadingCount.ToString();
            yield return new WaitForSeconds(0.95f);
            loadingCount--;
        }

        // 씬 이동
        // @시연용 일단 사진저장 빼놓기
        //PlayerPrefs.SetString("arFilePath", filePath);
        SceneManager.LoadScene("ArResult");
    }

    private IEnumerator PostRequest(string path, int index)
    {

        string post_url = GlobalManager.Instance.post_api;
        // 파일이 존재하는지 확인
        //path = "D:\\JH_Project\\insa_kiosk\\kiosk_image\\index_6.png";
        //path = lastImagePath;

        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError("File not found at " + path);
            yield break;
        }
        else
        {
            UnityEngine.Debug.LogError("File exit : " + path);
        }

        byte[] fileData = File.ReadAllBytes(path);


        WWWForm form = new WWWForm();
        form.AddBinaryData("image", fileData, Path.GetFileName(path), "image/jpeg");
        form.AddField("option", index);

        UnityWebRequest www = UnityWebRequest.Post(post_url, form);

        //www.redirectLimit = 0;

        yield return www.SendWebRequest();


        if (www.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.LogError("Error: " + www.error);
            UnityEngine.Debug.LogError("Response Code: " + www.responseCode);
            UnityEngine.Debug.LogError("URL THIRD: " + www.url);
        }
        else
        {
            UnityEngine.Debug.Log("Image successfully uploaded");
            byte[] receivedData = www.downloadHandler.data;

            Texture2D texture = new Texture2D(2, 2); // 임시 크기의 Texture2D 생성
            texture.LoadImage(receivedData);
            //texture.LoadImage(fileData);

            Texture2D waterMarkTexture = Resources.Load<Texture2D>("Image/AR/watermark");
            Texture2D resizeTexture = ResizeTexture(waterMarkTexture, 400, 400);

            Vector2 pixel = new Vector2(20, 10);

            Texture2D changeTexture = AddWatermark(texture, resizeTexture, pixel, 1.0f);
            // 리사이즈
            //Texture2D resizedTexture = ResizeTexture(texture, 1218, 1814);

            byte[] imageData = changeTexture.EncodeToPNG();

            // 파일로 저장
            string receiveFilePath = $"{folderPath}/{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";
            File.WriteAllBytes(receiveFilePath, imageData);
            UnityEngine.Debug.Log("receive Image Save Done.");

            PlayerPrefs.SetString("arFilePath", receiveFilePath);
        }

    }


    // 리사이즈된 이미지 생성 함수
    public Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        // RenderTexture 설정 (32비트 포맷을 지원하는 RenderTexture 사용)
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture.active = rt;

        // 원본 텍스처를 RenderTexture에 그리기
        Graphics.Blit(source, rt);

        // 새로운 Texture2D 생성, RGBA32 포맷을 사용하여 32비트 정보 유지
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        // RenderTexture 해제 및 메모리 정리
        RenderTexture.active = null;
        rt.Release();

        return result;
    }



    public Texture2D AddWatermark(Texture2D baseTexture, Texture2D watermarkTexture, Vector2 pixelVal, float alpha = 1.0f)
    {
        // 워터마크 위치를 오른쪽 하단으로 설정
        Vector2 position = new Vector2(
            baseTexture.width - watermarkTexture.width - pixelVal.x,
            pixelVal.y
        );

        // 새로운 Texture2D 생성
        Texture2D resultTexture = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);

        // 원본 텍스처 복사
        resultTexture.SetPixels(baseTexture.GetPixels());

        // 워터마크 합성
        for (int x = 0; x < watermarkTexture.width; x++)
        {
            for (int y = 0; y < watermarkTexture.height; y++)
            {
                // 워터마크의 현재 픽셀 위치
                int targetX = (int)position.x + x;
                int targetY = (int)position.y + y;

                // 텍스처 영역 벗어나면 스킵
                if (targetX >= baseTexture.width || targetY >= baseTexture.height)
                    continue;

                // 워터마크와 원본 픽셀 색상 가져오기
                Color baseColor = resultTexture.GetPixel(targetX, targetY);
                Color watermarkColor = watermarkTexture.GetPixel(x, y);

                // 알파값을 적용한 블렌딩
                float finalAlpha = watermarkColor.a * alpha; // 워터마크의 알파에 사용자 알파 값을 곱함
                Color blendedColor = Color.Lerp(baseColor, watermarkColor, finalAlpha);

                resultTexture.SetPixel(targetX, targetY, blendedColor);

            }
        }

        // 텍스처 적용
        resultTexture.Apply();

        return resultTexture;
    }

}

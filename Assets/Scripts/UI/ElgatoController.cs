using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// 디바이스 제어

public class ElgatoController : MonoBehaviour
{
    private WebCamTexture faceCamTexture;

    [SerializeField] RawImage display2RawImage; // Display 2에 출력될 RawImage
    [SerializeField] Image countDownImage;
    //[SerializeField] TextMeshProUGUI countDownText; // 이미지로 사용하여 텍스트X

    [SerializeField] TextMeshProUGUI countDownText_AD; // 광고용 1분 카운트다운.
    public GameObject adCountParent;
    public string LatestResultImagePath { get; set; }

    public int hanbokIndex { get; set; } = 1;
    public bool IsSuccessed { get; set; } = false;
    public bool IsElgatoRunning { get; private set; } = false;

    public bool isTogether = false;

    Page_Photo page_photo;

    PhotoResultToQR resultToQR;

    Coroutine runningCoroutine;
    Coroutine runningCoroutineAD;

    private uint shotSerial = 0;
    private uint currentShotSerial = 0;

    private void Awake()
    {
        page_photo = GetComponent<Page_Photo>();
        resultToQR = FindAnyObjectByType<PhotoResultToQR>(FindObjectsInactive.Include);
    }
    public void StartElgato()
    {
        StopAD();

        if (runningCoroutine != null)
        {
            StopAllCoroutines();
            runningCoroutine = null;
        }

        shotSerial++;
        currentShotSerial = shotSerial;

        IsElgatoRunning = true;

        LatestResultImagePath = "";
        IsSuccessed = false;
        runningCoroutine = StartCoroutine(StartElgatoCoroutine(currentShotSerial));
    }

    public void StopElgato()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            StopAD();
            runningCoroutine = null;
        }

        if (faceCamTexture != null && faceCamTexture.isPlaying)
        {
            faceCamTexture.Stop();
            faceCamTexture = null;
        }

        display2RawImage.texture = null;
        display2RawImage.gameObject.SetActive(false);

        IsElgatoRunning = false;
    }
    public void StopAD()
    {
        if (runningCoroutineAD != null)
        {
            StopCoroutine(runningCoroutineAD);
            runningCoroutineAD = null;
        }
        adCountParent.SetActive(false);
    }
    IEnumerator StartElgatoCoroutine(uint _serial)
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        display2RawImage.texture = null;

        if (devices.Length == 0)
        {
            KioskLogger.Error("No webcam found");
            page_photo.InitPage();
            yield break;
        }

        string camName = devices[0].name;
        faceCamTexture = new WebCamTexture(camName, 2160, 3840, 30);
        faceCamTexture.Play();

        display2RawImage.texture = faceCamTexture;
        display2RawImage.uvRect = new Rect(0, 1, 1, -1);

        float timeout = 5f;
        float elapsed = 0f;
        while (!faceCamTexture.isPlaying && elapsed < timeout)
        {
            Debug.Log("Waiting for camera to start...");
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!faceCamTexture.isPlaying)
        {
            KioskLogger.Error("Camera failed to start.");
            page_photo.InitPage();

            yield break;
        }

        // ✅ 카운트다운
        int count = 10;
        while (0 < count)
        {
            //if (countDownText != null)
            //    countDownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
            countDownImage.sprite = ResourceManager.Instance.PhotoCountDownImage[count];
        }
        yield return new WaitForSeconds(1f);

        //if (countDownText != null) countDownText.text = "";
        display2RawImage.gameObject.SetActive(false); // 
        runningCoroutineAD = StartCoroutine(ADCountDown(_serial)); // 광고 타이머 60초



        int cropMargin = Mathf.FloorToInt(faceCamTexture.width * 0.02f); // 
        int cropX = cropMargin;
        int cropWidth = faceCamTexture.width - 2 * cropMargin;

        // 캡처 및 저장
        Texture2D croppedTexture = new Texture2D(cropWidth, faceCamTexture.height);
        Color[] pixels = faceCamTexture.GetPixels(cropX, 0, cropWidth, faceCamTexture.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        // ✅ 저장 폴더 설정
        string folderPath = Path.Combine(Application.dataPath, "../CapturedImages");
        Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, $"Captured_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        File.WriteAllBytes(filePath, croppedTexture.EncodeToPNG());

        Debug.Log("📸 잘린 이미지 저장 완료: " + filePath);

        Debug.Log($"한복 인덱스 = {hanbokIndex}");

        yield return StartCoroutine(PostImageToServer(filePath, hanbokIndex, folderPath, _serial));

        faceCamTexture.Stop();
        faceCamTexture = null;
    }

    private void OnDestroy()
    {
        if (faceCamTexture != null && faceCamTexture.isPlaying)
        {
            faceCamTexture.Stop();
        }
    }

    /////////////////////
    public void StartElgatoDirect()
    {

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            KioskLogger.Error("No webcam found");
            page_photo.InitPage();
            return;
        }

        string camName = devices[0].name;
        Debug.Log("Selected Camera: " + camName);

        faceCamTexture = new WebCamTexture(camName, 2880, 1620, 30);
        faceCamTexture.Play();

        display2RawImage.texture = faceCamTexture;
        display2RawImage.uvRect = new Rect(0, 1, 1, -1);

        Debug.Log($"Started WebCamTexture: {faceCamTexture.width}x{faceCamTexture.height}");
    }

    // 서버 송수신
    IEnumerator PostImageToServer(string imagePath, int optionIndex, string saveFolder, uint _serial)
    {
        if (!System.IO.File.Exists(imagePath))
        {
            KioskLogger.Error("File not found: " + imagePath);
            yield break;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(imagePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", fileData, Path.GetFileName(imagePath), "image/png");
        form.AddField("option", optionIndex);

        string url;
        if (isTogether == true)
            url = Core.PhotoPostUrl_together;
        else
            url = Core.PhotoPostUrl;

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) // 실패하는 경우
        {
            UnityWebRequest www2 = UnityWebRequest.Post(url, form); // 2차 시도
            yield return www2.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) // 2차 시도 실패하면 처음으로
            {
                KioskLogger.Error("Upload failed: " + www.error);
                page_photo.InitPage(); // 실패했을때 초기화면으로
                yield break;
            }
            else
            {
                Debug.Log("✅ 서버 응답 수신 완료 2차");
            }
        }

        Debug.Log("✅ 서버 응답 수신 완료");


        // ✅ 응답 이미지 저장
        byte[] receivedData = www.downloadHandler.data;
        Texture2D responseTexture = new Texture2D(2, 2);
        responseTexture.LoadImage(receivedData);

        // 워터마크 삽입 (선택 사항)
        Texture2D watermark = Resources.Load<Texture2D>("Image/AR/WITHWaterMark");
        Texture2D resizedWatermark = ResizeTexture(watermark, 400, 200);
        Texture2D finalTexture = AddWatermark(responseTexture, resizedWatermark, new Vector2(20, 10), 1.0f);

        //string resultPath = Path.Combine(saveFolder, $"Result_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        string resultPath = Path.Combine(saveFolder, Path.GetFileNameWithoutExtension(imagePath) + "_AI.png");

        LatestResultImagePath = resultPath;
        File.WriteAllBytes(resultPath, finalTexture.EncodeToPNG());
        Debug.Log("📥 응답 이미지 저장 완료: " + resultPath);

        if (_serial == currentShotSerial)
        {
            LatestResultImagePath = resultPath;        // ★ 시리얼 확인 후에만 기록
            File.WriteAllBytes(resultPath, finalTexture.EncodeToPNG());
            Debug.Log("📥 응답 이미지 저장 완료: " + resultPath);

            StartCoroutine(resultToQR.FetchImageFile(LatestResultImagePath));
        }
        else
        {
            KioskLogger.Warn("📸 PostImageToServer - Serial mismatch, ignoring result.");
            page_photo.InitPage();
            yield break;
        }

        IsSuccessed = true;

    }

    /// 워터마크 추가
    public Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        return result;
    }

    // 워터마크 추가
    public Texture2D AddWatermark(Texture2D baseTex, Texture2D watermarkTex, Vector2 offset, float alpha = 1.0f)
    {
        Texture2D result = new Texture2D(baseTex.width, baseTex.height, baseTex.format, false);
        result.SetPixels(baseTex.GetPixels());

        for (int x = 0; x < watermarkTex.width; x++)
        {
            for (int y = 0; y < watermarkTex.height; y++)
            {
                int tx = (int)(baseTex.width - watermarkTex.width - offset.x + x);
                int ty = (int)(offset.y + y);
                if (tx >= baseTex.width || ty >= baseTex.height) continue;

                Color baseColor = result.GetPixel(tx, ty);
                Color wmColor = watermarkTex.GetPixel(x, y);
                Color blended = Color.Lerp(baseColor, wmColor, wmColor.a * alpha);
                result.SetPixel(tx, ty, blended);
            }
        }

        result.Apply();
        return result;
    }

    IEnumerator ADCountDown(uint _serial)
    {
        VideoPlayManager.Instance.PlayVideo(VideoType.Photo_Creating);

        var wfs = new WaitForSeconds(1);

        adCountParent.SetActive(true);

        int count = 60; // 광고시간

        while (count > 0)
        {
            if (!IsElgatoRunning) yield break; // 주간 취소방지

            if (countDownText_AD != null)
                countDownText_AD.text = count.ToString();

            yield return wfs;

            count--;
        }

        countDownText_AD.text = "";
        adCountParent.SetActive(false);

        if (_serial == currentShotSerial && IsSuccessed && !string.IsNullOrEmpty(LatestResultImagePath) && File.Exists(LatestResultImagePath))
        {
            page_photo.Final();
            yield break;
        }
        else
        {
            KioskLogger.Error("이미지 처리 실패 또는 결과 없음");
        }

        StartCoroutine(WaitForResultAndCallFinal(_serial));
    }

    IEnumerator WaitForResultAndCallFinal(uint _serial)
    {
        float timeout = 60f;
        float elapsed = 0f;

        while ((string.IsNullOrEmpty(LatestResultImagePath) || !File.Exists(LatestResultImagePath)) && elapsed < timeout)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        if (_serial == currentShotSerial && !string.IsNullOrEmpty(LatestResultImagePath) && File.Exists(LatestResultImagePath))
        {
            page_photo.Final();
        }
        else
        {
            KioskLogger.Error("결과 이미지가 준비되지 않음 (타임아웃)");
            page_photo.InitPage();
        }
    }
}

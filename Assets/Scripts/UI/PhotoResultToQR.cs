using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PhotoUploader : MonoBehaviour
{
    [Header("업로드된 사진 미리보기용 UI")]
    [SerializeField] private Image[] displayImages;

    [Header("QR 코드 표시용 UI")]
    [SerializeField] private RawImage qrSmallImage;
    [SerializeField] private RawImage qrLargeImage;

    private string filePath;
    private const string uploadURL = "http://134.185.113.244/upload";

    [Serializable]
    public class UploadResponse
    {
        public string message;
        public string file_path;
        public string download_url;
    }

    private void Start()
    {
        filePath = PlayerPrefs.GetString("arFilePath");

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogError("이미지 파일 경로가 유효하지 않거나 존재하지 않습니다.");
            return;
        }

        DisplayImageFromFile(filePath);
        StartCoroutine(UploadAndGenerateQR(filePath));
    }

    private void DisplayImageFromFile(string path)
    {
        try
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            foreach (var img in displayImages)
                img.sprite = sprite;
        }
        catch (Exception e)
        {
            Debug.LogError($"이미지 표시 실패: {e.Message}");
        }
    }

    private IEnumerator UploadAndGenerateQR(string filePath)
    {
        byte[] imageBytes = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, fileName, "application/octet-stream");
        form.AddField("kiosk_name", "KIOSK_A"); // 하드코딩된 키오스크 이름 (필요시 수정 가능)

        using UnityWebRequest request = UnityWebRequest.Post(uploadURL, form);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("파일 업로드 실패: " + request.error);
            yield break;
        }

        UploadResponse response = JsonUtility.FromJson<UploadResponse>(request.downloadHandler.text);
        if (string.IsNullOrEmpty(response.download_url))
        {
            Debug.LogError("서버 응답에 download_url이 없습니다.");
            yield break;
        }

        string fullDownloadURL = "http://134.185.113.244" + response.download_url;
        ApplyQRToUI(fullDownloadURL);
    }

    private void ApplyQRToUI(string url)
    {
        if (qrSmallImage != null)
            QRCreator.CreateQR(url, qrSmallImage);

        if (qrLargeImage != null)
            QRCreator.CreateQR(url, qrLargeImage);

        Debug.Log($"QR URL 생성 완료: {url}");
    }
}

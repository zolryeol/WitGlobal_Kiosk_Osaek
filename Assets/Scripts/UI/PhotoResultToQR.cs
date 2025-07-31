using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PhotoResultToQR : MonoBehaviour
{
    [Header("업로드된 사진 미리보기용 UI")]
    [SerializeField] private Image[] displayImages;

    [Header("QR 코드 표시용 UI")]
    [SerializeField] private RawImage qrSmallImage;
    [SerializeField] private RawImage qrLargeImage;

    private const string uploadURL = "http://134.185.113.244/upload";

    ElgatoController elgatoController;

    [Serializable]
    public class UploadResponse
    {
        public string message;
        public string file_path;
        public string download_url;
    }

    public IEnumerator FetchImageFile(string filePath)
    {
        //var filePath = elgatoController.LatestResultImagePath;

        byte[] imageBytes = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, fileName, "application/octet-stream");
        form.AddField("kiosk_name", "KIOSK_LEE_COM"); // 하드코딩된 키오스크 이름 (필요시 수정 가능)

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
            qrSmallImage.texture = CommonFunction.GenerateQRCode(url);

        if (qrLargeImage != null)
            qrLargeImage.texture = CommonFunction.GenerateQRCode(url);

        Debug.Log($"QR URL 생성 완료: {url}");
    }
}

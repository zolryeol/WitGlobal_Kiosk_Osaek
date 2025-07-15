using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PrintAPIController : MonoBehaviour
{

    public Button targetButton;

    // kiosk id Test url
    //private string kiosk_url = $"https://go.selpic.co.kr/skapi/kiosk/A1800112";
    // 카페 옆자리 
    private string kiosk_url = $"https://go.selpic.co.kr/skapi/kiosk/A2300427";
    // kiosk user_id Test url
    private string user_url = $"https://go.selpic.co.kr/skapi/order/WITKIOSK";

    private string post_url = $"https://go.selpic.co.kr/skapi/upload";

    //private string iamge_path = $"D:\\JH_Project\\insa_kiosk\\photo.jpg";
    private string iamge_path = "";

    public void OnClickPrintButton()
    {

        //StartCoroutine(GetRequest(kiosk_url));

        //StartCoroutine(GetRequest(user_url));

        iamge_path = PlayerPrefs.GetString($"arFilePath");

        Debug.LogError($"Save File Path : {iamge_path}");

        StartCoroutine(PostRequest(iamge_path));


        // "MyButton" 이름의 버튼 오브젝트 찾기
        GameObject buttonObject = GameObject.Find("Button01 (2)");

        targetButton = buttonObject.GetComponent<Button>();

        targetButton.interactable = false;

    }

    private IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error : " + webRequest.error);
            }
            else
            {
                Debug.Log("Response : " + webRequest.downloadHandler.text);
                //Debug.Log("URL: " + webRequest.url);
            }
        }
    }

    private IEnumerator PostRequest(string path)
    {
        // 파일이 존재하는지 확인
        if (!File.Exists(path))
        {
            Debug.LogError("File not found at " + path);
            yield break;
        }

        byte[] fileData = File.ReadAllBytes(path);
        

        WWWForm form = new WWWForm();
        //A1800112 테스트 기기
        //A2300427 카페 옆 기기
        form.AddField("robot_id", "A2300427");
        form.AddField("user_id", "WITKIOSK");
        form.AddBinaryData("image", fileData, Path.GetFileName(path), "image/jpeg");

        UnityWebRequest www = UnityWebRequest.Post(post_url, form);

        //Debug.LogError("URL FIRST: " + www.url);

        //www.SetRequestHeader("Cache-Control", "no-cache");
        www.redirectLimit = 0;
        //www.useHttpContinue = false;

        yield return www.SendWebRequest();

        //Debug.LogError("URL SECOND: " + www.url);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
            Debug.LogError("Response Code: " + www.responseCode);
            Debug.LogError("URL THIRD: " + www.url);
        }
        else
        {
            Debug.Log("Image successfully uploaded");
        }


        //// 파일을 바이트 배열로 읽기
        //byte[] imageData = File.ReadAllBytes(path);

        //// UnityWebRequest를 이용하여 POST 요청 생성
        //using (UnityWebRequest webRequest = new UnityWebRequest(post_url, "POST"))
        //{
        //    // 업로드 핸들러로 바이트 데이터를 설정
        //    webRequest.uploadHandler = new UploadHandlerRaw(imageData);
        //    webRequest.downloadHandler = new DownloadHandlerBuffer();

        //    // 헤더 설정: Content-Type을 바이너리 형식으로 설정
        //    webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
        //    webRequest.SetRequestHeader("Content-Disposition", $"attachment; image=\"{Path.GetFileName(path)}\"");

        //    // 요청 전송
        //    yield return webRequest.SendWebRequest();

        //    if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        //    {
        //        Debug.LogError("Error: " + webRequest.error);
        //        Debug.LogError("Response Code: " + webRequest.responseCode);
        //        Debug.LogError("URL: " + webRequest.url);
        //    }
        //    else
        //    {
        //        Debug.Log("Upload complete! Server Response: " + webRequest.downloadHandler.text);
        //    }
        //}
    }
}

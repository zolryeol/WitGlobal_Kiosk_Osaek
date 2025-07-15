using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ClickLogger : MonoBehaviour
{
    [Serializable]
    public class ClickEvent
    {
        public string kiosk_name;
        public string button_name;
        public string category_1;
        public string category_2;
        public string store_no;
        public string visitor;
        public string hours;
        public string playdo;
        public string click_time;
    }

    public ClickEvent clickEvent;

    //[SerializeField] private Canvas canvas;
    public Canvas canvas;


    public void Start()
    {

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>(); // Scene에서 첫 번째 Canvas를 자동으로 찾음
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in the scene! Please assign one in the Inspector.");
            }
        }

        Button[] buttons = canvas.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            if (button.name.Contains("Key"))    continue;

            //Debug.LogError($"{button.name} Find!!!!");


            button.onClick.RemoveAllListeners(); // 기존 리스너 제거
            button.onClick.AddListener(() =>
            {
                //Debug.LogError($"{button.name} clicked!");
                ParameterMatch(button.name);
                OnButtonClick();
            });
        }
    }

    public void ParameterMatch(string name)
    {
        clickEvent.category_2 = "Null";
        clickEvent.store_no = "Null";

        if (name == "Art")
        {
            name = "Museum";
        }
        else if (name == "Tran")
        {
            name = "Inform";
            clickEvent.category_2 = "PublicTrans";
        }
        else if (name == "FooterIcon1")
        {
            name = "Inform";
            clickEvent.category_2 = "InsaMap";
        }
        else if (name == "FooterIcon2")
        {
            name = "Camera";
        }
        else if (name == "FooterIcon3")
        {
            name = "ClothesInform";
        }

        clickEvent.kiosk_name = GlobalManager.Instance.kioskName;        
        clickEvent.click_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        clickEvent.button_name = name;
        clickEvent.category_1 = name;

        clickEvent.visitor = "Null";
        clickEvent.hours = "Null";
        clickEvent.playdo = "Null";
        //Debug.LogError($"ParameterMatch KIOSK_NAME : {clickEvent.kiosk_name}\tTIME : {clickEvent.click_time}\tBUTTON_NAME : {clickEvent.button_name}\tCATEGORY_1 : {clickEvent.category_1}\tCATEGORY_2 : {clickEvent.category_2}\tCATEGORY_3 : {clickEvent.category_3}");
    }

    public void OnButtonClick()
    {
        string json = JsonUtility.ToJson(clickEvent);
        StartCoroutine(SendClickData(json));
    }

    private IEnumerator SendClickData(string json)
    {
        string serverUrl = GlobalManager.Instance.domain + GlobalManager.Instance.record_click_api;

        //Debug.LogError($"serverUrl : {serverUrl}");

        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Click data logged successfully");
            }
            else
            {
                Debug.LogError("Failed to log click data: " + www.error);
            }
        }
    }
}


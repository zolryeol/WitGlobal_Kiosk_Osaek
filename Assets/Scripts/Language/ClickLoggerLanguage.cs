using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ClickLoggerLanguage : MonoBehaviour
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

    public Canvas canvas;

    public void Start()
    {

        Transform categoryButtonsParent = GameObject.Find("LanguageItemContent").transform;
        Button[] buttons = categoryButtonsParent.GetComponentsInChildren<Button>();

        //foreach (Button button in buttons)
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // 캡처한 인덱스

            //Debug.LogError($"index : {index}, {buttons[i].name} Find!!!!");
            //button.onClick.RemoveAllListeners(); // 기존 리스너 제거
            buttons[i].onClick.AddListener(() =>
            {
                ParameterMatch(index);
                OnButtonClick();
            });
        }

    }

    public void ParameterMatch(int index)
    {
        // 카테고리 값
        string CategoryStr = "NULL";
        if (index == 0)
        {
            CategoryStr = "한국어";
        }
        else if (index == 1)
        {
            CategoryStr = "영어";
        }
        else if (index == 2)
        {
            CategoryStr = "일본어";
        }
        else if (index == 3)
        {
            CategoryStr = "중국어";
        }

        clickEvent.kiosk_name = GlobalManager.Instance.kioskName;        
        clickEvent.click_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        clickEvent.button_name = CategoryStr;
        clickEvent.category_1 = "Lang";
        clickEvent.category_2 = CategoryStr;


        clickEvent.store_no = "Null";

        clickEvent.visitor = "Null";
        clickEvent.hours = "Null";
        clickEvent.playdo = "Null";
        
    }

    public void OnButtonClick()
    {
        string json = JsonUtility.ToJson(clickEvent);
        StartCoroutine(SendClickData(json));
    }

    private IEnumerator SendClickData(string json)
    {
        string serverUrl = GlobalManager.Instance.domain + GlobalManager.Instance.record_click_api;

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


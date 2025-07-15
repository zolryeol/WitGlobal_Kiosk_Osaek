using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ClickLoggerAR : MonoBehaviour
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
    [SerializeField]
    Button button;

    public ClickEvent clickEvent;

    public void Start()
    {
        //Transform categoryButtonsParent = GameObject.Find("Buttons").transform;
        //Button[] buttons = categoryButtonsParent.GetComponentsInChildren<Button>();


        //foreach (Button button in buttons)
        //for (int i = 0; i < buttons.Length; i++)
        {
            //int index = i;  // 캡처한 인덱스

            //buttons[i].onClick.AddListener(() =>
            button.onClick.AddListener(()=>
            {
                ParameterMatch();
                OnButtonClick();
            });
        }

    }

    public void ParameterMatch()
    {
        clickEvent.kiosk_name = GlobalManager.Instance.kioskName;        
        clickEvent.click_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        clickEvent.button_name = "PhotoButton";
        clickEvent.category_1 = "AR";
        clickEvent.category_2 = "ClothSelect";

        int HanbokIndex = PlayerPrefs.GetInt("testHanbokIndex");
        //Debug.LogError($"HanbokIndex : {HanbokIndex}");
        
        clickEvent.store_no = HanbokIndex.ToString();
        
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


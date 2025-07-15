using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ClickLoggerAICourse : MonoBehaviour
{
    [Serializable]
    public class ClickEvent
    {
        public string kiosk_name;
        public string button_name;
        public string category_1;
        public string category_2;
        public string store_no;
        public string click_time;
        public string visitor;
        public string hours;
        public string playdo;
    }

    public ClickEvent clickEvent;

    public void Start()
    {
        Transform categoryButtonsParent = GameObject.Find("CourseButtons").transform;
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
        int visitIndex = PlayerPrefs.GetInt("AISelectVisit");
        int hoursIndex = PlayerPrefs.GetInt("AISelectTime");
        int Ca1Index = PlayerPrefs.GetInt("AISelectCategory1");
        int Ca2Index = PlayerPrefs.GetInt("AISelectCategory2");
        int Ca3Index = PlayerPrefs.GetInt("AISelectCategory3");

        string ca1Str = CategoryController.GetCategory(MenuName.AI, Ca1Index, "ko");
        string ca2Str = CategoryController.GetCategory(MenuName.AI, Ca2Index, "ko");
        string ca3Str = CategoryController.GetCategory(MenuName.AI, Ca3Index, "ko");

        // 카테고리 값
        string CategoryStr = "NULL";

        string name = "NULL";
        if (index == 0) 
        {
            name = "A코스";
        }
        else if(index == 1)
        {
            name = "B코스";
        }
        else if (index == 2)
        {
            name = "C코스";
        }

        //Debug.LogError($"index : {index} Str : {foodCategoryStr}");

        clickEvent.kiosk_name = GlobalManager.Instance.kioskName;        
        clickEvent.click_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        clickEvent.button_name = name;
        clickEvent.category_1 = "AI";
        clickEvent.category_2 = CategoryStr;

        clickEvent.store_no = "Null";

        clickEvent.visitor = $"{visitIndex}";
        clickEvent.hours = $"{hoursIndex}";
        clickEvent.playdo = $"{ca1Str},{ca2Str},{ca3Str}";

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


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveToHelpDetailScene : MonoBehaviour
{
    [SerializeField]
    private string helpDetailScene;
    [SerializeField]
    private TextMeshProUGUI helpNo;

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

    public void MoveScene()
    {
        PlayerPrefs.SetString("helpNo", helpNo.text);
        SceneManager.LoadScene(helpDetailScene);

        ParameterCheck();
        OnButtonClick();
    }

    public void ParameterCheck()
    {
        string No = PlayerPrefs.GetString("helpNo");
        string category = PlayerPrefs.GetString("Category");

        //Debug.LogError($"No : {foodNo}, category : {category}");

        clickEvent.kiosk_name = GlobalManager.Instance.kioskName;
        clickEvent.click_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        clickEvent.button_name = "Detail";
        clickEvent.category_1 = "Help";
        clickEvent.category_2 = category;
        clickEvent.store_no = No;

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

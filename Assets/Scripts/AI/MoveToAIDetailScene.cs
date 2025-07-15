using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveToAIDetailScene : MonoBehaviour
{
    [SerializeField]
    private GameObject[] aiInfo;

    // 현재 리스트씬인지 확인
    private Boolean isListScene = true;

    // 보여줄 리스트 / 디테일 요소
    [SerializeField]
    private GameObject listScene;
    [SerializeField]
    private GameObject detailScene;

    // 디테일 컨트롤러
    [SerializeField]
    private AIDetailApiDataFetcher apiDataFetcher;

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

    public void Start()
    {
        // 각 버튼에 클릭 이벤트를 할당
        for (int i = 0; i < aiInfo.Length; i++)
        {
            int index = i;  // 캡처한 인덱스
            aiInfo[i].GetComponent<Button>().onClick.AddListener(() => MoveScene(index));
        }
    }

    public void MoveScene(int index)
    {
        isListScene = false;

        // aiNo로 리스트 불러오기
        string aiNo = aiInfo[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        apiDataFetcher.UpdateDetail(aiNo);

        // 리스트 숨기고 디테일 보이기
        listScene.SetActive(false);
        detailScene.SetActive(true);

        ParameterCheck(aiNo);
        OnButtonClick();

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    public void OnClickBackButton()
    {
        // 만약 지금이 리스트 씬이면 되돌아가기
        if (isListScene)
        {
            SceneManager.LoadScene("AISelect");
        }
        // 리스트 씬이 아니면 리스트 보이고 디테일 숨기기
        else
        {
            isListScene = true;
            listScene.SetActive(true);
            detailScene.SetActive(false);
        }
    }

    public void ParameterCheck(string aiNo)
    {
        string category = "Null";

        //Debug.LogError($"No : {foodNo}, category : {category}");

        clickEvent.kiosk_name = GlobalManager.Instance.kioskName;
        clickEvent.click_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        clickEvent.button_name = "Detail";
        clickEvent.category_1 = "AI";
        clickEvent.category_2 = category;
        clickEvent.store_no = aiNo;

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

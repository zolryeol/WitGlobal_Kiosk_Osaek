using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HanbokButtonController : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons; 
    [SerializeField]
    private Color selectedColor = Color.red;  
    [SerializeField]
    private Color normalColor = Color.black; 
    [SerializeField]
    private Sprite selectedImage;
    [SerializeField]
    private Sprite normalImage;

    [SerializeField]
    private GameObject[] scrollViews;

    [SerializeField]
    private GameObject photoButtonText;

    public int currentIndex { get; private set; }

    private void Start()
    {
        // 각 버튼에 클릭 이벤트를 할당
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // 캡처한 인덱스
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // 첫번째 버튼 클릭돼야함
        currentIndex = 0;
        OnButtonClick(0);

        languageChange();
    }

    // 버튼 클릭 시 이벤트
    private void OnButtonClick(int index)
    {
        currentIndex = index;

        ResetButton();
        ChangeButton(index);
        ShowContent(index);
    }

    // 클릭된 버튼의 텍스트 색상 및 스프라이트를 변경
    private void ChangeButton(int index)
    {
        TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = selectedColor;
        buttons[index].GetComponent<Image>().sprite = selectedImage;
    }

    // 버튼들의 텍스트 색상 및 스프라이트 초기화
    private void ResetButton()
    {
        foreach (Button button in buttons)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.color = normalColor;
            button.GetComponent<Image>().sprite = normalImage;
        }
    }

    // 선택된 버튼에 맞는 콘텐츠 보이기
    private void ShowContent(int index)
    {
        // 다른 콘텐츠 안보이기
        foreach (GameObject contents in scrollViews)
        {
            contents.SetActive(false);
        }

        // 선택된 콘텐츠 보여버리기~
        scrollViews[index].SetActive(true);
    }

    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.aRSelectSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("aRSelect");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray)
        {
            

            // key와 동일한 이름의 GameObject를 찾음
            GameObject targetObject = GameObject.Find(key);
            //GameObject targetObject = FindObjectsOfType<GameObject>(true).FirstOrDefault(obj => obj.name == key); // true는 비활성화된 오브젝트도 포함
            if (targetObject != null)
            {
                // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
                    // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
                    textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용         

                    //Debug.LogError($"key : {key} \ntextComponent.text : {textComponent.text}");
                }
                else
                {
                    Debug.LogWarning($"TextMeshProUGUI component not found in {key}");
                }
            }
            else
            {
                targetObject = FindObjectsOfType<GameObject>(true).FirstOrDefault(obj => obj.name == key); // true는 비활성화된 오브젝트도 포함
                if (targetObject != null)
                {
                    // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                    TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                    if (textComponent != null)
                    {
                        // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
                        // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
                        textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용         

                        //Debug.LogError($"key : {key} \ntextComponent.text : {textComponent.text}");
                    }
                    else
                    {
                        Debug.LogWarning($"TextMeshProUGUI component not found in {key}");
                    }
                }
                else
                {
                    Debug.LogWarning($"GameObject with name {key} not found");
                }
                Debug.LogWarning($"GameObject with name {key} not found");
            }
        }
    }
}
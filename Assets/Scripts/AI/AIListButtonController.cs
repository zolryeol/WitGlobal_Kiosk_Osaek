using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AIListButtonController : MonoBehaviour
{
    [SerializeField]
    private Button[]     buttons;            // ��ư ���
    [SerializeField]
    private Sprite       selectedImage;      // ��ư ���� �̹���
    [SerializeField]
    private Sprite       normalImage;        // ��ư ���� �̹���
    [SerializeField]
    private Color        selectedColor;      // ��ư ���� �ؽ�Ʈ�÷�
    [SerializeField]
    private Color        normalColor;        // ��ư ���� �ؽ�Ʈ�÷�

    [SerializeField]
    private GameObject[] gameObjects;        // ����,���� ó���� ��ҵ�

    bool categoryClickFlag = false;

    private void Start()
    {
        // �� ��ư�� Ŭ�� �̺�Ʈ�� �Ҵ�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // ĸó�� �ε���
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
        
        // ù��° ��ư Ŭ���ž���
        OnButtonClick(0);
    }

    // ��ư Ŭ�� �� �̺�Ʈ
    private void OnButtonClick(int index)
    {        
        // ���� �̹� ���õ� ��ư Ŭ����
        if (buttons[index].GetComponentInChildren<TextMeshProUGUI>().color == selectedColor)
        {
            // ��ư ����ȿ�� ����
            ResetButton();
            ShowContent(0);
        }
        // �ƴ϶�� ��ư ����ȿ��
        else
        {
            // �ٸ� ��� ��ư �̺�Ʈ �ʱ�ȭ
            ResetButton();
            ChangeButton(index);
            ShowContent(index);
        }
        languageChange();
    }

    // Ŭ���� ��ư�� �ؽ�Ʈ ���� �� ��������Ʈ�� ����
    private void ChangeButton(int index)
    {
        TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = selectedColor;
        buttons[index].GetComponent<Image>().sprite = selectedImage;        
    }

    // ��ư���� �ؽ�Ʈ ���� �� ��������Ʈ �ʱ�ȭ
    private void ResetButton()
    {
        foreach (Button button in buttons)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.color = normalColor;
            button.GetComponent<Image>().sprite = normalImage;
        }
     
    }
    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.aiListSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("aiList");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray)
        {
            // key와 동일한 이름의 GameObject를 찾음
            GameObject targetObject = GameObject.Find(key);
            if (targetObject != null)
            {
                // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
                    // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
                    textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용                         
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
        }
    }

    // ���õ� ��ư�� �´� ������ ���̱�
    private void ShowContent(int index)
    {
        // �ٸ� ������ �Ⱥ��̱�
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }

        // ���õ� ������ ����������~
        gameObjects[index].SetActive(true);        

        if(categoryClickFlag) new VideoController().OnChangeVideo("3", "what");
        categoryClickFlag = true;

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }
}
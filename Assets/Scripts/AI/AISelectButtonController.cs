
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AISelectButtonController : MonoBehaviour
{
    [Header("visitButtons")]
    [SerializeField]
    private Button[] visitButtons;
    [SerializeField]
    private Sprite normalVisitButton;
    [SerializeField]
    private Sprite selectedVisitButton;
    [SerializeField]
    private Color normalVisitColor;
    [SerializeField]
    private Color selectedVisitColor;

    [Header("timeButtons")]
    [SerializeField]
    private Button[] timeButtons;
    [SerializeField]
    private Sprite normalTimeButton;
    [SerializeField]
    private Sprite selectedTimeButton;
    [SerializeField]
    private Color normalTimeColor;
    [SerializeField]
    private Color selectedTimeColor;

    [Header("categoryButtons")]
    [SerializeField]
    private Button[] categoryButtons;
    [SerializeField]
    private Sprite normalCategoryButton;
    [SerializeField]
    private Sprite selectedCategoryButton;
    [SerializeField]
    private Color selectedCategoryColor;

    [Header("SubmitButton")]
    [SerializeField]
    private Button submitButton;

    private Boolean isCheckVisitButton = false;
    private Boolean isCheckTimeButton = false;
    private Dictionary<int, ButtonObject> buttonMap;

    private void Awake()
    {
        buttonMap = new Dictionary<int, ButtonObject>();
    }

    private void Start()
    {

        languageChange();
        // �� ��ư�� Ŭ�� �̺�Ʈ�� �Ҵ�
        for (int i = 0; i < visitButtons.Length; i++)
        {
            int index = i;  // ĸó�� �ε���
            visitButtons[i].onClick.AddListener(() => OnclickVisitButton(index));
        }

        for (int i = 0; i < timeButtons.Length; i++)
        {
            int index = i;
            timeButtons[i].onClick.AddListener(() => OnclickTimeButton(index));
        }

        for (int i = 0; i < categoryButtons.Length; i++)
        {
            int index = i;
            categoryButtons[i].onClick.AddListener(() => OnclickCategoryButton(index));
        }
    }

    private void languageChange()
    {        
        LanguageService ls = new LanguageService();
        ls.aiSelectSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("aiSelect");

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

    // �湮�ο� ���� �̺�Ʈ
    private void OnclickVisitButton(int index)
    {
        isCheckVisitButton = true;
        validButtonCheck();

        // �� ���� ���õ� ȿ���� ����
        foreach (Button button in visitButtons)
        {
            // �ؽ�Ʈ �÷� ����
            button.GetComponentInChildren<TextMeshProUGUI>().color = normalVisitColor;
            // ��ư �̹��� ����
            button.GetComponent<Image>().sprite = normalVisitButton;
        }

        // ���� ���õ� ��ư ȿ�� ����
        visitButtons[index].GetComponentInChildren<TextMeshProUGUI>().color = selectedVisitColor;
        visitButtons[index].GetComponent<Image>().sprite = selectedVisitButton;

        // ���õ� �ε��� ����
        int aiSelectVisit = 0;
        switch (index)
        {
            case 0:
                aiSelectVisit = 1;
                break;
            case 1:
                aiSelectVisit = 2;
                break;
            case 2:
                aiSelectVisit = 3;
                break;
            case 3:
                aiSelectVisit = 4;
                break;
            case 4:
                aiSelectVisit = 5;
                break;
            case 5:
                aiSelectVisit = 6;
                break;
            default:
                aiSelectVisit = 2;
                break;
        }
        PlayerPrefs.SetInt("AISelectVisit", aiSelectVisit);
    }

    // ü���ð� ���� �̺�Ʈ
    private void OnclickTimeButton(int index)
    {
        isCheckTimeButton = true;
        validButtonCheck();

        // �� ���� ���õ� ȿ���� ����
        foreach (Button button in timeButtons)
        {
            // �ؽ�Ʈ �÷� ����
            button.GetComponentInChildren<TextMeshProUGUI>().color = normalTimeColor;
            // ��ư �̹��� ����
            button.GetComponent<Image>().sprite = normalTimeButton;
        }

        // ���� ���õ� ��ư ȿ�� ����
        timeButtons[index].GetComponentInChildren<TextMeshProUGUI>().color = selectedTimeColor;
        timeButtons[index].GetComponent<Image>().sprite = selectedTimeButton;

        // ���õ� �ε��� ����
        PlayerPrefs.SetInt("AISelectTime", (index * 2) + 1);
    }

    // ���Ÿ� ���� �̺�Ʈ
    private void OnclickCategoryButton(int index)
    {
        // �̹� ���õ� ģ������ �˻�
        if (buttonMap.ContainsKey(index))
        {
            // ���� ���õ� ģ�� ���� ����
            ButtonObject selectedButtonObject = buttonMap[index];
            categoryButtons[selectedButtonObject.buttonIndex].GetComponentInChildren<TextMeshProUGUI>().color = selectedButtonObject.textColor;
            categoryButtons[selectedButtonObject.buttonIndex].GetComponent<Image>().sprite = normalCategoryButton;
            buttonMap.Remove(index);
            return;
        }

        // ���� ��ư�� 3���̻� ���õƴٸ� �������
        if (buttonMap.Count >= 3) return;

        // ��� Ŭ���� ��ư���� ����
        ButtonObject buttonObject = new ButtonObject(index, categoryButtons[index].GetComponentInChildren<TextMeshProUGUI>().color);
        buttonMap.Add(index, buttonObject);

        // ��� Ŭ���� ��ư �ؽ�Ʈ �÷� �� ��ư �̹��� ����
        categoryButtons[index].GetComponentInChildren<TextMeshProUGUI>().color = selectedCategoryColor;
        categoryButtons[index].GetComponent<Image>().sprite = selectedCategoryButton;

        validButtonCheck();
    }

    // �λ翡�� ��õ�ޱ� ��ư Ŭ����
    public void moveScene()
    {
        // ���� ���õ� ���Ÿ� ������
        List<int> selectedKeys = new List<int>(buttonMap.Keys);

        // ��ư�� 1�� ���õ� ���
        if (buttonMap.Count == 1)
        {
            int selectedKey = selectedKeys[0];
            PlayerPrefs.SetInt("AISelectCategory1", selectedKey);
            PlayerPrefs.SetInt("AISelectCategory2", selectedKey);
            PlayerPrefs.SetInt("AISelectCategory3", selectedKey);
        }
        // ��ư�� 2�� ���õ� ���
        else if (buttonMap.Count == 2)
        {
            int firstKey = selectedKeys[0];
            int secondKey = selectedKeys[1];
            PlayerPrefs.SetInt("AISelectCategory1", firstKey);
            PlayerPrefs.SetInt("AISelectCategory2", firstKey);
            PlayerPrefs.SetInt("AISelectCategory3", secondKey);
        }
        // ��ư�� 3�� ���õ� ���
        else
        {
            PlayerPrefs.SetInt("AISelectCategory1", selectedKeys[0]);
            PlayerPrefs.SetInt("AISelectCategory2", selectedKeys[1]);
            PlayerPrefs.SetInt("AISelectCategory3", selectedKeys[2]);
        }

        SceneManager.LoadSceneAsync("AIList");
    }

    // 버튼이 총 5개 선택됐는지 확인
    private void validButtonCheck()
    {
        if (isCheckVisitButton && isCheckTimeButton && buttonMap.Count >= 3)
        {
            submitButton.gameObject.SetActive(true);
            languageChange();
        }        
    }
}

public class ButtonObject
{
    public ButtonObject(int buttonIndex, Color textColor)
    {
        this.buttonIndex = buttonIndex;
        this.textColor = textColor;
    }

    public int buttonIndex;
    public Color textColor;
}

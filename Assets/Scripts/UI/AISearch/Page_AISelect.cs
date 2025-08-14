using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

// AI 페이지에서 선택한것을 바탕으로 AI 코스를 생성
// AI는 사실 다 거짓이다.

public class Page_AISelect : MonoBehaviour
{
    [SerializeField]
    Transform peopleCountButtonParent; // 사람 수 버튼 부모
    [SerializeField]
    Transform stayTimeButtonParent; // 체류 시간 버튼 부모
    [SerializeField]
    Transform aiCategoryButtonParent; // AI 카테고리 버튼 부모

    Button[] peopleCountButton = new Button[6]; // 1 ~  6명까지
    Button[] stayTimeButton = new Button[4];
    [SerializeField]
    AICategoryButton[] aiCategoryButton = new AICategoryButton[30]; // 카테고리 버튼.

    [SerializeField]
    public Button generateButton; // AI 코스 생성 버튼 누르면 AI 코스 생성됨
    [SerializeField]
    Button resetButton; // 초기화 버튼

    [SerializeField]
    private int selectedPeopleIndex = -1;
    [SerializeField]
    private int selectedStayTimeIndex = -1;

    public int aiSelectedCount = 0; // AI 카테고리 선택 개수

    List<AICategory> selectedCategoryList = new(); // 선택된 AI 카테고리 리스트

    public void Init()
    {
        AssignedButtons();

        for (int i = 0; i < peopleCountButton.Length; i++)
        {
            int index = i;
            peopleCountButton[i].onClick.AddListener(() => OnPeopleCountButtonClicked(index));
            CommonFunction.ChangeColorBtnAndTxt(peopleCountButton[i].transform, false);
        }

        for (int i = 0; i < stayTimeButton.Length; i++)
        {
            int index = i;
            stayTimeButton[i].onClick.AddListener(() => OnStayTimeButtonClicked(index));
            CommonFunction.ChangeColorBtnAndTxt(stayTimeButton[i].transform, false);
        }

        for (int i = LoadManager.Instance.AICategorieList.Count; i < aiCategoryButton.Length; i++)
        {
            aiCategoryButton[i].gameObject.SetActive(false);
        }

        generateButton.onClick.AddListener(UIManager.Instance.OnAIRecommendPage);
        generateButton.onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.AISearch_Category));

        UpdateGenerateButtonState();

        UIManager.Instance.ChangeLanguageEvent += LanguageChanged;
    }

    void AssignedButtons()
    {
        // 사람 수 버튼 할당
        for (int i = 0; i < peopleCountButton.Length; i++)
        {
            peopleCountButton[i] = peopleCountButtonParent.GetChild(i).GetComponent<Button>();
        }
        // 체류 시간 버튼 할당
        for (int i = 0; i < stayTimeButton.Length; i++)
        {
            stayTimeButton[i] = stayTimeButtonParent.GetChild(i).GetComponent<Button>();
        }
        // AI 카테고리 버튼 할당
        for (int i = 0; i < aiCategoryButton.Length; i++)
        {
            aiCategoryButton[i] = aiCategoryButtonParent.GetChild(i).GetComponent<AICategoryButton>();

            aiCategoryButton[i].Init(this); // AISelector 인스턴스 할당

            aiCategoryButton[i].SetCategoryInfo(i); // 카테고리 정보 설정
        }
    }

    // 사람 수 버튼이 클릭되었을 때 호출되는 메서드
    public void OnPeopleCountButtonClicked(int index)
    {
        if (selectedPeopleIndex == index)
            selectedPeopleIndex = -1; // 해제
        else
            selectedPeopleIndex = index;

        for (int i = 0; i < peopleCountButton.Length; i++)
        {
            if (i == selectedPeopleIndex)
            {
                CommonFunction.ChangeColorBtnAndTxt(peopleCountButton[i].transform);
            }
            else
            {
                CommonFunction.ChangeColorBtnAndTxt(peopleCountButton[i].transform, false);
            }
        }

        UpdateGenerateButtonState();
    }

    // 체류 시간 버튼이 클릭되었을 때 호출되는 메서드
    public void OnStayTimeButtonClicked(int index)
    {
        if (selectedStayTimeIndex == index)
            selectedStayTimeIndex = -1; // 해제
        else
            selectedStayTimeIndex = index;

        for (int i = 0; i < stayTimeButton.Length; i++)
        {
            if (i == selectedStayTimeIndex)
            {
                CommonFunction.ChangeColorBtnAndTxt(stayTimeButton[i].transform);
            }
            else
            {
                CommonFunction.ChangeColorBtnAndTxt(stayTimeButton[i].transform, false);
            }
        }

        UpdateGenerateButtonState();
    }

    public void UpdateGenerateButtonState()
    {
        bool canGenerate = selectedPeopleIndex != -1 && selectedStayTimeIndex != -1 && 3 <= aiSelectedCount;

        generateButton.gameObject.SetActive(canGenerate);
    }

    public void ResetAll()
    {
        foreach (var btn in peopleCountButton)
        {
            CommonFunction.ChangeColorBtn(btn.transform, false);
        }
        selectedPeopleIndex = -1;

        foreach (var btn in stayTimeButton)
        {
            CommonFunction.ChangeColorBtn(btn.transform, false);
        }
        selectedStayTimeIndex = -1;

        foreach (var btn in aiCategoryButton)
        {
            btn.DeSelected();
        }
        aiSelectedCount = 0;

        selectedCategoryList.Clear();

        UpdateGenerateButtonState();
    }

    public void AddSelectedCategory(AICategory category)
    {
        selectedCategoryList.Add(category);
    }
    public void RemoveSelectedCategory(AICategory category)
    {
        if (selectedCategoryList.Contains(category))
        {
            selectedCategoryList.Remove(category);
        }
    }
    public List<AICategory> GetSelectedCategoryList()
    {
        return selectedCategoryList;
    }
    public void LanguageChanged()
    {
        for (int i = 0; i < aiCategoryButton.Length; i++)
        {
            aiCategoryButton[i] = aiCategoryButtonParent.GetChild(i).GetComponent<AICategoryButton>();

            aiCategoryButton[i].SetCategoryInfo(i); // 카테고리 정보 설정
        }
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PeopleButtonController : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;            // 버튼 목록
    [SerializeField]
    private Sprite selectedImage;        // 버튼 선택 이미지
    [SerializeField]
    private Sprite normalImage;          // 버튼 해제 이미지
    [SerializeField]
    private Color selectedColor;         // 버튼 선택 텍스트컬러
    [SerializeField]
    private Color normalColor;           // 버튼 해제 텍스트컬러
    [SerializeField]
    private TMP_FontAsset selectedFont;  // 버튼 선택 폰트
    [SerializeField]
    private TMP_FontAsset normalFont;    // 버튼 해제 폰트

    // 선택인원
    public int peopleCount { get; private set; }


    private void Start()
    {
        // 각 버튼에 클릭 이벤트를 할당
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // 캡처한 인덱스
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // 첫번째 버튼 클릭돼야함
        OnButtonClick(0);
    }

    // 버튼 클릭 시 이벤트
    private void OnButtonClick(int index)
    {
        // 다른 모든 버튼 이벤트 초기화
        ResetButton();
        ChangeButton(index);

        // 인원수 갱신
        peopleCount = index + 1;
    }

    // 클릭된 버튼의 텍스트 색상 및 스프라이트를 변경
    private void ChangeButton(int index)
    {
        buttons[index].GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
        buttons[index].GetComponentInChildren<TextMeshProUGUI>().font = selectedFont;
        buttons[index].GetComponent<Image>().sprite = selectedImage;
    }

    // 버튼들의 텍스트 색상 및 스프라이트 초기화
    private void ResetButton()
    {
        foreach (Button button in buttons)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().color = normalColor;
            button.GetComponentInChildren<TextMeshProUGUI>().font = normalFont;
            button.GetComponent<Image>().sprite = normalImage;
        }
    }
}

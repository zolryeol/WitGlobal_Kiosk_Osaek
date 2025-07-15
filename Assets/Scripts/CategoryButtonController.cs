using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryButtonController : MonoBehaviour
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
    private TMP_FontAsset selectedFont;           // 버튼 선택 폰트
    [SerializeField]
    private TMP_FontAsset normalFont;             // 버튼 해제 폰트

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
    }

    // 버튼 클릭 시 이벤트
    private void OnButtonClick(int index)
    {
        currentIndex = index;

        // 다른 모든 버튼 이벤트 초기화
        ResetButton();
        ChangeButton(index);
    }

    // 클릭된 버튼의 텍스트 및 스프라이트를 변경
    private void ChangeButton(int index)
    {
        TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = selectedColor;
        buttonText.font = selectedFont;
        buttons[index].GetComponent<Image>().sprite = selectedImage;
    }

    // 버튼들의 텍스트 및 스프라이트 초기화
    private void ResetButton()
    {
        foreach (Button button in buttons)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.color = normalColor;
            buttonText.font = normalFont;
            button.GetComponent<Image>().sprite = normalImage;
        }
    }
}

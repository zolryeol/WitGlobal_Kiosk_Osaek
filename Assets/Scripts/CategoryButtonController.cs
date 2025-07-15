using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryButtonController : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;            // ��ư ���
    [SerializeField]
    private Sprite selectedImage;        // ��ư ���� �̹���
    [SerializeField]
    private Sprite normalImage;          // ��ư ���� �̹���
    [SerializeField]
    private Color selectedColor;         // ��ư ���� �ؽ�Ʈ�÷�
    [SerializeField]
    private Color normalColor;           // ��ư ���� �ؽ�Ʈ�÷�
    [SerializeField]
    private TMP_FontAsset selectedFont;           // ��ư ���� ��Ʈ
    [SerializeField]
    private TMP_FontAsset normalFont;             // ��ư ���� ��Ʈ

    public int currentIndex { get; private set; }


    private void Start()
    {
        // �� ��ư�� Ŭ�� �̺�Ʈ�� �Ҵ�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // ĸó�� �ε���
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // ù��° ��ư Ŭ���ž���
        currentIndex = 0;
        OnButtonClick(0);
    }

    // ��ư Ŭ�� �� �̺�Ʈ
    private void OnButtonClick(int index)
    {
        currentIndex = index;

        // �ٸ� ��� ��ư �̺�Ʈ �ʱ�ȭ
        ResetButton();
        ChangeButton(index);
    }

    // Ŭ���� ��ư�� �ؽ�Ʈ �� ��������Ʈ�� ����
    private void ChangeButton(int index)
    {
        TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = selectedColor;
        buttonText.font = selectedFont;
        buttons[index].GetComponent<Image>().sprite = selectedImage;
    }

    // ��ư���� �ؽ�Ʈ �� ��������Ʈ �ʱ�ȭ
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

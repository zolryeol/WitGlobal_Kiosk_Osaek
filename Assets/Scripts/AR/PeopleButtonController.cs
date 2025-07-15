using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PeopleButtonController : MonoBehaviour
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
    private TMP_FontAsset selectedFont;  // ��ư ���� ��Ʈ
    [SerializeField]
    private TMP_FontAsset normalFont;    // ��ư ���� ��Ʈ

    // �����ο�
    public int peopleCount { get; private set; }


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
        // �ٸ� ��� ��ư �̺�Ʈ �ʱ�ȭ
        ResetButton();
        ChangeButton(index);

        // �ο��� ����
        peopleCount = index + 1;
    }

    // Ŭ���� ��ư�� �ؽ�Ʈ ���� �� ��������Ʈ�� ����
    private void ChangeButton(int index)
    {
        buttons[index].GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
        buttons[index].GetComponentInChildren<TextMeshProUGUI>().font = selectedFont;
        buttons[index].GetComponent<Image>().sprite = selectedImage;
    }

    // ��ư���� �ؽ�Ʈ ���� �� ��������Ʈ �ʱ�ȭ
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

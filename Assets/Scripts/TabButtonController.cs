using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TabButtonController : MonoBehaviour
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


    private void Start()
    {
        // �� ��ư�� Ŭ�� �̺�Ʈ�� �Ҵ�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // ĸó�� �ε���
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // ù��° ��ư Ŭ���ž���
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("FooterMap"))) OnButtonClick(0);
        else {
            PlayerPrefs.DeleteKey("FooterMap");
            PlayerPrefs.Save(); // 변경 사항 저장
            OnButtonClick(1);
        }

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));
    }
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    // ��ư Ŭ�� �� �̺�Ʈ
    private void OnButtonClick(int index)
    {
        // �ٸ� ��� ��ư �̺�Ʈ �ʱ�ȭ
        ResetButton();
        ChangeButton(index);
        ShowContent(index);
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

    // ���õ� ��ư�� �´� ������ ���̱�
    private void ShowContent(int index)
    {
        // �ٸ� ������ �Ⱥ��̱�
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }

        // GameObject에 붙어 있는 VideoController를 가져옴
        VideoController videoController = GameObject.FindObjectOfType<VideoController>();

        if (videoController != null)
        {
            if (index == 2)
                videoController.OnChangeVideo("2", "trans");
            else
                videoController.OnChangeVideo("1", "trans");
        }
        else Debug.LogError("VideoController를 찾을 수 없습니다.");
        

        // ���õ� ������ ����������~
        gameObjects[index].SetActive(true);
    }
}
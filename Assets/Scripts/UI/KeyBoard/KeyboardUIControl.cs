using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class KeyboardUIControl : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject keyboard;
    [SerializeField] Button closePanelButton;
    HangulKeyborad hangulKeyborad;

    private void Awake()
    {
        hangulKeyborad = transform.parent.GetComponent<HangulKeyborad>();
        button.onClick.AddListener(OnKeyboard);
        button.onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Search));

        if (closePanelButton == null)
        {
            closePanelButton = transform.Find("ClosePanel").GetComponent<Button>();
        }

        closePanelButton.onClick.AddListener(CloseKeyboard);
    }
    private void Start()
    {
        keyboard.SetActive(false);
        closePanelButton.gameObject.SetActive(false);
    }

    void OnKeyboard()
    {
        Debug.Log("키보드 온");
        if (keyboard.activeSelf)
        {
            CloseKeyboard();
        }
        else
        {
            OpenKeyboard();
        }
    }

    public void CloseKeyboard()
    {
        keyboard.SetActive(false);
        closePanelButton.gameObject.SetActive(false);
        hangulKeyborad.Reset();

        if (UIManager.Instance.PageStack.Count == 0) // 메인페이지 예외처리
        {
            button.onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Default));
        }

    }

    public void OpenKeyboard()
    {
        keyboard.SetActive(true);
        closePanelButton.gameObject.SetActive(true);
    }
}
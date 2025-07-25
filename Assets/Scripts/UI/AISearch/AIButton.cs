using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  특별 취급하는 AI 버튼 클래스
/// </summary>
public class AIButton : MonoBehaviour
{
    Button button;
    Page_AISelect aiSelector;

    [SerializeField] CanvasGroup PageAICanvasGroup;
    public void Init()
    {
        if (TryGetComponent<Button>(out Button _button))
        {
            button = _button;
        }
        else button = this.AddComponent<Button>();

        aiSelector = FindObjectOfType<Page_AISelect>();

        PageAICanvasGroup = GameObject.Find("Page_AISelect").GetComponent<CanvasGroup>();

        button.onClick.AddListener(() => UIManager.Instance.OpenPage(PageAICanvasGroup));
        button.onClick.AddListener(aiSelector.ResetAll);

        button.onClick.AddListener(() => UIManager.Instance.CloseKeyboard());

        button.onClick.AddListener(() => UIManager.Instance.SetNowSelectCategory(_EtcCategory: Category_ETC.AISelect));

        button.onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.AISearch));
    }
}

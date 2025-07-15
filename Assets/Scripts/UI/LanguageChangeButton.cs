using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChangeButton : MonoBehaviour
{
    Button button;

    [SerializeField] CanvasGroup languagePageCanvasGroup;

    public void Init()
    {
        button = GetComponent<Button>();

        languagePageCanvasGroup = GameObject.Find("Page_Language").GetComponent<CanvasGroup>();

        button.onClick.AddListener(() => UIManager.Instance.OpenPage(languagePageCanvasGroup));
    }
}

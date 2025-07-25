using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChangeButton : MonoBehaviour
{
    Button button;
    Image languageImage;
    [SerializeField] CanvasGroup languagePageCanvasGroup;

    public void Init()
    {
        languageImage = GetComponent<Image>();
        button = GetComponent<Button>();

        languagePageCanvasGroup = GameObject.Find("Page_Language").GetComponent<CanvasGroup>();

        button.onClick.AddListener(() => UIManager.Instance.OpenPage(languagePageCanvasGroup));
        button.onClick.AddListener(() => UIManager.Instance.CloseKeyboard());
        button.onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.ChangeLanguage));

        UIManager.Instance.ChangeLanguageEvent += ChangeLanguageImage;
    }

    public void ChangeLanguageImage()
    {
        var nowLang = UIManager.Instance.NowLanguage;

        languageImage.sprite = ResourceManager.Instance.LanSelect_Icon[(int)nowLang];
    }
}

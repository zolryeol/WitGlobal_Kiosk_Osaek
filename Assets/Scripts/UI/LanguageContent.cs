using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageContent : MonoBehaviour
{
    public Image BackGround;
    [SerializeField] Image LanguageIcon;
    public Image CheckIcon;

    public Button button;

    public TextMeshProUGUI langText;

    public Language myLanguage;

    public void Init()
    {
        if (TryGetComponent(out Button b))
        {
            button = b;
        }
        //button.onClick.AddListener(Selected);
    }

    public void Selected()
    {
        BackGround.sprite = ResourceManager.Instance.LanguageSelect_Background;
        CheckIcon.sprite = ResourceManager.Instance.Check_Icon;
        LanguageIcon.sprite = ResourceManager.Instance.LanSelect_Icon[(int)myLanguage];
    }

    public void Unselected()
    {
        BackGround.sprite = ResourceManager.Instance.LanguageNormal_Background;
        CheckIcon.sprite = ResourceManager.Instance.NonCheck_Icon;
        LanguageIcon.sprite = ResourceManager.Instance.LanNormal_Icon[(int)myLanguage];
    }

    public void Copy(LanguageContent target)
    {
        LanguageIcon.sprite = target.LanguageIcon.sprite;
        langText.text = target.langText.text;
    }
}

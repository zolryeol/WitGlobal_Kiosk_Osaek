using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class KeyboardKeyButton : MonoBehaviour
{
    Button button;
    TextMeshProUGUI textKr;
    TextMeshProUGUI textEn;
    public KeyboardETC EtcKey = KeyboardETC.Default;
    private void Awake()
    {
        button = GetComponent<Button>();
        textKr = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textEn = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        button.onClick.AddListener(() => Debug.Log($"버튼누름 {gameObject.name}"));

        var hk = FindAnyObjectByType<HangulKeyborad>();

        switch (EtcKey)
        {
            case KeyboardETC.Default:
                button.onClick.AddListener(() => hk.OnClicked(textKr, textEn));
                break;
            case KeyboardETC.Shift:
                button.onClick.AddListener(() => hk.OnShiftClicked());
                break;
            case KeyboardETC.LanguageChange:
                button.onClick.AddListener(() => hk.OnKorEngClicked());
                break;
            case KeyboardETC.Enter:
                button.onClick.AddListener(() => hk.OnEnterClicked());
                break;
            case KeyboardETC.BackSpace:
                button.onClick.AddListener(() => hk.OnBackspaceClicked());
                break;
            case KeyboardETC.Space:
                button.onClick.AddListener(() => hk.OnSpaceClicked());
                break;
            default:
                KioskLogger.Warn("키가없어요");
                break;
        }
    }

    //public (TextMeshProUGUI, TextMeshProUGUI) GetUpdateText()
    //{
    //    return (textKr, textEn);
    //}
}

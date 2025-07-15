using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardKeyButton : MonoBehaviour
{
    Button button;
    TextMeshProUGUI text;
    public KeyboardETC EtcKey = KeyboardETC.Default;
    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        var hk = FindAnyObjectByType<HangulKeyborad>();

        switch (EtcKey)
        {
            case KeyboardETC.Default:
                if (text != null)
                    button.onClick.AddListener(() => hk.OnClicked(text));
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
            case KeyboardETC.Symbol:
                button.onClick.AddListener(() => hk.OnSymbolClicked());
                break;
            default:
                break;
        }
    }
}

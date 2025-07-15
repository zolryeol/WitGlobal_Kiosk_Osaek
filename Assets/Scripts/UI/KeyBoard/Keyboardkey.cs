using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardKey : MonoBehaviour
{
    public string keyValue; // 실제 입력될 문자값 ("ㄱ", "ㅏ", "Enter", 등)
    public TMP_Text targetText; // 버튼에 표시될 텍스트 (자동 할당됨)

    private VirtualKeyboard keyboard;

    private void Awake()
    {
        keyboard = FindAnyObjectByType<VirtualKeyboard>();

        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();

        GetComponent<Button>().onClick.AddListener(OnKeyPressed);
    }

    private void OnKeyPressed()
    {
        if (keyValue == "Shift")
        {
            keyboard.ToggleShift();
        }
        else if (keyValue == "Lang")
        {
            keyboard.ToggleLanguage();
        }
        else
        {
            keyboard.PressKey(keyValue);
        }
    }

    public void ApplyShift(bool shifted)
    {
        if (!keyboard || keyboard.currentLanguage != VirtualKeyboard.LanguageMode.Korean)
            return;

        // 간단한 Shift 변환
        if (shifted)
        {
            if (keyValue == "ㄱ") { keyValue = "ㄲ"; targetText.text = "ㄲ"; }
            else if (keyValue == "ㄷ") { keyValue = "ㄸ"; targetText.text = "ㄸ"; }
            else if (keyValue == "ㅂ") { keyValue = "ㅃ"; targetText.text = "ㅃ"; }
            else if (keyValue == "ㅅ") { keyValue = "ㅆ"; targetText.text = "ㅆ"; }
            else if (keyValue == "ㅈ") { keyValue = "ㅉ"; targetText.text = "ㅉ"; }
            else if (keyValue == "ㅐ") { keyValue = "ㅒ"; targetText.text = "ㅒ"; }
            else if (keyValue == "ㅔ") { keyValue = "ㅖ"; targetText.text = "ㅖ"; }
        }
        else
        {
            if (keyValue == "ㄲ") { keyValue = "ㄱ"; targetText.text = "ㄱ"; }
            else if (keyValue == "ㄸ") { keyValue = "ㄷ"; targetText.text = "ㄷ"; }
            else if (keyValue == "ㅃ") { keyValue = "ㅂ"; targetText.text = "ㅂ"; }
            else if (keyValue == "ㅆ") { keyValue = "ㅅ"; targetText.text = "ㅅ"; }
            else if (keyValue == "ㅉ") { keyValue = "ㅈ"; targetText.text = "ㅈ"; }
            else if (keyValue == "ㅒ") { keyValue = "ㅐ"; targetText.text = "ㅐ"; }
            else if (keyValue == "ㅖ") { keyValue = "ㅔ"; targetText.text = "ㅔ"; }
        }
    }
}

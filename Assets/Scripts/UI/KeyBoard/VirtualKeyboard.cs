
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VirtualKeyboard : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text inputDisplayText;
    public GameObject keyboardRoot;

    public GameObject koreanKeysGroup;
    public GameObject englishKeysGroup;

    public enum LanguageMode { Korean, English }
    public LanguageMode currentLanguage = LanguageMode.Korean;

    private string committedText;
    private bool isShifted = false;

    private readonly char[] chosung_index = { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
    private readonly char[] joongsung_index = { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' };
    private readonly char[] jongsung_index = { ' ', 'ㄱ','ㄲ','ㄳ','ㄴ','ㄵ','ㄶ','ㄷ','ㄹ','ㄺ','ㄻ','ㄼ','ㄽ','ㄾ','ㄿ','ㅀ','ㅁ',
                                               'ㅂ','ㅄ','ㅅ','ㅆ','ㅇ','ㅈ','ㅊ','ㅋ','ㅌ','ㅍ','ㅎ' };

    private readonly char[] Jcombo_index = { 'ㄳ', 'ㄵ', 'ㄶ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅄ' };
    private readonly string[] Jcombo = { "ㄱㅅ", "ㄴㅈ", "ㄴㅎ", "ㄹㄱ", "ㄹㅁ", "ㄹㅂ", "ㄹㅅ", "ㄹㅌ", "ㄹㅍ", "ㄹㅎ", "ㅂㅅ" };

    private readonly char[] Mcombo_index = { 'ㅘ', 'ㅙ', 'ㅚ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅢ' };
    private readonly string[] Mcombo = { "ㅗㅏ", "ㅗㅐ", "ㅗㅣ", "ㅜㅓ", "ㅜㅔ", "ㅜㅣ", "ㅡㅣ" };

    private char chKorInput = ' ';

    public void OpenKeyboard(TMP_InputField field)
    {
        inputField = field;
        committedText = "";
        keyboardRoot.SetActive(true);
        UpdateDisplay();
    }

    public void CloseKeyboard()
    {
        keyboardRoot.SetActive(false);
        committedText = "";
        UpdateDisplay();
    }
    public void SetLanguage(LanguageMode mode)
    {
        currentLanguage = mode;
        koreanKeysGroup.SetActive(mode == LanguageMode.Korean);
        englishKeysGroup.SetActive(mode == LanguageMode.English);
    }

    public void ToggleLanguage()
    {
        SetLanguage(currentLanguage == LanguageMode.Korean ? LanguageMode.English : LanguageMode.Korean);
    }


    public void ToggleShift()
    {
        isShifted = !isShifted;

        foreach (KeyboardKey key in FindObjectsOfType<KeyboardKey>())
        {
            key.ApplyShift(isShifted);
        }
    }

    public void PressKey(string key)
    {
        if (key == "Backspace")
        {
            if (inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            }
        }
        else if (key == "Space")
        {
            inputField.text += " ";
        }
        else if (key == "Enter")
        {
            OnSearch();
        }
        else
        {
            OnClickedOnKor(key[0]);
        }

        UpdateDisplay();
    }

    private void OnClickedOnKor(char inputChar)
    {
        string curText = inputField.text;
        if (curText.Length == 0 || !isKorean(curText[curText.Length - 1]) || !isKorean(inputChar))
        {
            inputField.text += inputChar;
            return;
        }

        chKorInput = inputChar;
        char JM = isJaOrMo(chKorInput);
        char last = curText[curText.Length - 1];

        string newText = "";
        if (isJaOrMo(last) == 'J' && JM == 'J')
        {
            string combo = "" + last + chKorInput;
            int idx = getIndexinArray(Jcombo, combo);
            if (idx != -1)
            {
                newText += Jcombo_index[idx];
                inputField.text = curText.Substring(0, curText.Length - 1) + newText;
                return;
            }
        }

        inputField.text += chKorInput;
    }

    private char isJaOrMo(char c)
    {
        if (c >= 12593 && c <= 12622) return 'J'; // 자음
        else if (c >= 12623 && c <= 12643) return 'M'; // 모음
        return ' ';
    }

    private bool isKorean(char c)
    {
        return (c >= 12593 && c <= 12643) || (c >= 0xAC00 && c <= 0xD7AF);
    }

    private int getIndexinArray<T>(T[] arr, T value)
    {
        for (int i = 0; i < arr.Length; ++i)
            if (EqualityComparer<T>.Default.Equals(arr[i], value))
                return i;
        return -1;
    }

    private void UpdateDisplay()
    {
        inputDisplayText.text = inputField.text;
        inputField.caretPosition = inputField.text.Length;
    }

    private void OnSearch()
    {
        Debug.Log("[검색] " + inputField.text);
        SearchManager.Instance.Search(inputField.text);
        CloseKeyboard();
    }
}

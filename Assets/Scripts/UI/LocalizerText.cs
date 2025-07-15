using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 번역 예외처리용

public class LocalizerText : MonoBehaviour
{
    [SerializeField] string key = string.Empty;
    [SerializeField] Text targetText;
    public void UpdateText()
    {
        if (ShopManager.Instance == null || string.IsNullOrEmpty(key) || targetText == null)
        {
            Debug.LogWarning($"[LocalizerText] 초기화 실패: key 또는 targetText가 비어있거나 ShopManager가 초기화되지 않음.");
            return;
        }

        var language = UIManager.Instance.NowLanguage;
        var entry = ShopManager.Instance.LocalizeTextList.Find(t => t.Key == key);

        if (entry != null)
        {
            targetText.text = entry.Text[(int)language];
        }
        else
        {
            Debug.LogWarning($"[LocalizerText] 키 '{key}'에 해당하는 로컬라이즈 데이터를 찾을 수 없습니다.");
            targetText.text = $"[{key}]";
        }
    }
}

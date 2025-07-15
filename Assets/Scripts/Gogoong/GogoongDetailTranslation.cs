using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GogoongDetailTranslation : MonoBehaviour
{
    [SerializeField]
    private GameObject[] gogoongList;

    private void Awake()
    {
        languageChange();

        // 번역끝나면 페이지 숨기기
        foreach (GameObject gogoong in gogoongList)
        {
            gogoong.SetActive(false);
        }
    }

    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.gogoongDetailSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("gogoongDetail");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray)
        {
            // key와 동일한 이름의 GameObject를 찾음
            GameObject targetObject = GameObject.Find(key);
            if (targetObject != null)
            {
                // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
                    // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
                    textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용      
                }
                else
                {
                    Debug.LogWarning($"TextMeshProUGUI component not found in {key}");
                }
            }
            else
            {
                Debug.LogWarning($"GameObject with name {key} not found");
            }
        }
    }
}

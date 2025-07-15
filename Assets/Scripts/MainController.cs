using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public GameObject globalManagerPrefab;

    // 각 언어 별 선택 시 비 선택 시 이미지
    [SerializeField] private Sprite KRselect;   
    [SerializeField] private Sprite ENselect;   
    [SerializeField] private Sprite JPselect;    
    [SerializeField] private Sprite CNselect;

    [SerializeField] private Image LangImg;

    LanguageService ls;

    public KeyboardManager targetKeyboard; // Reference to keyboard manager

    void Start()
    {

        GameObject gMInstance = Instantiate(globalManagerPrefab);
        gMInstance.GetComponent<GlobalManager>().ReadJsonFile();

        //Debug.LogError($"Kiosk Name : {GlobalManager.Instance.kioskName}");

        
        ls = new LanguageService();
        ls.mainSceneinit();        
        string currentLanguage = LanguageService.getCurrentLanguage();
        LangImg.sprite = currentLanguage == "KR" ? KRselect : currentLanguage == "EN" ? ENselect : currentLanguage == "JP" ? JPselect : CNselect;

        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("main");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray) {
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

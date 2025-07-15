using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GogoongListTranslation : MonoBehaviour
{
    [SerializeField]
    private Sprite qr_preImage;

    private void Start()
    {
        languageChange();
        palaceQRCreate();

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    private void palaceQRCreate()
    {
        // QR 코드 뿌리기
        string address1 = "서울특별시 종로구 사직로 161";
        string palaceNm1 = "경복궁";
        string address2 = "서울특별시 종로구 율곡로 99";
        string palaceNm2 = "창덕궁";
        string address3 = "서울특별시 종로구 창경궁로 185";
        string palaceNm3 = "창경궁";
        string address4 = "서울특별시 중구 세종대로 99";
        string palaceNm4 = "덕수궁";

        
        for (int i = 0; i < 4; i++)
        {
            RawImage qrImage = GameObject.Find($"RawImage{i+1}").GetComponentInChildren<RawImage>();
            qrImage.texture = qr_preImage.texture;
        }

        //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(address1)))}&flag=palace&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(palaceNm1)))}&apn=com.witdiocianapp", GameObject.Find("RawImage1").GetComponentInChildren<RawImage>());
        //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(address2)))}&flag=palace&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(palaceNm2)))}&apn=com.witdiocianapp", GameObject.Find("RawImage2").GetComponentInChildren<RawImage>());
        //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(address3)))}&flag=palace&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(palaceNm3)))}&apn=com.witdiocianapp", GameObject.Find("RawImage3").GetComponentInChildren<RawImage>());
        //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(address4)))}&flag=palace&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(palaceNm4)))}&apn=com.witdiocianapp", GameObject.Find("RawImage4").GetComponentInChildren<RawImage>());
        QRCreator.CreateQR($"https://map.naver.com/p/entry/place/11571707?c=13.00,0,0,0,dh", GameObject.Find("RawImage1").GetComponentInChildren<RawImage>());
        QRCreator.CreateQR($"https://map.naver.com/p/entry/place/12261493?c=15.00,0,0,0,dh", GameObject.Find("RawImage2").GetComponentInChildren<RawImage>());
        QRCreator.CreateQR($"https://map.naver.com/p/entry/place/11571740?c=15.00,0,0,0,dh", GameObject.Find("RawImage3").GetComponentInChildren<RawImage>());
        QRCreator.CreateQR($"https://map.naver.com/p/entry/place/11571730?c=7.00,0,0,0,dh", GameObject.Find("RawImage4").GetComponentInChildren<RawImage>());
    }

    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.gogoongSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("gogoongList");

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

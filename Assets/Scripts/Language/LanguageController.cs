using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class LanguegeController : MonoBehaviour
{

    // 변경언어 프리펩
    [SerializeField] private GameObject languegeItem;
    // 현제언어 프리펩
    [SerializeField] private GameObject currentLanguage;

    // 프리펩 담을 객체
    [SerializeField] private Transform languegeItemContent;
    [SerializeField] private Transform currentLanguegeContent;

    // 각 언어 별 선택 시 비 선택 시 이미지
    [SerializeField] private Sprite KRselect;
    [SerializeField] private Sprite KRnormal;
    [SerializeField] private Sprite ENselect;
    [SerializeField] private Sprite ENnormal;
    [SerializeField] private Sprite JPselect;
    [SerializeField] private Sprite JPnormal;
    [SerializeField] private Sprite CNselect;
    [SerializeField] private Sprite CNnormal;

    // 배경
    [SerializeField] private Sprite LangBackground;
    [SerializeField] private Sprite LangCurrentBackground;

    // 선택, 미선택 이미지
    [SerializeField] private Sprite checkOk;
    [SerializeField] private Sprite checkNo;
    

    LanguageService ls;


    // 생성된 프리팹을 관리할 Dictionary (ID와 GameObject를 매핑)
    private Dictionary<string, GameObject> instantiatedPrefabs = new Dictionary<string, GameObject>();
    Sprite[] langImgnormalArr;
    Sprite[] langImgSelectArr;
    Dictionary<string, string> languages;

    void Start()
    {        
        ls = new LanguageService();

        string currentLanguegeTxt = LanguageService.getCurrentLanguage();

        // 언어 페이지 초기 설정        
        ls.languageSceneInit();

        langImgnormalArr = new Sprite[] { KRnormal, ENnormal, JPnormal, CNnormal };
        langImgSelectArr = new Sprite[] { KRselect, ENselect, JPselect, CNselect};
        languages = new Dictionary<string, string>
        {
            { "KR", "한국어" },
            { "EN", "ENGLISH" },
            { "JP", "日本語" },
            { "CN", "中國語" }
        };    
        string[] languageArray = languages.Values.ToArray();
        string[] languageskeysArray = languages.Keys.ToArray();

        // 현재 언어 뿌려주기
        GameObject obj = Instantiate(currentLanguage, currentLanguegeContent);

        Image[] currentLangueges = obj.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] currentLanguageTxts = obj.GetComponentsInChildren<TextMeshProUGUI>();
                
        foreach (Image curImg in currentLangueges) {            

            if (curImg.name == "CurrentSelectLanguage") {
                curImg.sprite = LangCurrentBackground;
                curImg.rectTransform.anchoredPosition = new Vector2(0f, 910f);
            }
            else if (curImg.name == "LangImg") {
                
                // 현재 언어 텍스트 뿌리기
                switch (currentLanguegeTxt) {
                    case "KR":
                        curImg.sprite = KRselect;
                        break;
                    case "EN":                        
                        curImg.sprite = ENselect;
                        break;
                    case "JP":
                        curImg.sprite = JPselect;
                        break;
                    case "CN":
                        curImg.sprite = CNselect;
                        break;
                }
                curImg.rectTransform.anchoredPosition = new Vector3(-725.7f, 929.58f, 0f);
            }            
        }
        
        currentLanguageTxts[0].text = languages[currentLanguegeTxt]; 
        currentLanguageTxts[0].rectTransform.anchoredPosition = new Vector3(0f, 965f, 0f);

        float verticalSpacing = 400;
        // 나머지 언어 뿌려주기
        for (int i = 0; i < 4; i++)
        {
            GameObject languegeItemCopy = Instantiate(languegeItem, languegeItemContent);
            languegeItemCopy.name = "languegeItem" + (i+1);
            
            Button languegeItemBtn = languegeItemCopy.GetComponentInChildren<Button>();
            languegeItemBtn.onClick.AddListener(languegeUpdate);
            // RectTransform 가져오기
            RectTransform rectTransform = languegeItemCopy.GetComponent<RectTransform>();            

            // 세로 간격을 계산해서 위치를 설정 (부모의 anchoredPosition 기준)
            rectTransform.anchoredPosition = new Vector2(0, -i * verticalSpacing);
            
            // languegeItemCopy 안의 이미지 컴포넌트 불러오기
            Image[] imgs = languegeItemCopy.GetComponentsInChildren<Image>();
            TextMeshProUGUI text = languegeItemCopy.GetComponentInChildren<TextMeshProUGUI>();

            foreach (Image img in imgs)
            {                
                switch (img.name)
                {
                    case "LangItem":
                        img.sprite = LangBackground;
                        break;
                    case "LanguageIcon":
                        img.sprite = langImgnormalArr[i];
                        break;
                    case "LangCheckIcon":
                        img.sprite = checkNo;
                        break;
                }
            }

            text.text = languageArray[i];

            instantiatedPrefabs.Add(languageskeysArray[i], languegeItemCopy);

            Debug.LogError($"Language : {languageskeysArray[i]}\n languegeItemCopy : {languegeItemCopy}");
        }

        translationTxtUpdate();

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    //         언어 수정 이벤트 
    //       { "KR", "한국어" },
    //       { "EN", "ENGLISH" },
    //       { "JP", "日本語" },
    //       { "CN", "中國語" }
    public void languegeUpdate() {
        string[] languageArray = languages.Values.ToArray();
        string[] languageskeysArray = languages.Keys.ToArray();
        GameObject languageObj = new GameObject();
        // EventSystem을 통해 클릭된 GameObject를 가져옵니다.
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI text = clickedObject.GetComponentInChildren<TextMeshProUGUI>();

        int index = 0;
        switch (text.text) {
            case "한국어":
                index = 0;
                text.text = languageArray[0];
                languageObj = instantiatedPrefabs["KR"];
                break;
            case "ENGLISH":
                index = 1;
                text.text = languageArray[1];
                languageObj = instantiatedPrefabs["EN"];
                break;
            case "日本語":
                index = 2;
                text.text = languageArray[2];
                languageObj = instantiatedPrefabs["JP"];
                break;
            case "中國語":
                index = 3;
                text.text = languageArray[3];
                languageObj = instantiatedPrefabs["CN"];
                break;
        }

        
        // 현재 언어의 인덱스를 찾았으니 저장
        PlayerPrefs.SetString("language", languageskeysArray[index]);
        
        Image[] imgs = languageObj.GetComponentsInChildren<Image>();

        foreach (Image img in imgs)
        {
            switch (img.name)
            {
                case "LangItem":
                    img.sprite = LangCurrentBackground;
                    break;
                case "LanguegeIcon":
                    img.sprite = langImgSelectArr[index];
                    break;
                case "LangCheckIcon":
                    img.sprite = checkOk;
                    break;
            }
        }
        
        for (int i = 0; i < 4; i++) {
            if (i == index) continue;
            
            GameObject instantiatedPrefab = instantiatedPrefabs[languageskeysArray[i]];            

            Image[] prefabChindImgs = instantiatedPrefab.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] prefabChildTxt = instantiatedPrefab.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (Image img in prefabChindImgs)
            {
                switch (img.name)
                {
                    case "LangItem":
                        img.sprite = LangBackground;
                        break;
                    case "LanguegeIcon":
                        img.sprite = langImgnormalArr[i];
                        break;
                    case "LangCheckIcon":
                        img.sprite = checkNo;
                        break;
                }
            }

            prefabChildTxt[0].text = languageArray[i];
        }

        // 현재 고른 언어 바꾸기
        GameObject currentLanguegeObject = GameObject.Find("CurrentLanguege(Clone)");
        Image[] currentLangueges = currentLanguegeObject.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] currentLanguageTxts = currentLanguegeObject.GetComponentsInChildren<TextMeshProUGUI>();
        string currentLanguage = LanguageService.getCurrentLanguage();
        foreach (Image curImg in currentLangueges)
        {

            if (curImg.name == "CurrentSelectLanguage")
            {
                curImg.sprite = LangCurrentBackground;
                curImg.rectTransform.anchoredPosition = new Vector2(0f, 910f);
            }
            else if (curImg.name == "LangImg")
            {

                // 현재 언어 텍스트 뿌리기
                switch (currentLanguage)
                {
                    case "KR":
                        curImg.sprite = KRselect;
                        break;
                    case "EN":
                        curImg.sprite = ENselect;
                        break;
                    case "JP":
                        curImg.sprite = JPselect;
                        break;
                    case "CN":
                        curImg.sprite = CNselect;
                        break;
                }
                curImg.rectTransform.anchoredPosition = new Vector3(-725.7f, 929.58f, 0f);
            }
        }

        currentLanguageTxts[0].text = languages[currentLanguage];         
        currentLanguageTxts[0].rectTransform.anchoredPosition = new Vector3(0f, 965f, 0f);


        // 영상 제어 
        VideoController videoController = GameObject.FindObjectOfType<VideoController>();        
        videoController.OnChangeVideo("2", "language", languageskeysArray[index]);
        
        // 업데이트 
        translationTxtUpdate();
    }

    // 
    private void translationTxtUpdate()
    {
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("language");

        string[] translation = res.Keys.ToArray();

        for (int i = 0; i < translation.Length; i++)
        {
            GameObject findObj = GameObject.Find(translation[i]);
            TextMeshProUGUI translationTxt = findObj.GetComponent<TextMeshProUGUI>();

            translationTxt.text = res[translation[i]][currentLanguage];
        }
    }

}
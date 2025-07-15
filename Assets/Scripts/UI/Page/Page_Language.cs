using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Page_Language : MonoBehaviour
{
    [SerializeField] LanguageContent curLanguageContent;
    [SerializeField] LanguageContent[] languageContents = new LanguageContent[4];

    public void Init()
    {
        curLanguageContent = transform.Find("CurrentLanguege").GetComponent<LanguageContent>();

        for (int i = 0; i < languageContents.Length; ++i)
        {
            int index = i; // 클로저 캡처 방지용 지역 변수

            languageContents[i] = transform.Find("Body").GetChild(i).GetComponent<LanguageContent>();
            languageContents[i].myLanguage = (Language)index;
            languageContents[i].Init();
            languageContents[index].button.onClick.AddListener(() => OnButton(index));
        }
    }

    void OnButton(int index)
    {
        UIManager.Instance.NowLanguage = languageContents[index].myLanguage;

        foreach (var content in languageContents)
        {
            if (content.myLanguage == UIManager.Instance.NowLanguage)
            {
                content.Selected();
                curLanguageContent.Copy(content);
            }
            else
            {
                content.Unselected();
            }
        }
    }
}

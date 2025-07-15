using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageImageUpdate : MonoBehaviour
{
    // 언어별 이미지
    [SerializeField]
    private Sprite koImage;
    [SerializeField]
    private Sprite enImage;
    [SerializeField]
    private Sprite jaImage;
    [SerializeField]
    private Sprite zhImage;

    private void Start()
    {
        Image image = GetComponent<Image>();
        string language = LanguageService.getCurrentLanguage();

        switch (language) 
        {
            case "KR":
                image.sprite = koImage;
                break;
            case "EN":
                image.sprite = enImage;
                break;
            case "JP":
                image.sprite = jaImage;
                break;
            case "CN":
                image.sprite = zhImage;
                break;
            default:
                break;
        }
    }
}

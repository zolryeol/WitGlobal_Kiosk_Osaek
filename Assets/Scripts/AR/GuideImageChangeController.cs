using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideImageChangeController : MonoBehaviour
{
    // 언어별 가이드 이미지
    [SerializeField]
    private Sprite GuideManKo;
    [SerializeField]
    private Sprite GuideManEn;
    [SerializeField]
    private Sprite GuideManJa;
    [SerializeField]
    private Sprite GuideManZh;

    [SerializeField]
    private Sprite GuideWomanKo;
    [SerializeField]
    private Sprite GuideWomanEn;
    [SerializeField]
    private Sprite GuideWomanJa;
    [SerializeField]
    private Sprite GuideWomanZh;

    // Start is called before the first frame update
    void Start()
    {
        Image image = GetComponent<Image>();
        int HanbokIndex = PlayerPrefs.GetInt("testHanbokIndex");
        string language = LanguageService.getCurrentLanguage();

        /* 베트남 API Check 필수 
        * 여자한복 4 남자한복 1 궁중한복 3 인사 2
        * index 1 : 여자 
        * index 2 : 여자 
        * index 3 : 여자 
        * index 4 : 남자 
        * index 5 : 여자 
        * index 6 : 남자 
        * index 7 : 여자 
        * index 8 : 여자 
        * index 9 : 여자 
        * index 10 : 여자
        */

        // 남자
        if (HanbokIndex == 4 || HanbokIndex == 6)
        {
            switch (language)
            {
                case "KR":
                    image.sprite = GuideManKo;
                    break;
                case "EN":
                    image.sprite = GuideManEn;
                    break;
                case "JP":
                    image.sprite = GuideManJa;
                    break;
                case "CN":
                    image.sprite = GuideManZh;
                    break;
                default:
                    break;
            }
        }
        // 여자
        else
        {
            switch (language)
            {
                case "KR":
                    image.sprite = GuideWomanKo;
                    break;
                case "EN":
                    image.sprite = GuideWomanEn;
                    break;
                case "JP":
                    image.sprite = GuideWomanJa;
                    break;
                case "CN":
                    image.sprite = GuideWomanZh;
                    break;
                default:
                    break;
            }
        }

        
    }


    }

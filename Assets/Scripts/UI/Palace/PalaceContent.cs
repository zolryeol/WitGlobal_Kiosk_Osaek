using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PalaceContent : MonoBehaviour
{
    PalaceContentFetcher fetcher;

    public int Num;
    public TextMeshProUGUI PalaceName;
    public TextMeshProUGUI PalaceAddress;
    public TextMeshProUGUI PalaceHashTag;
    public TextMeshProUGUI PalaceOpenTime;
    public TextMeshProUGUI PalaceContactNum;
    public Image ThumbnailImage;

    public PalaceData Data;

    public Button button;

    public void Init(PalaceContentFetcher _fetcher)
    {
        button = GetComponent<Button>();
        fetcher = _fetcher;

        button.onClick.AddListener(() => fetcher.OpenPalaceDetail());
        button.onClick.AddListener(() => fetcher.palaceDetail.FetchDetail(this.Data));

        UIManager.Instance.ChangeLanguageEvent += () => FetchContent(Data);
    }

    public void FetchContent(PalaceData palaceData)
    {
        Data = palaceData;

        var nowLang = (int)UIManager.Instance.NowLanguage;

        Num = palaceData.Num;
        if (PalaceName != null)
            PalaceName.text = Data.PalaceNameString[nowLang];
        if (PalaceAddress != null)
            PalaceAddress.text = Data.PalaceAddressString[nowLang];
        if (PalaceHashTag != null)
            PalaceHashTag.text = Data.HashTagString[nowLang];
        if (PalaceOpenTime != null)
            PalaceOpenTime.text = Data.OpeningTime[nowLang];
        if (PalaceContactNum != null)
            PalaceContactNum.text = Data.ContactNum;

        ThumbnailImage.sprite = ResourceManager.Instance.PalaceSpritesDic[Num][0];
    }
}

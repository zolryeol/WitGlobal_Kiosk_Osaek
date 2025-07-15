using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PalaceContentDetail : PalaceContent
{
    public TextMeshProUGUI PalaceDescription;
    public TextMeshProUGUI Attraction;
    public TextMeshProUGUI Fee;

    public Image[] Image = new Image[4];

    public RawImage QRImage;
    public void FetchDetail(PalaceData _palaceData)
    {
        var uimgNowLanguage = (int)UIManager.Instance.NowLanguage;

        Num = _palaceData.Num;

        if (PalaceName != null)
            PalaceName.text = _palaceData.PalaceNameString[uimgNowLanguage];
        if (PalaceAddress != null)
            this.PalaceAddress.text = _palaceData.PalaceAddressString[uimgNowLanguage];
        if (PalaceHashTag != null)
            this.PalaceHashTag.text = _palaceData.HashTagString[uimgNowLanguage];
        if (PalaceOpenTime != null)
            this.PalaceOpenTime.text = _palaceData.OpeningTime[uimgNowLanguage];
        if (PalaceContactNum != null)
            PalaceContactNum.text = _palaceData.ContactNum;
        if (ThumbnailImage != null)
            this.ThumbnailImage.sprite = ResourceManager.Instance.PalaceSpritesDic[Num][0];

        if (PalaceDescription != null)
            PalaceDescription.text = _palaceData.DescriptionString[uimgNowLanguage];
        if (Attraction != null)
            Attraction.text = _palaceData.Attraction[uimgNowLanguage];
        if (Fee != null)
            Fee.text = _palaceData.Fee[uimgNowLanguage];

        for (int i = 0; i < Image.Length; ++i)
        {
            Image[i].sprite = ResourceManager.Instance.PalaceSpritesDic[Num][i + 1];
        }
    }
}

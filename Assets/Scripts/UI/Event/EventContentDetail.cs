using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventContentDetail : EventContent
{
    [SerializeField]
    private TextMeshProUGUI eventAge;
    [SerializeField]
    private TextMeshProUGUI eventFee;
    [SerializeField]
    private TextMeshProUGUI eventDescription;


    public override void FetchContent(EventData data)
    {
        var nowLanguage = (int)UIManager.Instance.NowLanguage;

        base.FetchContent(data);

        eventAge.text = data.Age[nowLanguage].ToString();
        eventFee.text = data.Fee[nowLanguage].ToString();
        eventDescription.text = data.DescriptionString[nowLanguage].ToString();


        if (string.IsNullOrEmpty(data.QRImageUrl))
        {
            qrCodeImage.texture = CommonFunction.ConvertSpriteToTexture(ResourceManager.Instance.NoQRImage);
        }
        else
        {
            qrCodeImage.texture = CommonFunction.GenerateQRCode(data.QRImageUrl);
        }
    }



}

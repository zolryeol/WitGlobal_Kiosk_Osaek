using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopContentDetail : ShopContent
{
    [SerializeField] TextMeshProUGUI shopDescripotion;
    [SerializeField] TextMeshProUGUI baseCategory;
    [SerializeField] TextMeshProUGUI secondCategory;
    [SerializeField] TextMeshProUGUI aiCategory;
    [SerializeField] TextMeshProUGUI openingTime;
    [SerializeField] TextMeshProUGUI contactNumber;

    [SerializeField] RawImage qrCodeImage;

    public override void FetchContent(BaseShopInfoData data)
    {
        var nowLanguage = UIManager.Instance.NowLanguage;
        base.FetchContent(data);

        //shopName.text = data.ShopName[(int)nowLanguage];
        //address.text = data.Address[(int)nowLanguage];
        //hashTag.text = data.HashTag[(int)nowLanguage];


        shopDescripotion.text = data.ShopDescription[(int)nowLanguage];
        openingTime.text = data.OpeningTime;
        contactNumber.text = data.ContactNumber;

        InsertSprite(data);
        //baseCategory.text = data.BaseCategoryString[(int)nowLanguage];
        //secondCategory.text = data.SecondCategoryString[(int)nowLanguage];
        //aiCategory.text = data.AICategoryString[(int)nowLanguage];

        if (string.IsNullOrEmpty(data.NaverLink))
        {
            qrCodeImage.texture = CommonFunction.ConvertSpriteToTexture(ResourceManager.Instance.noQRImage);
        }
        else
        {
            qrCodeImage.texture = CommonFunction.GenerateQRCode(data.NaverLink);
        }
    }

}

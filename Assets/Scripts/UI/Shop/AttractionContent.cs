using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttractionContent : MarketContent
{
    [SerializeField] TextMeshProUGUI areaName;
    AttractionData attractionData;

    public void FetchContent(AttractionData data)
    {
        var nowLanguage = UIManager.Instance.NowLanguage;

        attractionData = data;

        shopName.text = data.ShopName[(int)nowLanguage];
        shopAddress.text = data.Address[(int)nowLanguage];
        description.text = data.ShopDescription[(int)nowLanguage];
        hashTag.text = data.HashTag[(int)nowLanguage];
        naverRating = data.NaverRating;
        InsertSprite(data);
        areaName.text = data.BaseCategoryString[(int)nowLanguage] + ">" + data.SecondCategoryString[(int)nowLanguage];
    }
}

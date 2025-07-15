using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AICourseInfo : MonoBehaviour
{
    public TextMeshProUGUI ShopSecondCategory;
    public TextMeshProUGUI ShopName;
    public Image ShopImage;
    public TextMeshProUGUI HashTag;

    public void CopyContent(AICourseInfo _ori)
    {
        ShopSecondCategory.text = _ori.ShopSecondCategory.text;
        ShopName.text = _ori.ShopName.text;
        HashTag.text = _ori.HashTag.text;
        if (_ori.ShopImage.sprite != null)
        {
            ShopImage.sprite = _ori.ShopImage.sprite;
        }
        else
        {
            ShopImage.sprite = PrefabManager.Instance.NoImageSprite;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopContent : MonoBehaviour
{
    [SerializeField] protected int shopID;
    [SerializeField] protected TextMeshProUGUI ID;
    [SerializeField] protected TextMeshProUGUI shopName;
    [SerializeField] protected TextMeshProUGUI shopAddress;
    [SerializeField] protected TextMeshProUGUI description;
    [SerializeField] protected TextMeshProUGUI hashTag;
    [SerializeField] protected Image[] sprite = new Image[6];
    [SerializeField] protected float naverRating;
    BaseShopInfoData baseShopInfoData;

    public virtual void Awake()
    {
        if (TryGetComponent(out Button btn))
        {
            btn.onClick.AddListener(() =>
            {
                UIManager.Instance.OnDetailPage();
                UIManager.Instance.DetailPage.GetComponent<ShopContentDetail>().FetchContent(baseShopInfoData);
                UIManager.Instance.PlayVideoByDetail();
            }
            );
        }
    }

    public virtual void FetchContent(BaseShopInfoData data)
    {
        var nowLanguage = UIManager.Instance.NowLanguage;

        baseShopInfoData = data;

        shopName.text = data.ShopName[(int)nowLanguage];
        shopAddress.text = data.Address[(int)nowLanguage];
        description.text = data.ShopDescription[(int)nowLanguage];
        hashTag.text = data.HashTag[(int)nowLanguage];
        naverRating = data.NaverRating;
        InsertSprite(data);
    }

    public virtual void InsertSprite(BaseShopInfoData data)
    {
        int minLength = Mathf.Min(sprite.Length, data.spriteImage.Length);
        for (int i = 0; i < minLength; i++)
        {
            if (data.spriteImage[i] == null)
            {
                sprite[i].sprite = PrefabManager.Instance.NoImageSprite;
                continue;
            }
            sprite[i].sprite = data.spriteImage[i];
        }
        // 남은 sprite 슬롯은 NoImageSprite로 초기화
        for (int i = minLength; i < sprite.Length; i++)
        {
            sprite[i].sprite = PrefabManager.Instance.NoImageSprite;
        }
    }
}

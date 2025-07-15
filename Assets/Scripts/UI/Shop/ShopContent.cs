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
    [SerializeField] protected TextMeshProUGUI address;
    [SerializeField] protected TextMeshProUGUI hashTag;
    [SerializeField] protected Image[] sprite = new Image[6];

    BaseShopInfoData baseShopInfoData;

    private void Awake()
    {
        if (TryGetComponent(out Button btn))
        {
            btn.onClick.AddListener(() =>
            {
                UIManager.Instance.OnDetailPage();
                UIManager.Instance.DetailPage.GetComponent<ShopContentDetail>().FetchContent(baseShopInfoData);
            }
            );
        }
    }

    public virtual void FetchContent(BaseShopInfoData data)
    {
        var nowLanguage = UIManager.Instance.NowLanguage;

        baseShopInfoData = data;

        shopName.text = data.ShopName[(int)nowLanguage];
        address.text = data.Address[(int)nowLanguage];
        hashTag.text = data.HashTag[(int)nowLanguage];

        InsertSprite(data);
    }

    public void InsertSprite(BaseShopInfoData data)
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

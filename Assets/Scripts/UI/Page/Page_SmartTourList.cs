using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
///  스마트 투어 리스트 페이지
///  기존 ShopList와 통합하려했으나 추후 예외가 생길것으로 예상되어 따로 분리
/// </summary>

public class Page_SmartTourList : MonoBehaviour
{
    [SerializeField] SecondCategoryButton[] secondCategoryButtons = new SecondCategoryButton[3];
    Transform body;
    Transform contentParent;
    public List<MarketContent> TraditionalMarketContentList = new(); /// 전통시장전용리스트

    public void Init()
    {
        body = CommonFunction.FindDeepChild(this.gameObject, "Body").transform;
        var buttonParent = CommonFunction.FindDeepChild(body.gameObject, "ButtonsParent").transform;
        secondCategoryButtons[0] = buttonParent.GetChild(0).GetComponent<SecondCategoryButton>();
        secondCategoryButtons[1] = buttonParent.GetChild(1).GetComponent<SecondCategoryButton>();
        secondCategoryButtons[2] = buttonParent.GetChild(2).GetComponent<SecondCategoryButton>();

        contentParent = CommonFunction.FindDeepChild(body.gameObject, "Content").transform;

        CreateContentInstance();
    }

    public void CreateContentInstance()
    {
        int maxValue = new[]
{
    LoadManager.Instance.TraditionalMarketList.Count,
    LoadManager.Instance.AttractionList.Count,
    LoadManager.Instance.ServiceAreaList.Count
        }.Max();

        var childCount = contentParent.childCount;

        var addedCount = maxValue - childCount;

        for (int i = 0; i < addedCount; i++)
        {
            var content = Instantiate(PrefabManager.Instance.ContentItemInfoMarketPrafab, contentParent);
            content.SetActive(false);
            TraditionalMarketContentList.Add(content.GetComponent<MarketContent>());
        }

        Debug.Log($"<color=blue>스마트관광 콘텐츠 {maxValue} 개 인스턴싱</color>");
    }

    public void FetchingContent(string districtName) // 지역으로 바꾸어야함;
    {
        var markets = LoadManager.Instance.TraditionalMarketList;

        var firstContents = markets
            .Where(market => market.SecondCategoryString[(int)Language.Korean] == districtName).ToList();


        List<TraditionalMarketData> secondContents = new();

        if (firstContents.Count == 0)
        {
            firstContents = markets
                .Where(market => market.BaseCategoryString[(int)Language.Korean] == UIManager.Instance.NowSelectedKoreaMapName)
                .ToList();
        }
        else
        {
            secondContents = markets
                .Where(market => market.BaseCategoryString[(int)Language.Korean] == firstContents[0].BaseCategoryString[(int)Language.Korean])
                .Except(firstContents)
                .ToList();
        }

        if (firstContents.Count == 0)
        {
            Debug.Log("콘텐츠가 없다...");
        }

        var finalList = firstContents.Concat(secondContents).ToList();


        Debug.Log("콘텐츠 갯수 = " + finalList.Count);


        for (int i = 0; i < TraditionalMarketContentList.Count; ++i)
        {
            if (i < finalList.Count)
            {
                TraditionalMarketContentList[i].gameObject.SetActive(true);
                TraditionalMarketContentList[i].FetchContent(finalList[i]);
            }
            else
            {
                TraditionalMarketContentList[i].gameObject.SetActive(false);
            }
        }

        //InitScrollbarValue(FoodAndShopScrollbar);
    }

}

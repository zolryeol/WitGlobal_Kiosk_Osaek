using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///  스마트 투어 리스트 페이지
///  기존 ShopList와 통합하려했으나 추후 예외가 생길것으로 예상되어 따로 분리
/// </summary>

public class Page_SmartTourList : MonoBehaviour
{
    public List<SecondCategoryButton> SecondCategoryButtons = new();
    Transform body;
    Transform contentParent;

    public List<ShopContent> SmartTourContentList = new();
    public void Init()
    {
        body = CommonFunction.FindDeepChild(this.gameObject, "Body").transform;
        var buttonParent = CommonFunction.FindDeepChild(body.gameObject, "ButtonsParent").transform;

        for (int i = 0; i < 5; i++)
        {
            var b = buttonParent.GetChild(i).GetComponent<SecondCategoryButton>();
            b.SecondCategoryButtonIndex = i;
            SecondCategoryButtons.Add(b);
            b.onClick.AddListener(() => FetchingContent(UIManager.Instance.NowSelectedKoreaMapName, b.SecondCategoryButtonIndex));
            UIManager.Instance.SecondCategorieButtons.Add(b);
        }
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
            var content = Instantiate(PrefabManager.Instance.ContentItemInfoSmartTourPrafab, contentParent);
            content.SetActive(false);
            SmartTourContentList.Add(content.GetComponent<ShopContent>());
        }

        Debug.Log($"<color=blue>스마트관광 콘텐츠 {maxValue} 개 인스턴싱</color>");
    }

    public void FetchingContent(string districtName) // 지역으로 바꾸어야함;
    {

        var um = UIManager.Instance.NowSelectedETC;

        IEnumerable<BaseShopInfoData> _target = Enumerable.Empty<BaseShopInfoData>();

        switch (um)
        {
            case Category_ETC.TraditionalMarket:
                _target = LoadManager.Instance.TraditionalMarketList;
                break;
            case Category_ETC.Attraction:
                _target = LoadManager.Instance.AttractionList;
                break;
            case Category_ETC.ServiceArea:
                _target = LoadManager.Instance.ServiceAreaList;
                break;
            default:
                Debug.LogError("스마트투어 리스트 페이지에서 카테고리 에러");
                break;
        }

        var firstContents = _target
            .Where(t => t.SecondCategoryString[(int)Language.Korean] == districtName).ToList();


        List<BaseShopInfoData> secondContents = new();

        if (firstContents.Count == 0)
        {
            firstContents = _target
                .Where(t => t.BaseCategoryString[(int)Language.Korean] == UIManager.Instance.NowSelectedKoreaMapName)
                .ToList();
        }
        else
        {
            secondContents = _target
                .Where(t => t.BaseCategoryString[(int)Language.Korean] == firstContents[0].BaseCategoryString[(int)Language.Korean])
                .Except(firstContents)
                .ToList();
        }

        if (firstContents.Count == 0)
        {
            Debug.Log("콘텐츠가 없다...");
        }

        var finalList = firstContents.Concat(secondContents).ToList();


        Debug.Log("콘텐츠 갯수 = " + finalList.Count);


        for (int i = 0; i < SmartTourContentList.Count; ++i)
        {
            if (i < finalList.Count)
            {
                SmartTourContentList[i].gameObject.SetActive(true);
                SmartTourContentList[i].FetchContent(finalList[i]);
            }
            else
            {
                SmartTourContentList[i].gameObject.SetActive(false);
            }
        }

        //InitScrollbarValue(FoodAndShopScrollbar);
    }
    public void FetchingContent(string districtName, int categoryIndex) // 지역으로 바꾸어야함;
    {
        var _target = LoadManager.Instance.AttractionList;

        var firstFilter = _target
    .Where(t => t.SecondCategoryString[(int)Language.Korean] == districtName).ToList();

        var _targetContents = firstFilter
        .Where(t =>
        {
            string str = t.Category[(int)Language.Korean];
            int idx = str.IndexOf('_');
            string prefix = (idx >= 0) ? str.Substring(0, idx) : str;
            return prefix == (categoryIndex + 1).ToString();
        })
        .ToList();

        for (int i = 0; i < SmartTourContentList.Count; ++i)
        {
            if (i < _targetContents.Count)
            {
                SmartTourContentList[i].gameObject.SetActive(true);
                SmartTourContentList[i].FetchContent(_targetContents[i]);
            }
            else
            {
                SmartTourContentList[i].gameObject.SetActive(false);
            }
        }

        //InitScrollbarValue(FoodAndShopScrollbar);
    }
}

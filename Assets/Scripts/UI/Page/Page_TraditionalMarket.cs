using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Page_TraditionalMarket : MonoBehaviour
{
    [SerializeField] Transform buttonParent;
    [SerializeField] Transform contentParent;

    BackButton backButton;
    HomeButton homeButton;

    [SerializeField] List<Button> categoryButtonList = new();

    public List<EventContent> contentObjList = new();

    [SerializeField] Scrollbar scrollbar;

    int maxCountentCount;

    Page_SmartTourList page_SmartTourList;
    public void Init()
    {
        // 예외처리용
        var mainButton = GameObject.Find("TraditionalMarket").GetComponent<Button>();
        mainButton.onClick.AddListener(() => Select(0));
        mainButton.onClick.AddListener(() => FetchingContent(0));

        for (int i = 0; i < buttonParent.childCount; ++i)
        {
            var _button = buttonParent.GetChild(i).GetComponent<Button>();

            int index = i;

            _button.onClick.AddListener(() => Select(index));
            _button.onClick.AddListener(() => FetchingContent(index));
            categoryButtonList.Add(_button);
        }

        page_SmartTourList = FindObjectOfType<Page_SmartTourList>();

        backButton = GetComponentInChildren<BackButton>();
        homeButton = GetComponentInChildren<HomeButton>();
    }

    public void FetchingContent(int buttonIndex) // 지역으로 바꾸어야함;
    {
        var markets = LoadManager.Instance.TraditionalMarketList;

        var firstContents = markets
            .Where(market => market.SecondCategoryString[(int)Language.Korean] == JsonLoader.Config.MarketName).ToList();


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

    void Select(int _index)
    {
        Debug.Log("셀렉트 테스트");

        UnSelect();

        CommonFunction.ChangeColorBtnAndTxt(categoryButtonList[_index].transform);

        //categoryButtonList[_index].GetComponent<Image>().color = UIColorPalette.SelectedColor;
        //categoryButtonList[_index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.SelectedTextColor;
    }
    void UnSelect()
    {
        foreach (var c in categoryButtonList)
        {
            CommonFunction.ChangeColorBtnAndTxt(c.transform, false);

            //c.GetComponent<Image>().color = UIColorPalette.NormalColor;
            //c.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.NormalTextColor;
        }
    }

}

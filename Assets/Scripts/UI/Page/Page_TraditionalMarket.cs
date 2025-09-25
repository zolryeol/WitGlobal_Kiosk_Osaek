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

    public List<MarketContent> TraditionalMarketContentList = new(); /// 전통시장전용리스트

    [SerializeField] Scrollbar scrollbar;

    Page_SmartTourList page_SmartTourList;
    public void Init()
    {
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

        CreateContentInstance();
    }

    public void CreateContentInstance()
    {
        int maxValue = LoadManager.Instance.TraditionalMarketList.Count;

        var childCount = contentParent.childCount;

        var addedCount = maxValue - childCount;

        for (int i = 0; i < addedCount; i++)
        {
            var content = Instantiate(PrefabManager.Instance.ContentItemInfoSmartTourPrafab, contentParent);
            content.SetActive(false);
            TraditionalMarketContentList.Add(content.GetComponent<MarketContent>());
        }
    }

    public void FetchingContent(int _index) // 지역으로 바꾸어야함;
    {
        var markets = LoadManager.Instance.TraditionalMarketList;

        string provinceStr = IndexToProvinceString(_index);

        var marketDatas = markets
         .Where(market => market.BaseCategoryString[(int)Language.Korean] == provinceStr)
         .ToList();

        if (JsonLoader.Config.Province == provinceStr)
        {
            var thisMarket = markets
                .FirstOrDefault(market => market.ShopName[(int)Language.Korean] == JsonLoader.Config.MarketName);

            if (thisMarket != null &&
                thisMarket.BaseCategoryString[(int)Language.Korean] == provinceStr) // ✅ provinceStr 범주 포함 조건
            {
                // 중복 방지 후 맨 앞에 삽입
                marketDatas.Remove(thisMarket);
                marketDatas.Insert(0, thisMarket);
            }
        }

        for (int i = 0; i < TraditionalMarketContentList.Count; ++i)
        {
            if (i < marketDatas.Count)
            {
                TraditionalMarketContentList[i].gameObject.SetActive(true);
                TraditionalMarketContentList[i].FetchContent(marketDatas[i]);
            }
            else
            {
                TraditionalMarketContentList[i].gameObject.SetActive(false);
            }
        }

        UIManager.Instance.InitScrollbarValue(scrollbar);
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

    string IndexToProvinceString(int _index)
    {
        return _index switch
        {
            0 => "경기도",
            1 => "강원도",
            2 => "충청남도",
            3 => "충청북도",
            4 => "경상남도",
            5 => "경상북도",
            6 => "전라남도",
            7 => "전라북도",
            8 => "서울",
            _ => "",
        };
    }
}

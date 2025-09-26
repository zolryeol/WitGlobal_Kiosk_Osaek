using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 이미지 영역 테스트 
// 투명하지 않은 부분만 클릭 가능하도록 설정

public class KoreaMapButton : MonoBehaviour
{
    Image mapImage;
    Button button;
    [SerializeField] KoreaMapType koreaMapType;
    string mapName;
    public void Init()
    {
        button = GetComponent<Button>();
        mapImage = GetComponent<Image>();

        mapName = this.gameObject.name;

        if (mapImage == null)
        {
            Debug.LogError("Image 컴포넌트가 없습니다.");
            return;
        }

        mapImage.alphaHitTestMinimumThreshold = 0.1f; // 투명도 10% 이상만 클릭 허용

        button.onClick.AddListener(() => Debug.Log($"{this.gameObject.name} 클릭"));

        if (koreaMapType == KoreaMapType.Province)
        {
            button.onClick.AddListener(OnButton);
        }
        else if (koreaMapType == KoreaMapType.District)
        {
            button.onClick.AddListener(OnButtonDistrict);
        }
    }

    void OnButton()
    {
        var um = UIManager.Instance;

        um.NowSelectedKoreaMapName = this.mapName;

        if (um.NowSelectedETC == Category_ETC.ServiceArea) // 휴게소  (바로 페이지)
        {
            um.OpenPage(um.SmartTourListPage);
            um.Page_SmartTourList.OnCategoryButton(false);
            um.Page_SmartTourList.FetchingContent(mapName);
        }
        else if (um.NowSelectedETC == Category_ETC.TraditionalMarket)// 전통시장 
        {
            um.OpenPage(um.KoreaMapDetailPage);
            um.Page_KoreaMapDetail.OnMap(mapName);
        }
        else if (um.NowSelectedETC == Category_ETC.Attraction) // 관광지
        {
            um.OpenPage(um.KoreaMapDetailPage);
            um.Page_KoreaMapDetail.OnMap(mapName);

        }
    }
    void OnButtonDistrict()
    {
        var um = UIManager.Instance;

        um.NowSelectedKoreaMapName = this.mapName;

        um.OpenPage(um.SmartTourListPage);

        if (um.NowSelectedETC == Category_ETC.Attraction) // 관광지
        {
            um.SelectFirstCategory(um.Page_SmartTourList.SecondCategoryButtons); // 첫번째 카테고리 선택
            um.Page_SmartTourList.FetchingContent(mapName, 0 + 1);
        }
        else
            um.Page_SmartTourList.FetchingContent(mapName);
    }
}

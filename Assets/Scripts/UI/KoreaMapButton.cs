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
    Page_KoreaMapDetail pageKoreaMapDetail;
    [SerializeField] KoreaMapType koreaMapType;
    string mapName;
    Page_SmartTourList pageSmartTourList;
    public void Init()
    {
        button = GetComponent<Button>();
        mapImage = GetComponent<Image>();

        mapName = this.gameObject.name;

        pageSmartTourList = FindObjectOfType<Page_SmartTourList>();

        if (mapImage == null)
        {
            Debug.LogError("Image 컴포넌트가 없습니다.");
            return;
        }

        mapImage.alphaHitTestMinimumThreshold = 0.1f; // 투명도 10% 이상만 클릭 허용

        button.onClick.AddListener(() => Debug.Log($"{this.gameObject.name} 클릭"));

        switch (koreaMapType)
        {
            case KoreaMapType.Province:
                InitProvinceType();
                break;
            case KoreaMapType.District:
                InitDistrictType();
                break;
            default:
                Debug.LogWarning($"KoreaMapType이 설정되지 않았습니다: {koreaMapType}");
                break;
        }
    }

    void InitProvinceType()
    {
        var um = UIManager.Instance;

        pageKoreaMapDetail = FindObjectOfType<Page_KoreaMapDetail>();
        button.onClick.AddListener(() => um.OpenPage(um.KoreaMapDetailPage));
        button.onClick.AddListener(() => pageKoreaMapDetail.OnMap(mapName));
        button.onClick.AddListener(() => um.NowSelectedKoreaMapName = mapName);
    }

    void InitDistrictType()
    {
        var um = UIManager.Instance;

        button.onClick.AddListener(() => um.OpenPage(um.SmartTourListPage));
        button.onClick.AddListener(() => pageSmartTourList.FetchingContent(mapName));
        if (LoadManager.Instance.TraditionalMarketList.
            Any(t => t.SecondCategoryString[(int)Language.Korean] == mapName && t.isSetup == true))
        {
            this.mapImage.color = Color.red;
        }
    }


    void CheckNowSelectMode()
    {
        var um = UIManager.Instance;

        if (um.NowSelectedETC == Category_ETC.ServiceArea)
        {
            um.OpenPage(um.SmartTourListPage);
        }
        else
        {

        }
    }

}

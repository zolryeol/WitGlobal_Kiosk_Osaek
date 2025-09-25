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

        if (um.NowSelectedETC == Category_ETC.ServiceArea) // 휴게소인경우 바로 바로 페이지
        {
            um.OpenPage(um.SmartTourListPage);
            um.Page_SmartTourList.FetchingContent(mapName);
        }
        else// 전통시장 혹은 관광지 인경우 상세 지도로 이동
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
        um.Page_SmartTourList.FetchingContent(mapName);
    }
}

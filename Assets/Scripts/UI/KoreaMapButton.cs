using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 이미지 영역 테스트 
// 투명하지 않은 부분만 클릭 가능하도록 설정

public class KoreaMapButton : MonoBehaviour
{
    Image mapImage;
    Button button;
    Page_KoreaMapDetail pageKoreaMapDetail;
    private void Awake()
    {
        mapImage = GetComponent<Image>();
        button = GetComponent<Button>();
        pageKoreaMapDetail = FindObjectOfType<Page_KoreaMapDetail>();
    }

    public void Init()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            mapImage.alphaHitTestMinimumThreshold = 0.1f; // 투명도 10% 이상만 클릭 허용
        }

        button.onClick.AddListener(() => Debug.Log($"{this.gameObject.name} 클릭"));
        button.onClick.AddListener(() => UIManager.Instance.OpenPage(UIManager.Instance.KoreaMapDetailPage));
        button.onClick.AddListener(() => pageKoreaMapDetail.OnMap(this.gameObject.name));
    }
}

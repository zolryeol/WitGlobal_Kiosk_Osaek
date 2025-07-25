using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIContentBigButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] AICourseInfo courseInfo;
    [SerializeField] ShopContentDetail ShopContentDetail;
    public void Init()
    {
        button = GetComponent<Button>();
        courseInfo = GetComponent<AICourseInfo>();
        ShopContentDetail = FindObjectOfType<ShopContentDetail>();

        button.onClick.AddListener(UIManager.Instance.OnDetailPage);
        button.onClick.AddListener(() => FetchDetail(courseInfo));
        button.onClick.AddListener(() => Debug.LogWarning("버튼테스트"));
    }
    public void FetchDetail(AICourseInfo _ori)
    {
        // AICourseInfo의 ShopName을 이용하여 ShopManager에서 해당 가게 정보를 BaseShopInfoData 형태로 가져온다.
        BaseShopInfoData data = LoadManager.Instance.GetShopInfoByShopName(_ori.ShopName.text);
        ShopContentDetail.FetchContent(data);

        VideoPlayManager.Instance.PlayVideo(VideoType.AISearch_Detail);
    }

}

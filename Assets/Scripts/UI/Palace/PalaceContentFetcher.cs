using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalaceContentFetcher : MonoBehaviour
{
    [SerializeField] Transform contentsParent;
    [SerializeField] List<PalaceContent> palaceContentList = new();

    public PalaceContentDetail palaceDetail;

    public BackButton backButton;
    public HomeButton homeButton;

    public void Init()
    {
        palaceDetail = GetComponentInChildren<PalaceContentDetail>(true);

        backButton = GetComponentInChildren<BackButton>();
        homeButton = GetComponentInChildren<HomeButton>();

        backButton.onClick.AddListener(ClosePalaceDetail);
        homeButton.onClick.AddListener(ClosePalaceDetail);


        for (int i = 0; i < contentsParent.childCount; ++i)
        {
            var pc = contentsParent.GetChild(i).GetComponent<PalaceContent>();

            pc.Init(this);

            pc.FetchContent(LoadManager.Instance.PalaceDataList[i]);

            palaceContentList.Add(pc);
        }
    }
    public void OpenPalaceDetail()
    {
        if (palaceDetail.gameObject.activeSelf == false)
        {
            palaceDetail.gameObject.SetActive(true);

            VideoPlayManager.Instance.PlayVideo(VideoType.Palace_Detail);
        }
    }

    public void ClosePalaceDetail()
    {
        if (palaceDetail.gameObject.activeSelf == true)
        {
            palaceDetail.gameObject.SetActive(false);
        }
    }

}

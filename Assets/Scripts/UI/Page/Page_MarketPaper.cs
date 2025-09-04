using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Page_MarketPaper : MonoBehaviour
{
    [SerializeField]
    List<SecondCategoryButton> secondCategoryButtons = new();
    [SerializeField]
    List<GameObject> pages = new();

    public void Init()
    {
        for (int i = 0; i < secondCategoryButtons.Count; i++)
        {
            secondCategoryButtons[i].SecondCategoryButtonIndex = i;
            int index = i;
            secondCategoryButtons[index].onClick.AddListener(() =>
            {
                for (int j = 0; j < pages.Count; j++)
                {
                    pages[j].SetActive(j == index);
                }

                UIManager.Instance.DeselectAllCustomButtons(secondCategoryButtons.ToList());
                secondCategoryButtons[index].SetSelected(true);
            });
        }

        //secondCategoryButtons[0].onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Greeting));
        //secondCategoryButtons[1].onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Greeting_Hobby));
        //secondCategoryButtons[2].onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Greeting_Stretching, forceReset: true));
    }

    public void Reset()
    {
        secondCategoryButtons[0].SetSelected(true);
    }

    public void SelectFirstCategory()
    {
        // 첫번째 버튼 선택 상태로 설정
        UIManager.Instance.DeselectAllCustomButtons(secondCategoryButtons.ToList());
        secondCategoryButtons[0].SetSelected(true);
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(false);
        }
        pages[0].SetActive(true);
    }
}

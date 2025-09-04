using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 심플하게 카테고리를 용하여 페이지 온오프
/// </summary>

public class Page_Greeting : MonoBehaviour
{
    [SerializeField]
    List<SecondCategoryButton> secondCategoryButtons = new();
    [SerializeField]
    GameObject[] pages = new GameObject[3];

    public void Init()
    {
        for (int i = 0; i < secondCategoryButtons.Count; i++)
        {
            secondCategoryButtons[i].SecondCategoryButtonIndex = i;
            int index = i;
            secondCategoryButtons[index].onClick.AddListener(() =>
            {
                for (int j = 0; j < pages.Length; j++)
                {
                    pages[j].SetActive(j == index);
                }

                UIManager.Instance.DeselectAllCustomButtons(secondCategoryButtons.ToList());
                secondCategoryButtons[index].SetSelected(true);
            });
        }
        Debug.Log("인사페이지 버튼등록완료");

        secondCategoryButtons[0].onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Greeting));
        secondCategoryButtons[1].onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Greeting_Hobby));
        secondCategoryButtons[2].onClick.AddListener(() => VideoPlayManager.Instance.PlayVideo(VideoType.Greeting_Stretching, forceReset: true));
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
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        pages[0].SetActive(true);
    }
}

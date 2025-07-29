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
    SecondCategoryButton[] secondCategoryButtons = new SecondCategoryButton[3];
    [SerializeField]
    GameObject[] pages = new GameObject[3];

    

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        for (int i = 0; i < secondCategoryButtons.Length; i++)
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
    }

    public void Reset()
    {
        secondCategoryButtons[0].SetSelected(true);
    }
}

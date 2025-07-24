using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// AI Selector가 선택한 카테고리를 바탕으로 AI 추천 코스를 생성하는 클래스

public class AIRecommendCourse : MonoBehaviour
{
    [SerializeField] Page_AISelect aiSelector;

    [SerializeField] Button[] courseSelectButton = new Button[3]; // A, B, C 코스 선택 버튼

    [SerializeField] AICourseInfoHolder[] AICourseInfoHolder_Big = new AICourseInfoHolder[3];

    [SerializeField] AICourseInfoHolder[] AICourseInfoHolder_Small = new AICourseInfoHolder[3];

    [SerializeField] TextMeshProUGUI courseText;

    public void Init()
    {
        if (aiSelector == null)
        {
            aiSelector = FindObjectOfType<Page_AISelect>();
        }

        aiSelector.generateButton.onClick.AddListener(FetchCourseInfoSmall);

        for (int i = 0; i < courseSelectButton.Length; i++)
        {
            int index = i;
            courseSelectButton[i].onClick.AddListener(() => OnCourseContentBig(index));
        }

        this.transform.GetComponentInChildren<BackButton>().onClick.AddListener(CloseAllCourseContentBig);
    }

    public void FetchCourseInfoSmall() // 중복 추천의 가능성있으므로 추후 개선 필요
    {
        var selectedCategory = aiSelector.GetSelectedCategoryList();

        var nowLanguage = (int)UIManager.Instance.NowLanguage;

        for (int i = 0; i < AICourseInfoHolder_Small.Length; i++)
        {
            for (int j = 0; j < AICourseInfoHolder_Small[i].AICourseInfoArr.Length; j++)
            {
                var data = LoadManager.Instance.GetShopsByAICategory(selectedCategory[j]);

                int randomIndex = Random.Range(0, data.Count); // 추후 order로 받아오면 지울 것
                var aicourinfo = AICourseInfoHolder_Small[i].AICourseInfoArr[j];

                // AI카테고리가 비었으면 2차 카테고리 출력 있다면 AI카테고리 출력
                if (string.IsNullOrWhiteSpace(data[randomIndex].AICategoryString[nowLanguage])) 
                {
                    aicourinfo.ShopSecondCategory.text = CommonFunction.SplitAndTrim(data[randomIndex].SecondCategoryString[nowLanguage], '-', 1);
                }
                else aicourinfo.ShopSecondCategory.text = CommonFunction.SplitAndTrim(data[randomIndex].AICategoryString[nowLanguage], '-', 1);

                aicourinfo.ShopName.text = data[randomIndex].ShopName[nowLanguage];
                aicourinfo.HashTag.text = data[randomIndex].HashTag[nowLanguage];
                aicourinfo.ShopImage.sprite = data[randomIndex].spriteImage[0];

                if (aicourinfo.ShopImage.sprite == null)
                {
                    aicourinfo.ShopImage.sprite = PrefabManager.Instance.NoImageSprite;
                }

            }
        }
    }

    public void FetchCourseInfoBig(int index)
    {
        for (int i = 0; i < AICourseInfoHolder_Big[index].AICourseInfoArr.Length; i++)
        {
            AICourseInfoHolder_Big[index].AICourseInfoArr[i].CopyContent(AICourseInfoHolder_Small[index].AICourseInfoArr[i]);
        }
    }

    public void OnCourseContentBig(int index)
    {
        CloseAllCourseContentBig();

        courseText.gameObject.SetActive(false);
        AICourseInfoHolder_Big[index].gameObject.SetActive(true);

        FetchCourseInfoBig(index);
    }

    public void CloseAllCourseContentBig()
    {
        for (int i = 0; i < AICourseInfoHolder_Big.Length; i++)
        {
            AICourseInfoHolder_Big[i].gameObject.SetActive(false);
        }
        courseText.gameObject.SetActive(true);
    }


}



using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HanbokCategoryButton : Button, ISelectableButton
{
    public int CategoryButtonIndex { get; set; } = -1;

    public bool IsSelected { get; private set; } = false;

    [SerializeField] TextMeshProUGUI categoryText;

    public List<(string, Sprite)> HanbokSpriteList = new();
    protected override void Awake()
    {
        categoryText = GetComponentInChildren<TextMeshProUGUI>();
    }
    protected override void Start()
    {
        base.Start();
        onClick.AddListener(OnButtonClicked);
        SetSelected(false); // 초기 상태는 선택되지 않음
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (IsSelected == true)
        {
            CommonFunction.ChangeColorBtnAndTxt(transform);
            FetchHanbokContent();
        }
        else
        {
            CommonFunction.ChangeColorBtnAndTxt(transform, false);
        }
    }
    public void OnButtonClicked()
    {
        // 모든 버튼의 선택 해제
        UIManager.Instance.DeselectAllCustomButtons(UIManager.Instance.HanbokCategorieButtons);
        SetSelected(true);
        SelectFirstHanbokContent();
    }
    public void FetchHanbokContent()
    {
        for (int i = 0; i < UIManager.Instance.HanbokContentButtons.Count; ++i)
        {
            if (i < HanbokSpriteList.Count)
            {
                UIManager.Instance.HanbokContentButtons[i].gameObject.SetActive(true);

                (string fileName, Sprite sprite) = HanbokSpriteList[i];
                UIManager.Instance.HanbokContentButtons[i].FetchHanbokSprite(fileName, sprite);
            }
            else
            {
                UIManager.Instance.HanbokContentButtons[i].gameObject.SetActive(false);
            }
        }
    }
    public void SelectFirstHanbokContent()// 첫번째 콘텐츠 선택시키기
    {
        for (int i = 0; i < UIManager.Instance.HanbokContentButtons.Count; ++i)
        {
            if (i == 0)
            {
                UIManager.Instance.HanbokContentButtons[i].SetSelected(true);
            }
            else
            {
                UIManager.Instance.HanbokContentButtons[i].SetSelected(false);
            }
        }


    }
}

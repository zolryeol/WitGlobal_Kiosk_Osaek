using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Init();
    }

    public void Init()
    {
        onClick.AddListener(OnButtonClicked);
        onClick.AddListener(() => UIManager.Instance.PlayVideoByCategoryButton());
        SetSelected(false); // 초기 상태는 선택되지 않음
        Debug.Log("<color=blue>한복카테고리 초기화</color>");
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
        UIManager.Instance.InitScrollbarValue(UIManager.Instance.HanbokScrollbar, true);
    }
    public void FetchHanbokContent()
    {
        var _hanbokContentButton = UIManager.Instance.HanbokContentButtons;

        for (int i = 0; i < _hanbokContentButton.Count; ++i)
        {
            if (i < HanbokSpriteList.Count)
            {
                _hanbokContentButton[i].gameObject.SetActive(true);

                (string fileName, Sprite sprite) = HanbokSpriteList[i];
                _hanbokContentButton[i].FetchHanbokSprite(fileName, sprite);
            }
            else
            {
                _hanbokContentButton[i].gameObject.SetActive(false);
            }
        }

        // 자식 버튼이 모두 비활성화된 HanbokPage는 비활성화
        var pages = _hanbokContentButton.Select(btn => btn.transform.parent).Distinct();

        foreach (var page in pages)
        {
            bool anyActive = page.GetComponentsInChildren<HanbokContentButton>(true)
                                .Any(b => b.gameObject.activeSelf);
            page.gameObject.SetActive(anyActive);
        }
    }
    public void SelectFirstHanbokContent()// 첫번째 콘텐츠 선택시키기
    {
        var _hanbokContentButton = UIManager.Instance.HanbokContentButtons;


        for (int i = 0; i < _hanbokContentButton.Count; ++i)
        {
            if (i == 0)
            {
                _hanbokContentButton[i].SetSelected(true);
            }
            else
            {
                _hanbokContentButton[i].SetSelected(false);
            }
        }


    }
}

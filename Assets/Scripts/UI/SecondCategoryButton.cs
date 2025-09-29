using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SecondCategoryButton : Button, ILocalizable, ISelectableButton
{
    public int SecondCategoryButtonIndex { get; set; } = -1;

    [SerializeField]
    protected TextMeshProUGUI categoryText;

    public bool IsSelected { get; set; } = false;

    protected override void Awake()
    {
        categoryText = GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        //base.Start();

        onClick.AddListener(OnButtonClicked);
        SetSelected(false); // 초기 상태는 선택되지 않음
        onClick.AddListener(() => UIManager.Instance.PlayVideoByCategoryButton());
        Debug.Log("<color=blue>2차카테고리 버튼 초기화</color>");
    }

    public void UpdateLocalizedString(string targetString)
    {
        categoryText.text = targetString;
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (IsSelected == true)
        {
            CommonFunction.ChangeColorBtnAndTxt(transform);

            //GetComponent<Image>().color = UIColorPalette.SelectedColor;
            //transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.SelectedTextColor;
        }
        else
        {
            CommonFunction.ChangeColorBtnAndTxt(transform, false);

            //GetComponent<Image>().color = UIColorPalette.NormalColor;
            //transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.NormalTextColor;
        }
    }

    public void OnButtonClicked()
    {
        // 모든 버튼의 선택 해제
        UIManager.Instance.DeselectAllCustomButtons(UIManager.Instance.SecondCategorieButtons);
        SetSelected(true);
    }
}

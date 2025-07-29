using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인버튼에 컴포넌트로 이벤트등록
/// </summary>
public class MainButton : MonoBehaviour, ILocalizable
{
    Button button;

    [SerializeField]
    CanvasGroup _targetCanvasGroup;
    [SerializeField]
    Category_Base category;
    [SerializeField]
    Category_ETC categoryETC;
    [SerializeField]
    Button FirstCategory;

    [SerializeField] Transform body;
    [SerializeField] Transform buttonParent;
    [SerializeField] Button firstButton;

    TextMeshProUGUI buttonText;

    public void Init()
    {
        if (TryGetComponent<Button>(out Button _button))
        {
            button = _button;
        }
        else button = this.AddComponent<Button>();

        button.onClick.AddListener(() => UIManager.Instance.OpenPage(_targetCanvasGroup));
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // 카테고리 설정
        button.onClick.AddListener(() => UIManager.Instance.SetNowSelectCategory(category, categoryETC));

        if (category != Category_Base.Default)
        {
            if (_targetCanvasGroup != null) // Transform 할당
            {
                body = _targetCanvasGroup.transform.Find("Body").transform;
                buttonParent = body.Find("ButtonsParent").transform;
                firstButton = buttonParent.GetChild(0).GetComponent<Button>();
            }

            button.onClick.AddListener(() => OnCategoryButton()); // 카테고리 버튼 클릭시 서브 카테고리 버튼 활성화
            button.onClick.AddListener(() => SelectFirstCategory()); // 페이지 열때 첫번째 카테고리 자동으로 선택되기 위해
            button.onClick.AddListener(() => UIManager.Instance.FetchingContent(0)); // 페이지 열때 첫번째 카테고리 자동으로 선택되기 위해
            button.onClick.AddListener(() => HeaderChange());
        }

        // 베이스 카테고리가 아닐경우 예외처리
        if (categoryETC == Category_ETC.Photo) // 촬영버튼
        {
            button.onClick.AddListener(() => UIManager.Instance.SelectFirstCategory(UIManager.Instance.HanbokCategorieButtons));
            button.onClick.AddListener(() => UIManager.Instance.HanbokCategorieButtons[0].SelectFirstHanbokContent());
        }
        else if (categoryETC == Category_ETC.Map) // 지도
        {
            body = _targetCanvasGroup.transform.Find("Body").transform;
            buttonParent = body.Find("ButtonsParent").transform;
            firstButton = buttonParent.GetChild(1).GetComponent<Button>(); // 두번째버튼
            button.onClick.AddListener(() => firstButton.onClick.Invoke());
        }
        else if (categoryETC == Category_ETC.HanbokExplain) // 한복설명
        {
            button.onClick.AddListener(() => UIManager.Instance.InitScrollbarValue(UIManager.Instance.HanbokExplainScrollbar, true));
        }

        button.onClick.AddListener(() => UIManager.Instance.PlayVideoByMainButton()); // 영상재생

        button.onClick.AddListener(() => UIManager.Instance.CloseKeyboard());
    }
    private void SelectFirstCategory() // 페이지 열때 첫번째 카테고리 자동으로 선택되기 위해
    {
        if (category != Category_Base.Default || _targetCanvasGroup != null)
        {
            Debug.Log("첫번째 카테고리 선택");

            UIManager.Instance.DeselectAllCustomButtons(UIManager.Instance.SecondCategorieButtons);

            //firstButton.Select();
            firstButton.GetComponent<SecondCategoryButton>().SetSelected(true); // 첫번째 버튼 선택 상태로 설정)
                                                                                //Canvas.ForceUpdateCanvases(); // 강제 갱신
        }
    }
    public void OnCategoryButton()
    {
        var secondCateStrList = LoadManager.Instance.GetSecondCategoryStringTrim(category);

        Debug.Log("카테고리 카운트 = " + secondCateStrList.Count);

        for (int i = 0; i < UIManager.SecondCategoryMaxCount; i++)
        {
            var secondCategoryButton = UIManager.Instance.SecondCategorieButtons[i];

            if (i < secondCateStrList.Count)
            {
                secondCategoryButton.gameObject.SetActive(true);
                secondCategoryButton.UpdateLocalizedString(secondCateStrList[i].Item2); // 카테고리 문자열 업데이트
            }
            else
            {
                secondCategoryButton.gameObject.SetActive(false);
            }
        }
    }

    void ILocalizable.UpdateLocalizedString(string str)
    {
        buttonText.text = str;
    }

    void HeaderChange()
    {
        var targetHeader = UIManager.Instance.PublicHeader;

        switch (category)
        {
            case Category_Base.Default:
                return;
            case Category_Base.ToEat:
                targetHeader.SetKey("MainButton_ToEat");
                break;
            case Category_Base.ToBuy:
                targetHeader.SetKey("MainButton_ToBuy");
                break;
            case Category_Base.ToGallery:
                targetHeader.SetKey("MainButton_ToGallery");
                break;
            case Category_Base.ToHelp:
                targetHeader.SetKey("MainButton_ToHelp");
                break;
            case Category_Base.ToStay:
                targetHeader.SetKey("MainButton_ToStay");
                break;
            default:
                break;
        }

        targetHeader.UpdateText();
    }
}

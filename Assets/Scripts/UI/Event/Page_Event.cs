using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Page_Event : MonoBehaviour, IPrefabInstancer
{
    [SerializeField] Transform buttonParent;
    [SerializeField] Transform contentParent;

    BackButton backButton;
    HomeButton homeButton;

    [SerializeField] List<Button> categoryButtonList = new();

    public List<EventContent> contentObjList = new();

    [SerializeField] Scrollbar scrollbar;

    int maxCountentCount;
    public void Init()
    {
        // 예외처리용
        var mainButton = GameObject.Find("Event").GetComponent<Button>();
        mainButton.onClick.AddListener(() => Select(0));
        mainButton.onClick.AddListener(() => FetchingContent(0));

        for (int i = 0; i < buttonParent.childCount; ++i)
        {
            var _button = buttonParent.GetChild(i).GetComponent<Button>();

            int index = i;

            _button.onClick.AddListener(() => Select(index));
            _button.onClick.AddListener(() => FetchingContent((EventState)index));
            categoryButtonList.Add(_button);
        }


        backButton = GetComponentInChildren<BackButton>();
        homeButton = GetComponentInChildren<HomeButton>();
    }

    public void FetchingContent(EventState eventState)
    {
        var targetEvent = ShopManager.Instance.GetEventByState(eventState);

        for (int i = 0; i < contentObjList.Count; ++i)
        {
            if (i < targetEvent.Count)
            {
                contentObjList[i].gameObject.SetActive(true);
                contentObjList[i].FetchContent(targetEvent[i]);
                UIManager.Instance.InitScrollbarValue(scrollbar);

            }
            else
            {
                contentObjList[i].gameObject.SetActive(false);
            }
        }
    }

    void Select(int _index)
    {
        Debug.Log("셀렉트 테스트");

        UnSelect();

        CommonFunction.ChangeColorBtnAndTxt(categoryButtonList[_index].transform);

        //categoryButtonList[_index].GetComponent<Image>().color = UIColorPalette.SelectedColor;
        //categoryButtonList[_index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.SelectedTextColor;
    }
    void UnSelect()
    {
        foreach (var c in categoryButtonList)
        {
            CommonFunction.ChangeColorBtnAndTxt(c.transform, false);

            //c.GetComponent<Image>().color = UIColorPalette.NormalColor;
            //c.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.NormalTextColor;
        }
    }
    void IPrefabInstancer.CreateContentInstance()
    {
        buttonParent = CommonFunction.FindDeepChild(this.gameObject, "ButtonsParent").transform;
        contentParent = CommonFunction.FindDeepChild(this.gameObject, "Contents").transform;

        Debug.Log("이벤트 콘텐츠 인스턴싱");

        maxCountentCount = ShopManager.Instance.GetEventMaxCount();

        var page = FindObjectOfType<Page_Event>();

        for (int i = 0; i < maxCountentCount; ++i)
        {
            var content = Instantiate(PrefabManager.Instance.EventInfoPrefab, contentParent);
            content.GetComponent<EventContent>().Init();
            content.SetActive(false);
            page.contentObjList.Add(content.GetComponent<EventContent>());

        }

        Debug.Log($"이벤트콘텐츠 {maxCountentCount} 개 인스턴싱");

    }
}

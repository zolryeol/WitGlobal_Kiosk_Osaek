using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



// Shop리스트와 별개 교통페이지 

public class Page_Transport : MonoBehaviour
{
    Transform buttonParent;
    Transform contentParent;

    BackButton backButton;
    HomeButton homeButton;

    [SerializeField] List<Button> categoryButtonList = new();
    [SerializeField] List<GameObject> contentObjList = new();

    public List<Button> CategoryButtonList { get => categoryButtonList; set => categoryButtonList = value; }

    public void Init()
    {
        // 예외처리용
        var mainButton = GameObject.Find("Transport").GetComponent<Button>();
        mainButton.onClick.AddListener(() => OnContent(0));
        mainButton.onClick.AddListener(() => Select(0));
        //

        buttonParent = CommonFunction.FindDeepChild(this.gameObject, "ButtonsParent").transform;

        for (int i = 0; i < buttonParent.childCount; ++i)
        {
            var _button = buttonParent.GetChild(i).GetComponent<Button>();

            int index = i;

            _button.onClick.AddListener(() => OnContent(index));
            _button.onClick.AddListener(() => Select(index));

            categoryButtonList.Add(_button);
        }

        contentParent = CommonFunction.FindDeepChild(this.gameObject, "Contents").transform;

        for (int i = 0; i < contentParent.childCount; ++i)
        {
            contentObjList.Add(contentParent.GetChild(i).gameObject);
        }

        backButton = GetComponentInChildren<BackButton>();
        homeButton = GetComponentInChildren<HomeButton>();

        backButton.onClick.AddListener(CloseAllContent);
        homeButton.onClick.AddListener(CloseAllContent);
    }

    public void OnContent(int _index)
    {
        CloseAllContent();

        contentObjList[_index].SetActive(true);
    }
    public void CloseAllContent()
    {
        foreach (var c in contentObjList)
        {
            c.SetActive(false);
        }
    }

    public void Select(int _index)
    {
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static public UIManager Instance;

    public const int SecondCategoryMaxCount = 12; // 인사동은 현재 최대 12개라서

    public List<SecondCategoryButton> SecondCategorieButtons = new(); // 각각의 second category 버튼을 저장하는 리스트
    public List<HanbokCategoryButton> HanbokCategorieButtons = new();
    public List<HanbokContentButton> HanbokContentButtons = new();
    public List<ShopContent> ShopContents = new(); /// 관련된 것들을 Page_Shop 을 만들어서 옮기면 좋을듯 하다.

    private Language nowLanguage = Language.Korean;
    public Language NowLanguage
    {
        get => nowLanguage;
        set
        {
            if (nowLanguage != value)
            {
                nowLanguage = value;
                UpdateLocaization(); // 언어가 바뀌면 UI 갱신
                Debug.Log($"🌐 언어 변경됨: {nowLanguage}");
                ChangeLanguageEvent?.Invoke();
            }
        }
    }

    public event Action ChangeLanguageEvent; // 언어 변경시 호출될 이벤트들;

    public Category_Base NowSelectedCategory = Category_Base.Default; // 현재 선택된 카테고리

    public Scrollbar FoodAndShopScrollbar; // 뭐먹지, 뭐사지 스크롤바

    [Header("ContentFetcher")]
    public PalaceContentFetcher PalaceContentFetcher;
    public EventContentDetail EventContentDetail;

    [Header("PageScript")]
    [SerializeField] Page_Transport page_Tarnsport;
    [SerializeField] Page_Event page_Event;
    [SerializeField] Page_Language page_Language;


    [Header("PagesCanvasGroup")]
    public CanvasGroup DetailPage;
    public CanvasGroup ShopListPage;
    public CanvasGroup ShopDetail;
    public CanvasGroup AISelectPage;
    public CanvasGroup AIRecommendPage;
    public CanvasGroup EventDetailPage;

    [Header("Localization")]
    public List<ILocalizableImage> localizableImageList = new();
    public List<LocalizerText> localizerTexts = new();

    public Stack PageStack = new(); // 페이지스택
    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitContentCreator();

        InitMainButtons();
        InitAISelect();

        InitContentFetcher();
        InitPages();

        InitLocalization();
    }

    public void InitLocalization()
    {
        localizableImageList = FindObjectsOfType<MonoBehaviour>(true)  // 비활성 오브젝트까지 포함
       .OfType<ILocalizableImage>()
       .ToList();

        foreach (var localizable in localizableImageList)
        {
            localizable.InitLocalizableImage();
        }

        Debug.Log($"🌐 LocalizableImage 초기화 완료 - 총 {localizableImageList.Count}개");

        localizerTexts = FindObjectsOfType<MonoBehaviour>(true).OfType<LocalizerText>().ToList();

        foreach (var lt in localizerTexts)
        {
            lt.InitLocalizerText();
        }
    }

    public void UpdateLocaization()
    {
        foreach (var li in localizableImageList)
        {
            li.UpdateLocalizableImage();
        }

        foreach (var lt in localizerTexts)
        {
            lt.UpdateText();
        }
    }

    public void InitContentFetcher()
    {
        PalaceContentFetcher = FindAnyObjectByType<PalaceContentFetcher>();
        PalaceContentFetcher.Init();

        EventContentDetail = FindAnyObjectByType<EventContentDetail>();
    }

    public void InitPages()
    {
        page_Tarnsport = FindAnyObjectByType<Page_Transport>();
        page_Tarnsport.Init();

        page_Event = FindAnyObjectByType<Page_Event>();
        page_Event.Init();

        page_Language = FindAnyObjectByType<Page_Language>();
        page_Language.Init();
    }

    public void OpenPage(CanvasGroup _targetCanvasGroup, Action ac = null)
    {
        ac?.Invoke();
        PageStack.Push(_targetCanvasGroup);
        _targetCanvasGroup.alpha = 1f;
        _targetCanvasGroup.interactable = true;
        _targetCanvasGroup.blocksRaycasts = true;
    }

    public void ClosePage(CanvasGroup _targetCanvasGroup)
    {
        _targetCanvasGroup.alpha = 0f;
        _targetCanvasGroup.interactable = false;
        _targetCanvasGroup.blocksRaycasts = false;
    }
    public void OnDetailPage()
    {
        if (DetailPage.alpha < 1)
        {
            PageStack.Push(DetailPage);
            DetailPage.alpha = 1f;
            DetailPage.interactable = true;
            DetailPage.blocksRaycasts = true;
        }
        else
        {
            DetailPage.alpha = 0f;
            DetailPage.interactable = false;
            DetailPage.blocksRaycasts = false;
        }
    }

    public void OnAIRecommendPage()
    {
        if (AIRecommendPage.alpha < 1)
        {
            PageStack.Push(AIRecommendPage);
            AIRecommendPage.alpha = 1f;
            AIRecommendPage.interactable = true;
            AIRecommendPage.blocksRaycasts = true;
        }
        else
        {
            AIRecommendPage.alpha = 0f;
            AIRecommendPage.interactable = false;
            AIRecommendPage.blocksRaycasts = false;
        }
    }

    public void CloseAllPages()
    {
        while (PageStack.Count > 0)
        {
            var page = PageStack.Pop() as CanvasGroup;
            if (page != null)
            {
                ClosePage(page);
            }
        }

        VideoPlayManager.Instance.PlayVideo(VideoType.Default);
    }
    public void InitContentCreator() // 페이지안의 콘텐츠들 생성 (카테고리버튼 및 prefab)
    {
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();
        List<IPrefabInstancer> result = new();

        foreach (var behaviour in allBehaviours)
        {
            if (behaviour is IPrefabInstancer icc)
            {
                icc.CreateContentInstance();
            }
        }
    }

    public void InitMainButtons()
    {
        var mainButtons = FindObjectsOfType<MainButton>();

        foreach (var button in mainButtons)
        {
            button.Init();
        }

        Debug.Log("메인버튼 초기화 완료");

        var AiButton = FindObjectOfType<AIButton>();
        if (AiButton != null)
        {
            AiButton.Init();
            Debug.Log("AI 버튼 초기화 완료");
        }

        var languageButton = FindObjectOfType<LanguageChangeButton>();
        languageButton.Init();
    }

    public void InitAISelect()
    {
        var AISelector = FindObjectOfType<Page_AISelect>();
        AISelector.Init();

        var AIRecommendCourse = FindObjectOfType<AIRecommendCourse>();
        AIRecommendCourse.Init();

        var AIContentBigButton = FindObjectsOfType<AIContentBigButton>(true);
        foreach (var button in AIContentBigButton)
        {
            button.Init();
        }
    }



    public void FetchingContent(int categoryButtonIndex)
    {
        Category_Base baseCategory = NowSelectedCategory; // 현재 선택된 카테고리로 설정

        var t = ShopManager.Instance.GetShopsBySecondCategory(baseCategory, categoryButtonIndex);

        // 랜덤순으로 섞는다.
        t = t.OrderBy(_ => UnityEngine.Random.value).ToList();

        Debug.Log("콘텐츠 갯수 = " + t.Count);

        for (int i = 0; i < ShopContentCreator.MaxContentCount; ++i)
        {
            if (i < t.Count)
            {
                ShopContents[i].gameObject.SetActive(true);
                ShopContents[i].FetchContent(t[i]);
            }
            else
            {
                ShopContents[i].gameObject.SetActive(false);
            }
        }

        InitScrollbarValue(FoodAndShopScrollbar);
    }

    public void InitScrollbarValue(Scrollbar scrollbar)
    {
        StartCoroutine(SetScrollbarToTopNextFrame(scrollbar));
    }

    private IEnumerator SetScrollbarToTopNextFrame(Scrollbar targetScrollbar)
    {
        for (int i = 0; i < 2; ++i) // 임시방편
        {
            yield return null;
        }

        targetScrollbar.value = 1f;
        Debug.Log("스크롤바 value = 1 (코루틴)");
    }
    public void SelectFirstCategory<T>(List<T> targetButtonList) where T : MonoBehaviour  // 페이지 열때 첫번째 카테고리 자동으로 선택되기 위해
    {
        Debug.Log("첫번째 카테고리 선택");

        DeselectAllCustomButtons(targetButtonList);

        targetButtonList[0].GetComponent<ISelectableButton>().SetSelected(true);
    }

    public void DeselectAllCustomButtons<T>(List<T> buttonList) where T : MonoBehaviour
    {
        foreach (var btn in buttonList)
        {
            var settable = btn as ISelectableButton;
            if (settable != null)
            {
                settable.SetSelected(false);
            }
            else
            {
                Debug.LogWarning($"{btn.name}은(는) ISelectableButton 인터페이스를 구현하지 않았습니다.");
            }
        }
    }

    private void Update()
    {

        if (0 < Input.touchCount)
        {
            Debug.Log("터치함");
        }
    }
}

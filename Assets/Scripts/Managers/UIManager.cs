using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public Category_ETC NowSelectedETC = Category_ETC.Default; // 현재 선택된 ETC 카테고리
    public string NowSelectedKoreaMapName = ""; // 현재 선택된 한국지도 이름

    public Scrollbar FoodAndShopScrollbar; // 뭐먹지, 뭐사지 스크롤바
    public Scrollbar HanbokScrollbar; // 사진찍기 한복선택 스크롤바
    public Scrollbar HanbokExplainScrollbar; // 한복 설명 스크롤바

    [Header("ContentFetcher")]
    public PalaceContentFetcher PalaceContentFetcher;
    public EventContentDetail EventContentDetail;

    [Header("PageScript")]
    [SerializeField] Page_Transport page_Tarnsport;
    [SerializeField] Page_Event page_Event;
    [SerializeField] Page_Language page_Language;
    [SerializeField] Page_Greeting page_Greeting;
    [SerializeField] Page_KoreaMapDetail page_KoreaMapDetail;
    [SerializeField] Page_SmartTourList page_SmartTourList;
    [SerializeField] Page_TraditionalMarket page_TraditionalMarket;

    [SerializeField] Page_HereInfo page_HereInfo;
    [SerializeField] Page_MarketPaper page_MarketPaper;
    public Page_Photo Page_Photo;
    public Page_SmartTourList Page_SmartTourList => page_SmartTourList;
    public Page_KoreaMapDetail Page_KoreaMapDetail => page_KoreaMapDetail;

    [Header("PagesCanvasGroup")]
    public CanvasGroup DetailPage;
    public CanvasGroup ShopListPage;
    public CanvasGroup ShopDetail;
    public CanvasGroup AISelectPage;
    public CanvasGroup AIRecommendPage;
    public CanvasGroup EventDetailPage;
    public CanvasGroup KoreaMapDetailPage;
    public CanvasGroup SmartTourListPage;


    [Header("Localization")]
    [Space(30)]
    public List<ILocalizableImage> localizableImageList = new();
    public List<LocalizerText> localizerTexts = new();
    public LocalizerText PublicHeader; // shop페이지의 공용헤더
    public LocalizerText PublicSubHeader; // shop페이지의 공용Sub헤더

    [Header("키보드")]
    [Space(30)]
    public HangulKeyborad keyboard;

    [Header("날씨버튼")]
    public WeatherButton weatherButton;


    [Header("초기화타임")]
    private float lastTouchTime = 0f;
    [SerializeField] private float idleThreshold = 180f;
    private int lastLoggedSecond = -1;

    [Header("ETC")] // 예외처리용
    public GameObject NoContentText; // 검색 결과없을때 나타나도록
    public GameObject NoContentFooter; // 검색 결과없을때 나타나도록
    public GameObject ToStayOnlyText;
    public GameObject ToStayOnlyText2;

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

        InitKoreaMapButton();

        InitLocalization();

        weatherButton.Init();
    }

    public void SetNowSelectCategory(Category_Base _baseCategory = Category_Base.Default, Category_ETC _EtcCategory = Category_ETC.Default)
    {
        NowSelectedCategory = _baseCategory;
        NowSelectedETC = _EtcCategory;
        Debug.Log($"현재 선택된 카테고리: {NowSelectedCategory}, ETC: {NowSelectedETC}");
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
        //PalaceContentFetcher = FindAnyObjectByType<PalaceContentFetcher>();
        //PalaceContentFetcher.Init();

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

        page_Greeting = FindAnyObjectByType<Page_Greeting>();
        page_Greeting.Init();

        page_HereInfo = FindAnyObjectByType<Page_HereInfo>();
        page_HereInfo.Init();

        page_MarketPaper = FindAnyObjectByType<Page_MarketPaper>();
        page_MarketPaper.Init();

        page_KoreaMapDetail = FindAnyObjectByType<Page_KoreaMapDetail>();
        page_KoreaMapDetail.Init();

        page_SmartTourList = FindAnyObjectByType<Page_SmartTourList>();
        page_SmartTourList.Init();

        Page_Photo = FindAnyObjectByType<Page_Photo>();
    }

    public void OpenPage(CanvasGroup _targetCanvasGroup, Action ac = null)
    {
        ac?.Invoke();
        if (1f <= _targetCanvasGroup.alpha)
        {
            KioskLogger.Warn($"페이지 {_targetCanvasGroup.name}는 이미 열려 있습니다.");
            return;
        }
        PageStack.Push(_targetCanvasGroup);
        Debug.Log($"페이지 스텍 카운트 = {PageStack.Count}");
        _targetCanvasGroup.alpha = 1f;
        _targetCanvasGroup.interactable = true;
        _targetCanvasGroup.blocksRaycasts = true;
    }

    public void OnDetailPage()
    {
        if (DetailPage.alpha < 1)
        {
            PageStack.Push(DetailPage);
            Debug.Log($"OnDeatailPage 페이지 스텍 카운트 = {PageStack.Count}");

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

    public void ClosePage(CanvasGroup _targetCanvasGroup)
    {
        Debug.Log($"클로즈 페이지 pageStackcount = {PageStack.Count}");
        _targetCanvasGroup.alpha = 0f;
        _targetCanvasGroup.interactable = false;
        _targetCanvasGroup.blocksRaycasts = false;

        if (PageStack.Count == 0)
        {
            Debug.Log($"카운트가 0 이라서 기본 비디오 재생");

            VideoPlayManager.Instance.PlayVideo(VideoType.Default);
            OpenKeyboard(); // 키보드 열기
        }

        OnToStayText(false); // 숙박안내 예외처리용
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

    public void InitHanbokCategoryButton()
    {
        HanbokCategorieButtons = FindObjectsOfType<HanbokCategoryButton>(true).OrderBy(b => b.transform.GetSiblingIndex()).ToList();
        foreach (var button in HanbokCategorieButtons)
        {
            button.Init();
        }
        Debug.Log("한복 카테고리 버튼 초기화 완료");
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

    public void InitKoreaMapButton()
    {
        var koreaMapButtons = FindObjectsOfType<KoreaMapButton>(true);
        foreach (var button in koreaMapButtons)
        {
            button.Init();
        }
        Debug.Log("한국지도 버튼 초기화 완료");
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
        Category_Base baseCategory = NowSelectedCategory;

        var shops = LoadManager.Instance.GetShopsBySecondCategory(baseCategory, categoryButtonIndex);

        // 이미지가 하나라도 있는지 검사 (spriteImage 배열에 null 아닌 항목이 있는지 확인) // 이미지 있는것 우선순위
        var hasImageList = shops
            .Where(shop => shop.spriteImage != null && shop.spriteImage.Any(sprite => sprite != null))
            .OrderBy(_ => UnityEngine.Random.value)
            .ToList();

        var noImageList = shops
            .Where(shop => shop.spriteImage == null || shop.spriteImage.All(sprite => sprite == null))
            .OrderBy(_ => UnityEngine.Random.value)
            .ToList();

        var finalList = hasImageList.Concat(noImageList).ToList();

        Debug.Log("콘텐츠 갯수 = " + finalList.Count);

        OnNoContentText(finalList.Count == 0);

        for (int i = 0; i < ShopContentCreator.MaxContentCount; ++i)
        {
            if (i < finalList.Count)
            {
                ShopContents[i].gameObject.SetActive(true);
                ShopContents[i].FetchContent(finalList[i]);
            }
            else
            {
                ShopContents[i].gameObject.SetActive(false);
            }
        }

        InitScrollbarValue(FoodAndShopScrollbar);
    }

    public void OnNoContentText(bool anyContent = false)
    {
        NoContentText.SetActive(anyContent);
        NoContentFooter.SetActive(anyContent);

        if (anyContent == false) Debug.Log("콘텐츠 없음");
    }

    public void FetchingContent(string str)
    {
        str = str.Trim(); // 공백 제거
        var shops = LoadManager.Instance.GetShopsBySearch(str);

        // 검색 결과가 있으면 텍스트와 푸터 숨김
        OnNoContentText(shops.Count == 0);

        for (int i = 0; i < ShopContentCreator.MaxContentCount; ++i)
        {
            if (i < shops.Count)
            {
                ShopContents[i].gameObject.SetActive(true);
                ShopContents[i].FetchContent(shops[i]);
            }
            else
            {
                ShopContents[i].gameObject.SetActive(false);
            }
        }
        InitScrollbarValue(FoodAndShopScrollbar);
    }

    public void InitScrollbarValue(Scrollbar scrollbar, bool isHorizon = false)
    {
        StartCoroutine(SetScrollbarToTopNextFrame(scrollbar, isHorizon));
    }
    private IEnumerator SetScrollbarToTopNextFrame(Scrollbar targetScrollbar, bool isHorizon = false)
    {
        yield return null; // 프레임 1
        yield return null; // 프레임 2

        //Canvas.ForceUpdateCanvases(); // 👈 레이아웃 즉시 강제 갱신

        if (isHorizon)
            targetScrollbar.value = 0;
        else
            targetScrollbar.value = 1f;

        Debug.Log("📌 스크롤바 위치 초기화 완료 (value = " + targetScrollbar.value + ")");
    }
    public void SelectSpecificCategory<T>(List<T> targetButtonList, int index) where T : MonoBehaviour  // 페이지 열때 첫번째 카테고리 자동으로 선택되기 위해
    {
        Debug.Log("첫번째 카테고리 선택");

        DeselectAllCustomButtons(targetButtonList);

        targetButtonList[index].GetComponent<ISelectableButton>().SetSelected(true);
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
                KioskLogger.Warn($"{btn.name}은(는) ISelectableButton 인터페이스를 구현하지 않았습니다.");
            }
        }
    }

    public void OffCategoryButton() //검색용
    {
        var secondCateStrList = 0;

        for (int i = 0; i < UIManager.SecondCategoryMaxCount; i++)
        {
            var secondCategoryButton = UIManager.Instance.SecondCategorieButtons[i];

            if (i < secondCateStrList)
            {
                secondCategoryButton.gameObject.SetActive(true);
            }
            else
            {
                secondCategoryButton.gameObject.SetActive(false);
            }
        }
    }

    public void CloseKeyboard()
    {
        if (keyboard.gameObject.activeSelf)
            keyboard.gameObject.SetActive(false);
    }
    public void OpenKeyboard()
    {
        if (keyboard.gameObject.activeSelf == false)
        {
            keyboard.gameObject.SetActive(true);
            keyboard.Reset();
        }
    }


    private void Update() // 입력없으면 초기화면으로
    {
        // 터치 입력 감지
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            lastTouchTime = Time.time;
            lastLoggedSecond = -1; // 로그 리셋
        }

        // 마우스 클릭 감지
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            lastTouchTime = Time.time;
            lastLoggedSecond = -1; // 로그 리셋
        }

        float elapsed = Time.time - lastTouchTime;
        float remaining = idleThreshold - elapsed;

        if (remaining > 0f)
        {
            int remainingSeconds = Mathf.CeilToInt(remaining);
            if (remainingSeconds != lastLoggedSecond)
            {
                //Debug.Log($"⏳ 초기화까지 남은 시간: {remainingSeconds}초");
                lastLoggedSecond = remainingSeconds;
            }
        }

        if (elapsed > idleThreshold)
        {
            if (PageStack.Count == 0 && nowLanguage == Language.Korean)
            {
                lastTouchTime = Time.time + 99999f;
                return;
            }

            Debug.Log("⏳ 일정 시간 입력 없음 → 초기화면으로 이동");
            ResetToHomeScreen();
            lastTouchTime = Time.time + 99999f;
            lastLoggedSecond = -1;
        }
    }
    private void ResetToHomeScreen()
    {
        NowLanguage = Language.Korean; // 언어 초기화

        if (PageStack.Count == 0)
        {
            return;
        }

        var photo = FindAnyObjectByType<Page_Photo>(); // 사진 예외처리
        if (0 < photo.GetComponent<CanvasGroup>().alpha)
        {
            var elgato = FindAnyObjectByType<ElgatoController>();
            elgato.StopElgato(); // 엘가토 카메라 정지
            elgato.StopAD(); // 엘가토 비디오 정지
            photo.InitPage(); // 사진 페이지 초기화
        }
        CloseAllPages(); // 또는 다른 초기화 함수

        VideoPlayManager.Instance.PlayVideo(VideoType.Default); // 기본 영상 등
        OpenKeyboard(); // 기본 입력 대기
    }

    public void PlayVideoByCategoryButton()
    {
        switch (NowSelectedCategory)
        {
            case Category_Base.Default:
                goto NonBase;

            case Category_Base.ToEat:
                if (VideoPlayManager.Instance.CurrentPlayingType == VideoType.ToEat_Category) return;
                VideoPlayManager.Instance.PlayVideo(VideoType.ToEat_Category);
                break;
            case Category_Base.ToBuy:
                if (VideoPlayManager.Instance.CurrentPlayingType == VideoType.ToBuy_Category) return;
                VideoPlayManager.Instance.PlayVideo(VideoType.ToBuy_Category);
                break;
            case Category_Base.ToGallery:
                if (VideoPlayManager.Instance.CurrentPlayingType == VideoType.ToGallery_Category) return;
                VideoPlayManager.Instance.PlayVideo(VideoType.ToGallery_Category);
                break;
            case Category_Base.ToHelp:
                if (VideoPlayManager.Instance.CurrentPlayingType == VideoType.ToHelp_Category) return;
                VideoPlayManager.Instance.PlayVideo(VideoType.ToHelp_Category);
                break;
            case Category_Base.ToStay:
                if (VideoPlayManager.Instance.CurrentPlayingType == VideoType.ToStay_Category) return;
                VideoPlayManager.Instance.PlayVideo(VideoType.ToStay_Category);
                break;
            default:
                break;
        }
        return;

    NonBase:
        switch (NowSelectedETC)
        {
            case Category_ETC.Default:
                break;
            case Category_ETC.Palace:
                break;
            case Category_ETC.HanbokExplain:
                break;
            case Category_ETC.Map:
                break;
            case Category_ETC.Here:
                break;
            case Category_ETC.Greeting:
                break;
            case Category_ETC.Photo:
                if (VideoPlayManager.Instance.CurrentPlayingType == VideoType.Photo_SelectHanbok) return;
                VideoPlayManager.Instance.PlayVideo(VideoType.Photo_SelectHanbok);
                break;
            case Category_ETC.Event:
                VideoPlayManager.Instance.PlayVideo(VideoType.Event_Category);
                break;
            case Category_ETC.Mission:
                break;
            case Category_ETC.Exchange:
                break;
            case Category_ETC.Transport:
                break;
            default:
                break;
        }
    }

    public void PlayVideoByMainButton() // 메인버튼 눌렀을때
    {
        if (NowSelectedCategory == Category_Base.ToHelp && NowSelectedETC == Category_ETC.Toilet)
        {
            VideoPlayManager.Instance.PlayVideo(VideoType.Toilet);
            return;
        }

        switch (NowSelectedCategory)
        {
            case Category_Base.Default:
                goto NonBase;
            case Category_Base.ToEat:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToEat);
                break;
            case Category_Base.ToBuy:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToBuy);
                break;
            case Category_Base.ToGallery:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToGallery);
                break;
            case Category_Base.ToHelp:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToHelp);
                break;
            case Category_Base.ToStay:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToStay);
                break;
            default:
                break;
        }

    NonBase:
        switch (NowSelectedETC)
        {
            case Category_ETC.Palace:
                VideoPlayManager.Instance.PlayVideo(VideoType.Palace);
                break;
            case Category_ETC.HanbokExplain:
                VideoPlayManager.Instance.PlayVideo(VideoType.HanbokExplain);
                break;
            case Category_ETC.Map:
                VideoPlayManager.Instance.PlayVideo(VideoType.Map);
                break;
            case Category_ETC.Here:
                VideoPlayManager.Instance.PlayVideo(VideoType.Here);
                break;
            case Category_ETC.Greeting:
                VideoPlayManager.Instance.PlayVideo(VideoType.Greeting);
                break;
            case Category_ETC.Photo:
                VideoPlayManager.Instance.PlayVideo(VideoType.Photo);
                break;
            case Category_ETC.Event:
                VideoPlayManager.Instance.PlayVideo(VideoType.Event);
                break;
            case Category_ETC.Mission:
                VideoPlayManager.Instance.PlayVideo(VideoType.Mission);
                break;
            case Category_ETC.Exchange:
                VideoPlayManager.Instance.PlayVideo(VideoType.Exchange);
                break;
            case Category_ETC.Transport:
                VideoPlayManager.Instance.PlayVideo(VideoType.Transport);
                break;
            case Category_ETC.MarketPaper:
                VideoPlayManager.Instance.PlayVideo(VideoType.MarketPaper);
                break;
            case Category_ETC.TaxFree:
                VideoPlayManager.Instance.PlayVideo(VideoType.TaxFree);
                break;
            default:
                break;
        }
    }

    public void PlayVideoByDetail()
    {
        switch (NowSelectedCategory)
        {
            case Category_Base.Default:
                goto NonBase;

            case Category_Base.ToEat:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToEat_Detail);
                break;
            case Category_Base.ToBuy:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToBuy_Detail);
                break;
            case Category_Base.ToGallery:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToGallery_Detail);
                break;
            case Category_Base.ToHelp:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToHelp_Detail);
                break;
            case Category_Base.ToStay:
                VideoPlayManager.Instance.PlayVideo(VideoType.ToStay_Detail);
                break;
            default:
                break;
        }
        return;

    NonBase:
        switch (NowSelectedETC)
        {
            case Category_ETC.Default:
                break;
            case Category_ETC.Palace:
                VideoPlayManager.Instance.PlayVideo(VideoType.Palace_Detail);
                break;
            case Category_ETC.HanbokExplain:
                break;
            case Category_ETC.Map:
                break;
            case Category_ETC.Here:
                break;
            case Category_ETC.Greeting:
                break;
            case Category_ETC.Photo:
                break;
            case Category_ETC.Event:
                break;
            case Category_ETC.Mission:
                break;
            case Category_ETC.Exchange:
                break;
            case Category_ETC.Transport:
                break;
            case Category_ETC.Search:
                VideoPlayManager.Instance.PlayVideo(VideoType.Search_Detail);
                break;
            default:
                break;
        }
    }

    public void OnToStayText(bool isOn = false) // 숙박안내 예외처리용
    {
        ToStayOnlyText.SetActive(isOn);
        ToStayOnlyText2.SetActive(isOn);
    }
}

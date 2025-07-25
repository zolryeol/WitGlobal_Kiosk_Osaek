using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// 스프레드 시트에서 데이터를 불러와서 저장할 예정
public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }
    [SerializeField]
    private List<BaseShopInfoData> shopList = new();

    [SerializeField]
    private List<AICategory> aiCategorieList = new();
    [SerializeField]
    private List<PalaceData> palaceList = new();
    [SerializeField]
    private List<EventData> eventList = new();
    [SerializeField]
    private List<LocalizationText> locaizationList = new();
    [SerializeField]
    private List<VideoSubtitleData> videoSubTitleList = new();

    public List<VideoSubtitleData> VideoSubTitleList => videoSubTitleList;

    public List<BaseShopInfoData> ShopList => shopList;
    public List<AICategory> AICategorieList => aiCategorieList;
    public List<PalaceData> PalaceDataList => palaceList;
    public List<EventData> EventDataList => eventList;
    public List<LocalizationText> LocalizeTextList => locaizationList;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Core에서 호출

    public async Task Init()
    {
        await LoadShopInfoDataAsync();
        await LoadAICategoryDataAsync();
        await LoadPalaceInfoDataAsync();
        await LoadEventDataAsync();
        await LoadLocalizationTextAsync();
        await LoadVideoSubTitleDataAsync();

        ResourceManager.Instance.BuildVideoMapFromSubtitleList(videoSubTitleList);

        Debug.Log("<color=green>[Shop 매니저] 로드 완료</color>");
    }


    private async Task LoadVideoSubTitleDataAsync()
    {
        var sheet = await GoogleSheetReader.ReadVideoSubTitleDataRange();
        if (sheet != null)
        {
            // 비디오 자막 데이터 파싱
            videoSubTitleList = ShopSheetParser.VideoSubTitleParse(sheet);
            Debug.Log($"[ShopManager] 비디오 자막 데이터 {videoSubTitleList.Count}개 로드 완료");
        }
        else
        {
            Debug.LogError("[ShopManager] 비디오 자막 데이터를 불러오지 못했습니다.");
        }

    }

    private async Task LoadShopInfoDataAsync()
    {
        var sheet = await GoogleSheetReader.ReadShopDataSheetAsync();
        if (sheet != null)
        {
            shopList = ShopSheetParser.ShopDataParse(sheet);
            Debug.Log($"[ShopManager] 상점 데이터 {shopList.Count}개 로드 완료");
        }
        else
        {
            Debug.LogError("[ShopManager] 상점 데이터를 불러오지 못했습니다.");
        }
    }

    private async Task LoadAICategoryDataAsync()
    {
        var sheet = await GoogleSheetReader.ReadAICategoryAsync();
        if (sheet != null)
        {
            // AICategory 데이터 파싱
            aiCategorieList = ShopSheetParser.AICategoryParser(sheet);
            Debug.Log($"[ShopManager] AI 카테고리 데이터 {aiCategorieList.Count}개 로드 완료");
        }
        else
        {
            Debug.LogError("[ShopManager] AI 카테고리 데이터를 불러오지 못했습니다.");
        }
    }

    private async Task LoadEventDataAsync()
    {
        var sheet = await GoogleSheetReader.ReadEventInfoDataRange();

        if (sheet != null)
        {
            eventList = ShopSheetParser.EventDataParser(sheet);
            Debug.Log($"[ShopManager] 이벤트 데이터 {eventList.Count}개 로드 완료");
        }
        else
        {
            Debug.LogError("[ShopManager] 이벤트 데이터를 불러오지 못했습니다.");
        }

        // 이미지불러오기
        //foreach (var item in eventList)
        //{
        //    Sprite sprite = await LoadEventThumbnailSpriteAsync(item);
        //    item.ThumbNailImage = sprite;
        //}
    }

    // 코루틴을 Task로 래핑
    private Task<Sprite> LoadEventThumbnailSpriteAsync(EventData data)
    {
        var tcs = new TaskCompletionSource<Sprite>();
        StartCoroutine(LoadEventThumbnailSprite(data, sprite => tcs.SetResult(sprite)));
        return tcs.Task;
    }

    private async Task LoadLocalizationTextAsync()
    {
        var sheet = await GoogleSheetReader.ReadLocalizationDataRange();
        if (sheet != null)
        {
            // 고궁 정보 데이터 파싱
            locaizationList = ShopSheetParser.LocalizationTextParse(sheet);
            Debug.Log($"[ShopManager] 현지화 데이터 로드 완료");
        }
        else
        {
            Debug.Log($"[ShopManager] 현지화 데이터 불러오지 못했습니다.");
        }
    }

    private async Task LoadPalaceInfoDataAsync()
    {
        var sheet = await GoogleSheetReader.ReadPalaceInfoDataRange();
        if (sheet != null)
        {
            // 고궁 정보 데이터 파싱
            palaceList = ShopSheetParser.PalaceDataParser(sheet);
            Debug.Log($"[ShopManager] 고궁 데이터 로드 완료");
        }
        else
        {
            Debug.Log($"[ShopManager] 고궁 데이터 불러오지 못했습니다.");
        }
    }

    public BaseShopInfoData GetShopById(int shopId)
    {
        return shopList.Find(shop => shop.ShopID == shopId);
    }

    public int GetCountByBaseCategory(Category_Base categoryBase)
    {
        var baseCategoryStr = CommonFunction.GetBaseCategoryString(categoryBase); // 카테고리 문자열을 얻기 위해 호출
        return ShopList.Count(shop => shop.BaseCategoryString[(int)Language.Korean] == baseCategoryStr);
    }

    // Category Base에서 가장 많은 count를 가진 카테고리의 개수를 반환
    public int GetBaseCategoryMaxCount()
    {
        if (ShopList.Count == 0)
        {
            return 0;
        }
        // 각 카테고리의 개수를 세기
        var categoryCounts = ShopList.GroupBy(shop => shop.BaseCategoryString[(int)Language.Korean])
                                      .Select(group => new { Category = group.Key, Count = group.Count() });
        // 가장 많은 개수를 가진 카테고리 찾기
        var maxCount = categoryCounts.Max(group => group.Count);
        return maxCount;
    }
    public int GetEventMaxCount()
    {
        if (eventList.Count == 0) return 0;

        var eventCount = eventList.GroupBy(e => e.EventState).Select(group => new { State = group.Key, Count = group.Count() });

        var maxCount = eventCount.Max(group => group.Count);

        return maxCount;
    }

    public int GetSecondCategoryCount(Category_Base baseCategory)
    {
        var baseCategoryStr = CommonFunction.GetBaseCategoryString(baseCategory);
        var secondCategorySet = new HashSet<string>();

        foreach (var shop in ShopList)
        {
            if (shop.BaseCategoryString[(int)Language.Korean] == baseCategoryStr)
            {
                var raw = shop.SecondCategoryString[(int)Language.Korean];
                if (!string.IsNullOrEmpty(raw) && raw.Contains("-"))
                {
                    var parts = raw.Split('-');
                    if (parts.Length > 1)
                    {
                        var categoryName = parts[1].Trim();
                        if (!string.IsNullOrEmpty(categoryName))
                            secondCategorySet.Add(categoryName);
                    }
                }
            }
        }
        return secondCategorySet.Count;
    }

    // 현재 언어에따라서 secondcategory문자를 반환 ("xxx-xx"  -이후의 문자만)
    public List<(int, string)> GetSecondCategoryStringTrim(Category_Base baseCategory)
    {
        var baseCategoryStr = CommonFunction.GetBaseCategoryString(baseCategory);
        var nowLanguage = UIManager.Instance.NowLanguage;

        var categoryList = new List<(int priority, string categoryName)>();

        foreach (var shop in ShopList)
        {
            if (shop.BaseCategoryString[(int)Language.Korean] == baseCategoryStr)
            {
                var raw = shop.SecondCategoryString[(int)nowLanguage];

                if (raw.Contains("-"))
                {
                    var parts = raw.Split('-');
                    if (int.TryParse(parts[0].Trim(), out int number))
                    {
                        categoryList.Add((number, parts[1].Trim()));
                    }
                }
                //else
                //{
                //    // '-' 없는 경우: 우선순위를 가장 낮게 설정
                //    categoryList.Add((int.MaxValue, raw.Trim()));
                //}
            }
        }

        // 중복 제거 후 내림차순 정렬
        var sorted = categoryList
            .Distinct() // 중복 제거
            .OrderBy(x => x.priority)
            .ToList();

        return sorted;
    }

    public List<(int, string)> GetSecondCategoryList(Category_Base baseCategory)
    {
        var baseCategoryStr = CommonFunction.GetBaseCategoryString(baseCategory);

        var categoryList = new List<(int priority, string categoryName)>();

        foreach (var shop in ShopList)
        {
            if (shop.BaseCategoryString[(int)Language.Korean] == baseCategoryStr)
            {
                var raw = shop.SecondCategoryString[(int)Language.Korean];

                if (raw.Contains("-"))
                {
                    var parts = raw.Split('-');
                    if (int.TryParse(parts[0].Trim(), out int number))
                    {
                        categoryList.Add((number, raw));
                    }
                }
            }
        }

        // 중복 제거 후 내림차순 정렬
        var sorted = categoryList
            .Distinct() // 중복 제거
            .OrderBy(x => x.priority)
            .ToList();

        return sorted;
    }


    // Category_Base 에따라, // secondCategoryIndex 번째의 상점들을 반환
    public List<BaseShopInfoData> GetShopsBySecondCategory(Category_Base baseCategory, int secondCategoryIndex)
    {
        var baseCategoryStr = CommonFunction.GetBaseCategoryString(baseCategory);
        var secondCategoryList = GetSecondCategoryList(baseCategory);

        var targetString = secondCategoryList[secondCategoryIndex].Item2;

        return ShopList.Where(shop => shop.BaseCategoryString[(int)Language.Korean] == baseCategoryStr &&
                                      shop.SecondCategoryString[(int)Language.Korean].Contains(targetString)).ToList();
    }

    public List<EventData> GetEventByState(EventState state)
    {
        return eventList.Where(e => e.EventState == state).ToList();
    }

    public List<BaseShopInfoData> GetShopsByAICategory(AICategory _aicategory)
    {
        if (_aicategory == null)
        {
            Debug.LogError("[ShopManager] AICategory가 null입니다.");
            return new List<BaseShopInfoData>(); // 빈 리스트 반환
        }

        return ShopList.Where(shop =>
            shop.AICategoryString[(int)Language.Korean] == _aicategory.AICategoryString[(int)Language.Korean] ||
            shop.SecondCategoryString[(int)Language.Korean] == _aicategory.AICategoryString[(int)Language.Korean]
        ).Distinct().ToList();
    }

    //public List<BaseShopInfoData> GetShopsByAICategory(AICategory _aicategory, ShopSortType sortType = ShopSortType.Default)
    //{
    //    if (_aicategory == null)
    //    {
    //        Debug.LogError("[ShopManager] AICategory가 null입니다.");
    //        return new List<BaseShopInfoData>();
    //    }

    //    var filtered = ShopList
    //        .Where(shop =>
    //            shop.AICategoryString[(int)Language.Korean] == _aicategory.AICategoryString[(int)Language.Korean] ||
    //            shop.SecondCategoryString[(int)Language.Korean] == _aicategory.AICategoryString[(int)Language.Korean]
    //        )
    //        .Distinct();

    //    switch (sortType)
    //    {
    //        case ShopSortType.NaverRatingHigh:
    //            return filtered.OrderByDescending(shop => shop.NaverRating).ToList();
    //        case ShopSortType.NaverRatingLow:
    //            return filtered.OrderBy(shop => shop.NaverRating).ToList();
    //        case ShopSortType.Random:
    //            return filtered.OrderBy(_ => UnityEngine.Random.value).ToList();
    //        default:
    //            return filtered.ToList();
    //    }
    //}

    public List<BaseShopInfoData> GetShopsBySearch(string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString) || searchString.Length < 1)
        {
            Debug.Log("검색창 비어있다");
            return new List<BaseShopInfoData>();
        }

        var result = new List<BaseShopInfoData>();
        var added = new HashSet<BaseShopInfoData>();

        // 검색 대상 언어 인덱스
        int[] searchLangs;

        switch (UIManager.Instance.NowLanguage)
        {
            case Language.Korean:
                searchLangs = new[] { (int)Language.Korean, (int)Language.English };
                break;
            case Language.English:
            case Language.Japanese:
            case Language.Chinese:
                searchLangs = new[] { (int)Language.English, (int)Language.Korean };
                break;
            default:
                searchLangs = new[] { (int)Language.English, (int)Language.Korean };
                break;
        }

        foreach (var shop in shopList)
        {
            foreach (int langIdx in searchLangs)
            {
                string name = shop.ShopName[langIdx];

                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogWarning($"shopID={shop.ShopID}의 {((Language)langIdx)} 이름이 null 또는 비어 있음");
                    continue;
                }
                // 완전 일치
                if (string.Equals(name, searchString, StringComparison.OrdinalIgnoreCase))
                {
                    if (added.Add(shop))
                        result.Add(shop);
                    break;
                }

                // 앞부분 일치
                if (name.StartsWith(searchString, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(name, searchString, StringComparison.OrdinalIgnoreCase))
                {
                    if (added.Add(shop))
                        result.Add(shop);
                    break;
                }

                // 부분 포함
                if (name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 &&
                    !name.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                {
                    if (added.Add(shop))
                        result.Add(shop);
                    break;
                }
            }
        }

        return result;
    }
    public BaseShopInfoData GetShopInfoByShopName(string _shopName)
    {
        var nowLang = (int)UIManager.Instance.NowLanguage;

        return ShopList.FirstOrDefault(shop => shop.ShopName[nowLang] == _shopName);
    }

    public List<BaseShopInfoData> GetAICategoryString()
    {
        return null;
    }
    public IEnumerator LoadEventThumbnailSprite(EventData data, System.Action<Sprite> onLoaded)
    {
        if (string.IsNullOrEmpty(data.ImageUrl))
        {
            onLoaded?.Invoke(null);
            yield break;
        }

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(data.ImageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                onLoaded?.Invoke(sprite);
            }
            else
            {
                Debug.LogWarning("썸네일 이미지 로드 실패: " + uwr.error);
                onLoaded?.Invoke(null);
            }
        }
    }
}

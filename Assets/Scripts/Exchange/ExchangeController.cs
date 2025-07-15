using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class ExchangeController : MonoBehaviour
{
    // 오늘 날짜 텍스트
    [SerializeField]
    private TextMeshProUGUI date;

    // 환율 프리펩
    [SerializeField]
    private GameObject exchangeInfo;

    // 라인 프리팹
    [SerializeField]
    private GameObject linePrefab;

    // 프리팹 생성 위치
    [SerializeField]
    private Transform targetTransform;

    // 상승, 하락 아이콘
    [SerializeField]
    private Sprite upArrow;
    [SerializeField]
    private Sprite downArrow;

    // 국기 아이콘들, 아래 배열 기준으로 똑같이 순서 맞춤
    [SerializeField]
    private Sprite[] countryIcons;

    // API 파라미터 값
    private string authkey;
    private string data;

    /* 현재 가져오고 싶은 통화들 
        AUD 호주 달러
        CAD 캐나다 달러
        CNH 위안화
        GBP 영국 파운드
        HKD 홍콩 달러
        JPY(100) 일본 옌
        SAR 사우디 리얄
        THB 태국 바트
        USD 미국 달러
 */
    private string[] targetExchangeArr = new string[]
    {
        "AUD",
        "CAD",
        "CNH",
        "GBP",
        "HKD",
        "JPY(100)",
        "SAR",
        "THB",
        "USD"
    };

    private void Awake()
    {
        // authkey = "m6gKD2pIabqoHLvMChUonPZLHsqAozYa";
        authkey = "pttChPuezkTVoIybdPgYZYSJulNWpArp";
        data = "AP01";
    }

    private void Start()
    {
        //date.text = DateTime.Now.ToString("yyyy.MM.dd HH시 mm분") + " 기준";

        //switch (LanguageService.getCurrentLanguage())
        //{
        //    case "KR":
        //        date.text = DateTime.Now.ToString("yyyy.MM.dd HH시 mm분") + " 기준";
        //        break;
        //    case "EN":
        //        date.text = DateTime.Now.ToString("MMMM d, yyyy, h:mm tt", new System.Globalization.CultureInfo("en-US")) + " As of";
        //        break;
        //    case "JP":
        //        date.text = DateTime.Now.ToString("yyyy年MM月dd日HH時mm分現在", new System.Globalization.CultureInfo("ja-JP"));
        //        break;
        //    case "CN":
        //        date.text = DateTime.Now.ToString("yyyy年MM月dd日HH时mm分基准", new System.Globalization.CultureInfo("zh-CN"));
        //        break;
        //}

        //// 페이지 번역
        //languageChange();

        // API 호출  
        FetchExchangeAPI();

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

        
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    private async void FetchExchangeAPI()
    {
        var (currentExchange, yesterdayExchange) = await FetchExchangeData();

        //Debug.LogError($"Today : {currentExchange} Yesterday : {yesterdayExchange}");

        if (currentExchange != null && yesterdayExchange != null)
        {
            UpdateExchangeUI(currentExchange, yesterdayExchange);
        }
        else
        {
            Debug.LogError("Failed to fetch exchange data.");
        }
    }

    private async Task<(List<ExchangeData> currentExchange, List<ExchangeData> yesterdayExchange)> FetchExchangeData()
    {
        // 환율 데이터 리스트
        List<ExchangeData> currentExchange = null;
        List<ExchangeData> yesterdayExchange = null;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // 오늘자 환율 가져오기
                currentExchange = await GetExchangeData(client, "today");
                if (currentExchange == null || currentExchange.Count == 0)
                {
                    currentExchange = await GetExchangeData(client, "yesterday");
                    if (currentExchange == null || currentExchange.Count == 0) return (null, null);

                    yesterdayExchange = await GetExchangeData(client, "twodays");
                    if (yesterdayExchange == null || yesterdayExchange.Count == 0) return (null, null);
                }
                else
                {
                    // 어제자 환율 가져오기
                    yesterdayExchange = await GetExchangeData(client, "yesterday");
                    if (yesterdayExchange == null || yesterdayExchange.Count == 0)
                    {
                        yesterdayExchange = await GetExchangeData(client, "twodays");
                        if (yesterdayExchange == null || yesterdayExchange.Count == 0) return (null, null);
                    }
                }


                return (currentExchange, yesterdayExchange);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError(e);
                return (null, null);
            }
        }
    }

    private async Task<List<ExchangeData>> GetExchangeData(HttpClient client, string type)
    {
        List<ExchangeData> exchangeData = null;

        int adjust = 0;
        while (exchangeData == null || exchangeData.Count == 0)
        {
            string dateStr = DateTime.Now.AddDays(adjust).ToString("yyyyMMdd");
            string url = GlobalManager.Instance.domain + GlobalManager.Instance.exchange_rate_api +"/"+ type;
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            exchangeData = JsonConvert.DeserializeObject<List<ExchangeData>>(responseBody);

            if (exchangeData == null || exchangeData.Count == 0)
                adjust--;

            if (adjust <= -7) break;
        }

        return exchangeData;
    }


    private void UpdateExchangeUI(List<ExchangeData> currentExchange, List<ExchangeData> yesterdayExchange)
    {
        if (currentExchange == null || yesterdayExchange == null) return;

        string won = GetCurrencySymbol(LanguageService.getCurrentLanguage());
        for (int i = 0; i < currentExchange.Count; i++)
        {
            string currency = GetCurrencyName(currentExchange[i].cur_unit, LanguageService.getCurrentLanguage());

            // 만약 필요없는 통화면 패스
            if (Array.IndexOf(targetExchangeArr, currentExchange[i].cur_unit) == -1) continue;

            // 환율 프리팹 생성
            GameObject instance = Instantiate(exchangeInfo, targetTransform);
            Instantiate(linePrefab, targetTransform);

            // 아이콘 설정
            instance.transform.GetChild(0).GetComponent<Image>().sprite = countryIcons[Array.IndexOf(targetExchangeArr, currentExchange[i].cur_unit)];

            // 이름 설정
            instance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{currency} {currentExchange[i].cur_unit}";

            // 금액 설정
            instance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{currentExchange[i].deal_bas_r}" + won;

            // 전날 및 오늘 매매기준율 가져오기
            float yesterdayRate = float.Parse(yesterdayExchange[i].deal_bas_r.Replace(",", ""));
            float currentRate = float.Parse(currentExchange[i].deal_bas_r.Replace(",", ""));

            // 변화율 계산
            float rateChange = currentRate - yesterdayRate;
            float rateChangePercent = (rateChange / yesterdayRate) * 100;

            // 상승 또는 하락 판단
            bool isUp = rateChange > 0;

            // 전날대비 상승, 하락에 따른 화살표 이미지 변경 및 텍스트 색 변경
            Sprite sprite = isUp ? upArrow : downArrow;
            instance.transform.GetChild(3).GetComponent<Image>().sprite = sprite;
            Color color = isUp ? new Color(246f / 255f, 86f / 255f, 70f / 255f) : new Color(61f / 255f, 109f / 255f, 235f / 255f);
            instance.transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = color;

            // 전날대비 수치 및 퍼센테이지 적용
            instance.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(rateChange):F2} ({rateChangePercent:F2}%)";
        }
    }

    private string GetCurrencySymbol(string language)
    {
        switch (language)
        {
            case "KR": return "원";
            case "EN": return "won";
            case "JP": return "ウォン";
            case "CN": return "韩元";
            default: return "won";
        }
    }

    private string GetCurrencyName(string curUnit, string language)
    {
        switch (language)
        {
            case "KR":
                return curUnit switch
                {
                    "AUD" => "호주달러",
                    "CAD" => "캐나다 달러",
                    "CNH" => "위안화",
                    "GBP" => "영국 파운드",
                    "HKD" => "홍콩 달러",
                    "JPY" => "일본 엔",
                    "SAR" => "사우디 리얄",
                    "THB" => "태국 바트",
                    "USD" => "미국 달러",
                    _ => curUnit
                };
            case "EN":
                return curUnit switch
                {
                    "AUD" => "Australian Dollar",
                    "CAD" => "Canadian Dollar",
                    "CNH" => "Renminbi",
                    "GBP" => "British Pound",
                    "HKD" => "Hong Kong Dollar",
                    "JPY(100)" => "Japanese Yen",
                    "SAR" => "Saudi Riyal",
                    "THB" => "Thai Baht",
                    "USD" => "United States Dollar",
                    _ => curUnit
                };
            case "JP":
                return curUnit switch
                {
                    "AUD" => "オーストラリアドル",
                    "CAD" => "カナダドル",
                    "CNH" => "人民元",
                    "GBP" => "イギリスポンド",
                    "HKD" => "香港ドル",
                    "JPY" => "日本円",
                    "SAR" => "サウジリヤル",
                    "THB" => "タイバーツ",
                    "USD" => "アメリカドル",
                    _ => curUnit
                };
            case "CN":
                return curUnit switch
                {
                    "AUD" => "澳大利亚元",
                    "CAD" => "加拿大元",
                    "CNH" => "人民币",
                    "GBP" => "英镑",
                    "HKD" => "港元",
                    "JPY" => "日元",
                    "SAR" => "沙特里亚尔",
                    "THB" => "泰铢",
                    "USD" => "美元",
                    _ => curUnit
                };
            default:
                return curUnit;
        }
    }


    private void languageChange()
    {
        LanguageService ls = new LanguageService();
        ls.exchangeSceneInit();
        string currentLanguage = LanguageService.getCurrentLanguage();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("exchange");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray)
        {
            // key와 동일한 이름의 GameObject를 찾음
            GameObject targetObject = GameObject.Find(key);
            if (targetObject != null)
            {
                // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
                    // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
                    textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용                    
                }
                else
                {
                    Debug.LogWarning($"TextMeshProUGUI component not found in {key}");
                }
            }
            else
            {
                Debug.LogWarning($"GameObject with name {key} not found");
            }
        }
    }

    // API 응답 클래스
    public class ExchangeData
    {
        public int result { get; set; }
        public string cur_unit { get; set; }
        public string ttb { get; set; }
        public string tts { get; set; }
        public string deal_bas_r { get; set; }
        public string bkpr { get; set; }
        public string cur_nm { get; set; }
    }
}
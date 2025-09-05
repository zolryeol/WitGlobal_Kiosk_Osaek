using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherAPI_JH : MonoBehaviour
{
    public static WeatherAPI_JH Instance { get; private set; }
    public VideoType WeatherVideoType { get => weatherVideoType; }

    [SerializeField] private TextMeshProUGUI curTemperature;
    [SerializeField] private TextMeshProUGUI maxlowTemperature;
    [SerializeField] private Image curWeartherImage;
    [SerializeField] private Sprite sunnySprite;
    [SerializeField] private Sprite cloudySprite;
    [SerializeField] private Sprite rainSprite;
    [SerializeField] private Sprite snowSprite;
    [SerializeField] private Sprite thunderSprite;
    [SerializeField] private Sprite hailSprite;

    VideoType weatherVideoType = VideoType.Weather_Sunny;

    private static readonly HttpClient client = new HttpClient();

    private readonly string apiKey = "75341a6d64eb213493a7ffa4cdfe348e";
    //private readonly string apiUrl = "https://api.openweathermap.org/data/2.5/weather?lat=37.5744&lon=126.9849&units=metric&lang=en";  // 경도 위도 참조해서 가져옵니다. // 인사동 좌표
    private readonly string apiUrl = "https://api.openweathermap.org/data/2.5/weather?lat=37.1527&lon=127.0699&units=metric&lang=en";  // 경도 위도 참조해서 가져옵니다. // 오색시장 좌표
    //확인용
    //https://api.openweathermap.org/data/2.5/weather?lat=37.5744&lon=126.9849&units=metric&lang=kr&appid=75341a6d64eb213493a7ffa4cdfe348e

    private static WeatherResponse cachedWeather = null;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(UpdateWeatherEvery30Minutes());
    }

    private void ApplyWeatherToUI(WeatherResponse weather)
    {
        SetWeatherIcons(weather.weather[0].description, curWeartherImage);
        curTemperature.text = $"{Mathf.RoundToInt(weather.main.temp)}°";
        maxlowTemperature.text = $"{Mathf.RoundToInt(weather.main.temp_min)}˚ / {Mathf.RoundToInt(weather.main.temp_max)}˚";
    }

    private async Task<WeatherResponse> GetCurrentWeatherAsync()
    {
        string fullUrl = $"{apiUrl}&appid={apiKey}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();
            string results = await response.Content.ReadAsStringAsync();

            var weatherData = JsonConvert.DeserializeObject<WeatherResponse>(results);
            return weatherData;
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError($"🌧 날씨 API 요청 실패: {ex.Message}");
            return null;
        }
    }
    private void SetWeatherIcons(string mainKeyword, Image img)
    {
        switch (mainKeyword)
        {
            case "Clear":
                img.sprite = sunnySprite;
                weatherVideoType = VideoType.Weather_Sunny;
                break;
            case "Clouds": img.sprite = cloudySprite; break;
            case "Rain":
            case "Drizzle":
                img.sprite = rainSprite;
                weatherVideoType = VideoType.Weather_Rain;
                break;
            case "Snow":
                img.sprite = snowSprite;
                weatherVideoType = VideoType.Weather_Cold;
                break;
            case "Thunderstorm": img.sprite = thunderSprite; break;
            case "Tornado":
            case "Squall": img.sprite = hailSprite; break;
            default:
                Debug.LogWarning($"알 수 없는 날씨 main: {mainKeyword}");
                break;
        }
    }

    private int getUnixTime()
    {
        var now = DateTime.Now.ToLocalTime();
        var span = now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        return (int)span.TotalSeconds;
    }

    private IEnumerator UpdateWeatherEvery30Minutes()
    {
        while (true)
        {
            yield return UpdateWeatherOnce();

            // 30분 대기 (1800초)
            yield return new WaitForSeconds(1800f);
        }
    }

    private IEnumerator UpdateWeatherOnce()
    {
        var weatherTask = GetCurrentWeatherAsync();

        while (!weatherTask.IsCompleted)
            yield return null;

        var currentWeather = weatherTask.Result;

        if (currentWeather != null)
        {
            cachedWeather = currentWeather;
            ApplyWeatherToUI(currentWeather);
        }
        else if (cachedWeather != null)
        {
            Debug.LogWarning("✅ API 실패 → 마지막 저장된 날씨 데이터를 사용합니다.");
            ApplyWeatherToUI(cachedWeather);
        }
        else
        {
            Debug.LogWarning("❌ 현재 날씨 정보를 가져올 수 없고, 저장된 데이터도 없습니다.");
        }
        yield break;
    }
}

[System.Serializable]
public class WeatherResponse
{
    public List<WeatherInfo> weather;
    public MainInfo main;
    public long dt;
}

[System.Serializable]
public class WeatherInfo
{
    public string description;
    public string icon;
}

[System.Serializable]
public class MainInfo
{
    public float temp;
    public float temp_min;
    public float temp_max;
}

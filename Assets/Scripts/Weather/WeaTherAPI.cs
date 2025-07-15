using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeatherAPI_JH : MonoBehaviour
{
    public static WeatherAPI_JH Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI curTemperature;
    [SerializeField] private TextMeshProUGUI maxlowTemperature;
    [SerializeField] private Image curWeartherImage;
    [SerializeField] private Sprite sunnySprite;
    [SerializeField] private Sprite cloudySprite;
    [SerializeField] private Sprite rainSprite;
    [SerializeField] private Sprite snowSprite;
    [SerializeField] private Sprite thunderSprite;
    [SerializeField] private Sprite hailSprite;
    public Image[] weatherImages;

    private static readonly HttpClient client = new HttpClient();

    private readonly string apiKey = "75341a6d64eb213493a7ffa4cdfe348e";
    private readonly string apiUrl = "https://api.openweathermap.org/data/2.5/weather?lat=37.5744&lon=126.9849&units=metric&lang=kr";  // 경도 위도 참조해서 가져옵니다.

    private static WeatherItem cachedWeatherItem = null;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        StartCoroutine(UpdateWeatherEvery30Minutes());
    }

    private void ApplyWeatherToUI(WeatherItem weather)
    {
        SetWeatherIcons(weather.weather[0].description, curWeartherImage);
        curTemperature.text = $"{Mathf.RoundToInt((float)weather.main.temp)}°";
        maxlowTemperature.text = $"{Mathf.RoundToInt((float)weather.main.temp_min)}˚ / {Mathf.RoundToInt((float)weather.main.temp_max)}˚";
    }

    private async Task<WeatherItem> GetCurrentWeatherAsync()
    {
        string fullUrl = $"{apiUrl}&appid={apiKey}";
        try
        {
            HttpResponseMessage response = await client.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();
            string results = await response.Content.ReadAsStringAsync();

            var weatherItem = JsonConvert.DeserializeObject<WeatherItem>(results);
            return weatherItem;
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError($"현재 날씨 API 요청 실패: {ex.Message}");
            return null;
        }
    }


    private WeatherItem getCurrentWeatherData(List<WeatherItem> weatherDataList)
    {
        int currentUnixTime = getUnixTime();
        DateTime currentTime = DateTimeOffset.FromUnixTimeSeconds(currentUnixTime).DateTime;
        Debug.Log($"🕒 현재 시각: {currentTime}");

        WeatherItem closestFutureItem = null;
        int minFutureDiff = int.MaxValue;

        int minTemp = int.MaxValue;
        int maxTemp = int.MinValue;

        foreach (var item in weatherDataList)
        {
            // 디버깅 출력
            DateTime forecastTime = DateTimeOffset.FromUnixTimeSeconds(item.dt).DateTime;
            Debug.Log($"🔍 예보 시간: {forecastTime}, 온도: {item.main.temp}°C");

            // 온도 통계 계산
            int tempMin = Mathf.RoundToInt((float)item.main.temp_min);
            int tempMax = Mathf.RoundToInt((float)item.main.temp_max);
            if (minTemp > tempMin) minTemp = tempMin;
            if (maxTemp < tempMax) maxTemp = tempMax;

            int timeDiff = (int)(item.dt - currentUnixTime);

            // 현재 시각 이후 항목만 선택
            if (timeDiff >= 0 && timeDiff < minFutureDiff)
            {
                minFutureDiff = timeDiff;
                closestFutureItem = item;
            }
        }

        if (closestFutureItem == null && weatherDataList.Count > 0)
        {
            closestFutureItem = weatherDataList.Last();
        }

        if (closestFutureItem != null)
        {
            closestFutureItem.main.temp_min = minTemp;
            closestFutureItem.main.temp_max = maxTemp;
        }

        return closestFutureItem;
    }

    private void SetWeatherIcons(string description, Image img)
    {
        if (description.Contains("맑음")) img.sprite = sunnySprite;
        else if (description.Contains("구름")) img.sprite = cloudySprite;
        else if (description.Contains("비")) img.sprite = rainSprite;
        else if (description.Contains("눈")) img.sprite = snowSprite;
        else if (description.Contains("천둥")) img.sprite = thunderSprite;
        else if (description.Contains("우박")) img.sprite = hailSprite;
        else Debug.LogWarning($"알 수 없는 날씨 설명: {description}");
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
            cachedWeatherItem = currentWeather;  // ✅ 성공 시 캐시
            ApplyWeatherToUI(currentWeather);
        }
        else if (cachedWeatherItem != null)
        {
            Debug.LogWarning("✅ API 실패 → 마지막 저장된 날씨 데이터를 사용합니다.");
            ApplyWeatherToUI(cachedWeatherItem);
        }
        else
        {
            Debug.LogWarning("❌ 현재 날씨 정보를 가져올 수 없고, 저장된 데이터도 없습니다.");
        }
        yield break;
    }
}


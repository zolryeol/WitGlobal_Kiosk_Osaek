using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WeatherInfoFetcher : MonoBehaviour
{
    [SerializeField] 
    private RegionData regionData;
    [SerializeField]
    private TextMeshProUGUI degreeInfo;
    [SerializeField]
    private TextMeshProUGUI weatherText;

    // �λ絿�� ����
    [Header("InsaWeatherDetail")]
    [SerializeField]
    private TextMeshProUGUI currentDegree;
    [SerializeField]
    private TextMeshProUGUI humidity;
    [SerializeField]
    private TextMeshProUGUI windSpeed;

    // �λ絿�� ������
    [Header("NoInsaWeatherDetail")]
    [SerializeField]
    private TextMeshProUGUI regionName;

    // ������ ����
    [Header("WeatherIcon")]
    [SerializeField]
    private Image iconSprite;
    // ������ ����
    [SerializeField]
    private WeatherIconInfo weatherIconInfo;

    private HttpClient client;
    private string     apiKey;
    private string languageStr;
    bool isInsa;


    private void Awake()
    {
        isInsa = false;
        client = new HttpClient();
        apiKey = "ee3ed66c5f7e4d306ec2c54ab62c3c9b";
    }

    private void Start()
    {
        getWeatherData();

        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    private void getWeatherData()
    {
        // url����(�λ絿�� ��� ���� �� �浵�� �˻�)
        string url = "";

        string currentLanguage = LanguageService.getCurrentLanguage();

        switch (currentLanguage)
        {
            case "KR":
                languageStr = "kr";
                break;
            case "EN":
                languageStr = "en";
                break;
            case "JP":
                languageStr = "ja";
                break;
            case "CN":
                languageStr = "zh_cn";
                break;
            default:
                languageStr = "kr";
                break;
        }
        
        if (regionData.isInsa)
        {
            //url = $"https://api.openweathermap.org/data/2.5/forecast?lat={regionData.lat}&lon={regionData.lon}&lang={languageStr}&cnt=8&appid={apiKey}&units=metric";            
            url = $"http://134.185.113.244/forecast2?id=1835848&lang=kr&cnt=8";
        }
        else
        {
            url = $"https://api.openweathermap.org/data/2.5/forecast?id={regionData.regionId}&lang={languageStr}&cnt=8&appid={apiKey}&units=metric";            
        }

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = request.GetResponse() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();

            // JSON �����͸� WeatherData ��ü�� ������ȭ
            var weatherData = JsonConvert.DeserializeObject<WeatherData>(results);

            // ���� �ð����κ��� ���� ������ �����͸� 1�� ������
            var currentData = getCurrentWeatherData(weatherData.list);

            // ���������� ȭ�鿡 �ѷ���
            updateWeatherInfo(currentData, isInsa);
        }
    }

    // ���� �ð��� ���н��ð����� ��ȯ
    private int getUnixTime() {

        var now = DateTime.Now.ToLocalTime();
        var span = (now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
        int timestamp = (int)span.TotalSeconds;

        return timestamp;
    }

    // ���� �ֱ��� �������� �� ����, �ְ����� �������� ���� �޼���
    private WeatherItem getCurrentWeatherData(List<WeatherItem> weatherDataList)
    {
        // ���� �ֱ��� ���� ������ �������� ���� ������
        WeatherItem currentData = null;
        int         currentDiff = int.MaxValue;
        int         currentTime = getUnixTime();

        // ����, �ְ����� ����ϱ� ���� ����
        int minTemp = int.MaxValue;
        int maxTemp = int.MinValue;

        foreach (var item in weatherDataList)
        {
            // �ð����
            if (Mathf.Abs(currentTime - item.dt) < currentDiff) 
            {
                currentDiff = (int)Mathf.Abs(currentTime - item.dt);
                currentData = item;
            }

            // �µ����
            int currentMinTemp = (int)Math.Round(item.main.temp_min);
            int currentMaxTemp = (int)Math.Round(item.main.temp_max);
            if (minTemp > currentMinTemp) minTemp = currentMinTemp;
            if (maxTemp < currentMaxTemp) maxTemp = currentMaxTemp;
        }

        // �µ�����
        currentData.main.temp_min = minTemp;
        currentData.main.temp_max = maxTemp;

        return currentData;
    }

    // ���������� ȭ�鿡 �ѷ��ִ� �޼���
    private void updateWeatherInfo(WeatherItem weatherItem, bool isInsadong)
    {
        // ���� �̸� ����
        weatherText.text = weatherItem.weather[0].description;
       
        // ����, �ְ�µ� ����
        double lowDegreeVal = Math.Round(weatherItem.main.temp_min);
        double highDegreeVal = Math.Round(weatherItem.main.temp_max);

        // ������ ����
        Sprite icon = weatherIconInfo.GetIcon(weatherItem.weather[0].icon);
        iconSprite.sprite = icon;
        
        // ���� �λ絿�̶��
        if (regionData.isInsa) 
        {
            // 상대습도 설정
            string relativeHumidity = "";
            switch(languageStr)
            {
                case "kr":
                    relativeHumidity = "상대습도";
                    break;
                case "en":
                    relativeHumidity = "RelativeHumidity";
                    break;
                case "ja":
                    relativeHumidity = "相対湿度";
                    break;
                case "zh_cn":
                    relativeHumidity = "相对湿度";
                    break;
                default:
                    relativeHumidity = "상대습도";
                    break;
            }

            //Debug.LogError($"weatherItem.weather[0].icon : {weatherItem.weather[0].icon}");

            if (weatherItem.weather[0].icon == "09d" ||
                weatherItem.weather[0].icon == "09n" ||
                weatherItem.weather[0].icon == "10d" ||
                weatherItem.weather[0].icon == "10n" ||
                weatherItem.weather[0].icon == "11d" ||
                weatherItem.weather[0].icon == "11n" ||
                weatherItem.weather[0].icon == "13d" || 
                weatherItem.weather[0].icon == "13n")
            {
                new VideoController().OnChangeVideo("2", "weather");
            }
            else if (weatherItem.weather[0].icon == "01d" ||
                weatherItem.weather[0].icon == "01n" || 
                weatherItem.weather[0].icon == "02d" ||
                weatherItem.weather[0].icon == "02n")
            {
                new VideoController().OnChangeVideo("1", "weather");
            }
            else if (weatherItem.weather[0].icon == "03d" || 
                weatherItem.weather[0].icon == "03n" || 
                weatherItem.weather[0].icon == "04d" || 
                weatherItem.weather[0].icon == "04n" || 
                weatherItem.weather[0].icon == "50d" || 
                weatherItem.weather[0].icon == "50n")
            {
                new VideoController().OnChangeVideo("3", "weather");
            }

            // ����µ� ����
            double currentDegreeVal = Math.Round(weatherItem.main.temp);
            currentDegree.text = $"{currentDegreeVal}°";

            // ü���µ� ����
            double windChillDegreeVal = Math.Round(weatherItem.main.feels_like);
            degreeInfo.text = $"{lowDegreeVal}° / {highDegreeVal}° {relativeHumidity}{windChillDegreeVal}%";
          
            // ���� ����
            double humidityVal = weatherItem.main.humidity;
            humidity.text = $"{humidityVal}%";
           
            // ǳ�� ����
            double windSpeedVal = Math.Round(weatherItem.wind.speed, 1);
            windSpeed.text = $"{windSpeedVal}m/s";
        } 
        else 
        {
            string cityTranslation = "";
            string city = regionData.regionName;
            // 서울 부산 대전 광주 울산 대구
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    cityTranslation = city; // 한국어
                    break;
                case "EN":
                    switch (city)
                    {
                        case "서울":
                            cityTranslation = "Seoul";
                            break;
                        case "부산":
                            cityTranslation = "Busan";
                            break;
                        case "대전":
                            cityTranslation = "Daejeon";
                            break;
                        case "광주":
                            cityTranslation = "Gwangju";
                            break;
                        case "인천":
                            cityTranslation = "Incheon";
                            break;
                        case "울산":
                            cityTranslation = "Ulsan";
                            break;
                    }
                    break;
                case "JP":
                    switch (city)
                    {
                        case "서울":
                            cityTranslation = "ソウル";
                            break;
                        case "부산":
                            cityTranslation = "釜山";
                            break;
                        case "대전":
                            cityTranslation = "大田";
                            break;
                        case "광주":
                            cityTranslation = "光州";
                            break;
                        case "인천":
                            cityTranslation = "仁川";
                            break;
                        case "울산":
                            cityTranslation = "蔚山";
                            break;
                    }
                    break;
                case "CN":
                    switch (city)
                    {
                        case "서울":
                            cityTranslation = "首尔";
                            break;
                        case "부산":
                            cityTranslation = "釜山";
                            break;
                        case "대전":
                            cityTranslation = "大田";
                            break;
                        case "광주":
                            cityTranslation = "光州";
                            break;
                        case "인천":
                            cityTranslation = "仁川";
                            break;
                        case "울산":
                            cityTranslation = "蔚山";
                            break;
                    }
                    break;
            }

            regionName.text = cityTranslation;

            degreeInfo.text = $"{lowDegreeVal}° / {highDegreeVal}°";
        }
    }
}

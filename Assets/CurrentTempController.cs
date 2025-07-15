using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class CurrentTempController : MonoBehaviour
{
    // 현재 온도를 출력할 TextMeshProUGUI 컴포넌트
    [SerializeField]
    private TextMeshProUGUI currentTempText;
    // 최저 최고기온 및 체감온도 출력
    [SerializeField]
    private TextMeshProUGUI currentTempDetailText;

    private string languageStr;

    private void Start()
    {
        // 날씨 정보를 가져오는 코루틴 시작
        //StartCoroutine(GetWeatherData());
    }

    private IEnumerator GetWeatherData()
    {
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

        // API 호출을 위한 URL 구성 (5일간의 날씨 예보 데이터)
        //string url = $"https://api.openweathermap.org/data/2.5/forecast?id=1835848&lang={languageStr}&appid={apiKey}&units=metric";
        string url = $"http://134.185.113.244/forecast?id=1835848&lang={languageStr}&location=insa";
        // 1835848은 서울의 도시 ID입니다.

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        // 에러 체크
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("날씨 데이터를 가져오는 중 오류 발생: " + request.error);
            Debug.LogError("request.result: " + request.result);
            Debug.LogError($"HTTP Status Code: {request.responseCode}");
        }
        else
        {
            //Debug.LogError("날씨 데이터 파싱!!!!!");
            // 응답 데이터 수신
            string json = request.downloadHandler.text;

            // JSON 파싱
            var weatherData = JsonConvert.DeserializeObject<WeatherData>(json);

            // 현재 시간과 가장 가까운 WeatherItem 가져오기
            WeatherItem currentData = GetCurrentWeatherData(weatherData.list);

            // 전체 데이터에서 최저 및 최고 온도 계산
            double minTemp = double.MaxValue;
            double maxTemp = double.MinValue;

            foreach (var item in weatherData.list)
            {
                if (item.main.temp_min < minTemp)
                    minTemp = item.main.temp_min;

                if (item.main.temp_max > maxTemp)
                    maxTemp = item.main.temp_max;
            }

            minTemp = Math.Round(minTemp);
            maxTemp = Math.Round(maxTemp);

            //Debug.LogError($"최저 : {minTemp}, 최고 : {maxTemp}");

            // 현재 온도 및 체감 온도
            double currentTemp = Math.Round(currentData.main.temp);
            double feelsLikeTemp = Math.Round(currentData.main.feels_like);

            //Debug.LogError($"currentTemp : {currentTemp}, feelsLikeTemp : {feelsLikeTemp}");

            // UI 업데이트
            currentTempText.text = $"{currentTemp}˚"; 
            currentTempDetailText.text = $"{minTemp}˚/ {maxTemp}˚\n" + @"                    " + $"{feelsLikeTemp}˚";
        }
    }

    // 현재 시간과 가장 가까운 WeatherItem을 찾는 메소드
    private WeatherItem GetCurrentWeatherData(List<WeatherItem> weatherDataList)
    {
        WeatherItem currentData = null;
        double currentDiff = double.MaxValue;
        DateTime currentTime = DateTime.Now;

        foreach (var item in weatherDataList)
        {
            DateTime itemTime = DateTime.Parse(item.dt_txt);

            double diff = Math.Abs((currentTime - itemTime).TotalSeconds);
            if (diff < currentDiff)
            {
                currentDiff = diff;
                currentData = item;
            }
        }

        return currentData;
    }
}

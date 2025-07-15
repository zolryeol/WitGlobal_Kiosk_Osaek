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

public class DustInfoFetcher : MonoBehaviour
{
    [SerializeField]
    private RegionData regionData;

    [SerializeField]
    private TextMeshProUGUI particulate;      // 미세먼지
    [SerializeField]
    private TextMeshProUGUI fineParticulate;  // 초미세먼지

    private HttpClient client;
    private string apiKey;

    private void Awake()
    {
        client = new HttpClient();
        apiKey = "ee3ed66c5f7e4d306ec2c54ab62c3c9b";
    }

    private void Start()
    {
        getDustData();
    }

    private void getDustData()
    {
        //string url = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={regionData.lat}&lon={regionData.lon}&lang=kr&appid={apiKey}&units=metric";
        string url = $"http://134.185.113.244/air_pollution?lat={regionData.lat}&lon={regionData.lon}";

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = request.GetResponse() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();

            // JSON 데이터를 dustData 객체로 역직렬화
            var dustData = JsonConvert.DeserializeObject<DustData>(results);

            // 미세먼지정보를 화면에 뿌려줌
            updateDustInfo(dustData);
        }
    }

    // 미세먼지정보를 화면에 뿌려주는 메서드
    private void updateDustInfo(DustData dustData)
    {
        /*미세먼지 기준
            0~30 좋음
            31~80 보통
            81~150 나쁨
            151이상 매우나쁨*/

        // 미세먼지 갱신
        double particulateVal = dustData.list[0].components.pm10;
        string particulateStr = "";
        if (particulateVal < 31)
        {
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    particulateStr = "좋음";
                    break;
                case "EN":
                    particulateStr = "Good";
                    break;
                case "JP":
                    particulateStr = "良好";
                    break;
                case "CN":
                    particulateStr = "良好";
                    break;
            }

        }
        else if (particulateVal < 81)
        {

            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    particulateStr = "보통";
                    break;
                case "EN":
                    particulateStr = "Moderate";
                    break;
                case "JP":
                    particulateStr = "普通";
                    break;
                case "CN":
                    particulateStr = "一般";
                    break;
            }
        }
        else if (particulateVal < 151)
        {
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    particulateStr = "나쁨";
                    break;
                case "EN":
                    particulateStr = "Unhealthy";
                    break;
                case "JP":
                    particulateStr = "悪い";
                    break;
                case "CN":
                    particulateStr = "不良";
                    break;
            }
        }
        else
        {
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    particulateStr = "매우나쁨";
                    break;
                case "EN":
                    particulateStr = "Very Unhealthy";
                    break;
                case "JP":
                    particulateStr = "非常に悪い";
                    break;
                case "CN":
                    particulateStr = "非常差";
                    break;
            }
        }
        particulate.text = $"{particulateStr}";


        // 초미세먼지 갱신
        double fineParticulateVal = dustData.list[0].components.pm2_5;
        string fineParticulateStr = "";
        if (fineParticulateVal < 16)
        {
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    fineParticulateStr = "좋음";
                    break;
                case "EN":
                    fineParticulateStr = "Good";
                    break;
                case "JP":
                    fineParticulateStr = "良好";
                    break;
                case "CN":
                    fineParticulateStr = "良好";
                    break;
            }
        }
        else if (fineParticulateVal < 36)
        {
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    fineParticulateStr = "보통";
                    break;
                case "EN":
                    fineParticulateStr = "Moderate";
                    break;
                case "JP":
                    fineParticulateStr = "普通";
                    break;
                case "CN":
                    fineParticulateStr = "一般";
                    break;
            }

        }
        else if (fineParticulateVal < 76)
        {
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    fineParticulateStr = "나쁨";
                    break;
                case "EN":
                    fineParticulateStr = "Unhealthy";
                    break;
                case "JP":
                    fineParticulateStr = "悪い";
                    break;
                case "CN":
                    fineParticulateStr = "不良";
                    break;
            }

        }
        else
        {
            switch (LanguageService.getCurrentLanguage())
            {
                case "KR":
                    fineParticulateStr = "매우나쁨";
                    break;
                case "EN":
                    fineParticulateStr = "Very Unhealthy";
                    break;
                case "JP":
                    fineParticulateStr = "非常に悪い";
                    break;
                case "CN":
                    fineParticulateStr = "非常差";
                    break;
            }
        }

        fineParticulate.text = $"{fineParticulateStr}";
    }
}




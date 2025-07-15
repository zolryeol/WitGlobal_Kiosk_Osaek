using System.IO;
using System.Net;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class WeaTherAPI : MonoBehaviour
{


    private HttpClient client;
    private MySqlConnection connection;

    [SerializeField] private TextMeshProUGUI curTemperature;  // 현재 온도
    [SerializeField] private TextMeshProUGUI maxlowTemperature;  // 최고/최저 온도

    [SerializeField] private TextMeshProUGUI mondayTemperature;     // 월요일 최고/최저 온도
    [SerializeField] private TextMeshProUGUI wednsedayTemperature;  // 화요일 최고/최저 온도
    [SerializeField] private TextMeshProUGUI tueseTemperature;      // 수요일 최고/최저 온도
    [SerializeField] private TextMeshProUGUI thurseTemperature;     // 목요일 최고/최저 온도
    [SerializeField] private TextMeshProUGUI friTemperature;        // 금요일 최고/최저 온도
    int dayCount = 0;
    Image img;
    bool jobScheduledForToday = false;


    string server = "134.185.113.244";
    string database = "kiosk";
    string user = "witserver";
    //string password = "1234";
    string password = "Witglobal2030$$";
    string port = "3306";

    // Start is called before the first frame update
    void Start()
    {
        //weatherDBInsert();

        //GetTodayTemperature();

        // DB에서 이번주 데이터 가져오기        
        using (MySqlConnection connection = new MySqlConnection($"Server={server};Database={database};Uid={user};Pwd={password};Port={port};SslMode=none;Charset=utf8mb4;"))
        {

            // 숫자만 추출            
            string result = "";
            

            connection.Open();
            //Debug.Log("DB 열림");

            string selectQuery = @"SELECT * 
                                FROM week_weather
                                WHERE YEARWEEK(DATE(weather_dt), 1) = YEARWEEK(CURDATE(), 1) ";
            // Week_WeatherVO 객체를 생성하여 데이터 매핑
            
            using (var selectCmd = new MySqlCommand(selectQuery, connection))
            {
                using (MySqlDataReader sqlReader = selectCmd.ExecuteReader())
                {
                    string[] weatherImgs = { "img1", "img2", "img3", "img4", "img5" };
                    int i = 0;                    
                    while (sqlReader.Read() && i < weatherImgs.Length)
                    {
                        var targetObject = GameObject.Find(weatherImgs[i]);
                        if (targetObject == null)
                        {
                            Debug.LogError($"GameObject {weatherImgs[i]}를 찾을 수 없습니다.");
                            continue; // 다음 반복으로 이동
                        }

                        var weatherImg = sqlReader["weather_img"].ToString();
                        if (string.IsNullOrEmpty(weatherImg) || weatherImg.Length < 2)
                        {
                            Debug.LogError($"weather_img 값이 유효하지 않습니다: {weatherImg}");
                            continue; // 다음 반복으로 이동
                        }

                        targetObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Image/Weather/WeatherIcon/" + weatherImg.Substring(0, 2));
                        i++;

                        Debug.Log(Resources.Load<Sprite>("Image/Weather/WeatherIcon/" + sqlReader["weather_img"].ToString().Substring(0, 2)));
                        DateTime weatherDate = DateTime.Parse(sqlReader["weather_dt"].ToString());  // 문자열을 DateTime으로 변환

                        var matches = Regex.Matches(sqlReader["WEATHER_TEMPERATURE"].ToString(), @"\d+(\.\d+)?");
                        result = "";
                        foreach (Match match in matches)
                        {
                            // 소수점 이하 제거
                            double number = double.Parse(match.Value);
                            int truncatedNumber = (int)number;

                            result += truncatedNumber + "˚/";
                        }

                        // 마지막 "/" 제거
                        result = result.TrimEnd('/');

                        switch (weatherDate.DayOfWeek)  // DayOfWeek를 기준으로 요일을 나눔
                        {
                            case DayOfWeek.Monday:
                                // 월요일 로직
                                //Debug.Log("월요일?" + mondayTemperature);                                
                                mondayTemperature.text = result;
                                break;

                            case DayOfWeek.Tuesday:
                                //Debug.Log("화요일?");
                                // 화요일 로직
                                tueseTemperature.text = result;
                                break;

                            case DayOfWeek.Wednesday:
                                //Debug.Log("수요일?");
                                // 수요일 로직
                                wednsedayTemperature.text = result;
                                break;

                            case DayOfWeek.Thursday:
                                //Debug.Log("목요일?");
                                // 목요일 로직
                                thurseTemperature.text = result;
                                break;

                            case DayOfWeek.Friday:
                                //Debug.Log("금요일?");
                                // 금요일 로직
                                friTemperature.text = result;
                                break;

                            default:
                                // 월~금요일이 아닌 경우 (토요일, 일요일 등)
                                //Console.WriteLine("주말 또는 다른 요일");
                                break;
                        }
                    }
                }
            }
        }              

        // 메인 날짜 번역
        string currentLanguage = LanguageService.getCurrentLanguage();
        LanguageService ls = new LanguageService();
        ls.weatherSceneInit();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("weather");

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
                    Debug.Log(res[key][currentLanguage] + "어디감");
                    //TMP_FontAsset font = LanguageService.getFontAsset(currentLanguage);
                    //textComponent.font = font;                    
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

    private void getWeekData()
    {
        string url = "http://134.185.113.244/weather/weekData";
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = request.GetResponse() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();



            // JSON 데이터를 WeatherData 객체로 역직렬화
            var weatherData = JsonConvert.DeserializeObject<WeatherData>(results);
        }
    }

    private void weatherDBInsert()
    {


        //string url = "https://api.openweathermap.org/data/2.5/forecast?id=1833742&lang=kr&appid=ee3ed66c5f7e4d306ec2c54ab62c3c9b&units=metric";
        string url = "http://134.185.113.244/forecast?id=1833742&lang=kr";
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = request.GetResponse() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();

            

            // JSON 데이터를 WeatherData 객체로 역직렬화
            var weatherData = JsonConvert.DeserializeObject<WeatherData>(results);
            
            // 현재 날짜                                               
            DateTime today = DateTime.Now.Date;

            using (MySqlConnection connection = new MySqlConnection($"Server={server};Database={database};Uid={user};Pwd={password};Port={port};SslMode=none;Charset=utf8mb4;"))
            {
                
                connection.Open();
                // Debug.Log("DB 열림");

                string selectQuery = @"SELECT * 
                       FROM week_weather 
                       WHERE DATE(WEATHER_DT) = DATE_SUB(CURDATE(), INTERVAL WEEKDAY(CURDATE()) DAY)";
                // Week_WeatherVO 객체를 생성하여 데이터 매핑
                Week_WeatherVO data = new Week_WeatherVO();
                using (var selectCmd = new MySqlCommand(selectQuery, connection))
                {
                    using (MySqlDataReader sqlReader = selectCmd.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {                                                    
                            data.weather_cd = int.Parse(sqlReader["weather_cd"].ToString());
                            data.weather_dt = sqlReader["weather_dt"].ToString();                            
                        }

                        // 이번 주 월요일인지 확인하는 로직
                        DateTime t = DateTime.Today;
                        DateTime thisWeekMonday = t.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

                        // 널이면 저장안함
                        if (data.weather_dt != null) { Debug.Log("월요일 데이터가 있데요"); return; }
                    }
                }

                DateTime startDate = DateTime.Now.Date;  // 당일 날짜
                DateTime currentDate = DateTime.MinValue;  // 현재 처리 중인 날짜
                double temp_min = double.MaxValue;
                double temp_max = double.MinValue;
                bool isFirstEntryOfDay = true;

                foreach (var item in weatherData.list)  // weatherDataList는 받아온 데이터 리스트
                {
                    // dt_txt를 DateTime으로 변환
                    DateTime dateTime = DateTime.Parse(item.dt_txt);

                    // 만약 데이터가 당일 이전이면 스킵
                    if (dateTime.Date < startDate)
                    {
                        continue;  // 이전 날짜의 데이터는 처리하지 않음
                    }

                    // 새로운 날짜가 나오면 현재 날짜의 최고/최저를 DB에 저장하고, 새로운 날짜로 전환
                    if (dateTime.Date != currentDate.Date)
                    {
                        if (!isFirstEntryOfDay)
                        {
                            // 이전 날짜의 최고/최저를 DB에 저장
                            //string insertQuery = "INSERT INTO WEEK_WEATHER VALUES(DEFAULT, @description, @icon, @date, @temp, DEFAULT, @location)";
                            string insertQuery = "INSERT INTO WEEK_WEATHER VALUES(DEFAULT, @description, @icon, @date, @temp, DEFAULT, @location)";
                            using (var insertCmd = new MySqlCommand(insertQuery, connection))
                            {
                                Debug.Log("날짜: " + currentDate.ToString("yyyy-MM-dd") + " - 최고: " + temp_max + ", 최저: " + temp_min);
                                insertCmd.Parameters.AddWithValue("@description", item.weather[0].description);
                                insertCmd.Parameters.AddWithValue("@icon", item.weather[0].icon);
                                insertCmd.Parameters.AddWithValue("@date", currentDate.ToString("yyyy-MM-dd"));
                                insertCmd.Parameters.AddWithValue("@temp", temp_min + "˚/" + temp_max + "˚");
                                insertCmd.Parameters.AddWithValue("@location", "INSA");
                                insertCmd.ExecuteNonQuery();
                            }

                        }

                        // 새로운 날짜로 설정 및 초기화
                        currentDate = dateTime;
                        temp_min = double.MaxValue;
                        temp_max = double.MinValue;
                        isFirstEntryOfDay = false;
                    }

                    // 최저 온도 추적
                    if (item.main.temp_min < temp_min)
                    {
                        temp_min = item.main.temp_min;
                    }

                    // 최고 온도 추적
                    if (item.main.temp_max > temp_max)
                    {
                        temp_max = item.main.temp_max;
                    }
                }

                // 마지막 날짜의 최고/최저 온도 저장
                if (!isFirstEntryOfDay)
                {
                    //string insertQuery = "INSERT INTO WEEK_WEATHER VALUES(DEFAULT, @description, @icon, @date, @temp, DEFAULT, @location)";
                    string insertQuery = "INSERT INTO WEEK_WEATHER (description, icon, date, temp, location) VALUES (@description, @icon, @date, @temp, @location) ON DUPLICATE KEY UPDATE description = VALUES(description), icon = VALUES(icon), temp = VALUES(temp)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection))
                    {
                        Debug.Log("날짜: " + currentDate.ToString("yyyy-MM-dd") + " - 최고: " + temp_max + ", 최저: " + temp_min);
                        insertCmd.Parameters.AddWithValue("@description", weatherData.list[weatherData.list.Count-1].weather[0].description);
                        insertCmd.Parameters.AddWithValue("@icon", weatherData.list[weatherData.list.Count - 1].weather[0].icon);
                        insertCmd.Parameters.AddWithValue("@date", currentDate.ToString("yyyy-MM-dd"));
                        insertCmd.Parameters.AddWithValue("@temp", temp_min + "˚/" + temp_max + "˚");
                        insertCmd.Parameters.AddWithValue("@location", "INSA");
                        insertCmd.ExecuteNonQuery();
                    }
                }

                connection.Clone();
                Debug.Log("DB 종료");
            }
        }
    }

    void GetTodayTemperature()
    {
        using (MySqlConnection connection = new MySqlConnection($"Server={server};Database={database};Uid={user};Pwd={password};Port={port};SslMode=none;Charset=utf8mb4;"))
        {
            connection.Open();

            // 오늘의 데이터를 가져오는 쿼리
            string todayQuery = @"SELECT WEATHER_TEMPERATURE 
                              FROM week_weather 
                              WHERE DATE(WEATHER_DT) = CURDATE()";

            using (var todayCmd = new MySqlCommand(todayQuery, connection))
            {
                using (var reader = todayCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string temperatureData = reader["WEATHER_TEMPERATURE"].ToString();

                        // 온도 데이터 분리
                        var matches = Regex.Matches(temperatureData, @"\d+");
                        if (matches.Count >= 2)
                        {
                            int minTemp = int.Parse(matches[0].Value); // 최저 온도
                            int maxTemp = int.Parse(matches[1].Value); // 최고 온도

                            // 결과 출력 및 UI 업데이트
                            Debug.Log($"오늘 최저 온도: {minTemp}˚, 최고 온도: {maxTemp}˚");
                            curTemperature.text = $"{minTemp}˚/{maxTemp}˚"; // UI 텍스트 업데이트
                        }
                    }
                    else
                    {
                        Debug.LogWarning("오늘의 날씨 데이터를 찾을 수 없습니다.");
                    }
                }
            }
        }
    }

    private void Update()
    {
        //DateTime now = DateTime.Now;
        //// 현재 시간이 월요일 오전 9시인지 확인
        //if (now.DayOfWeek == DayOfWeek.Monday && now.Hour == 06 && now.Minute == 00 && !jobScheduledForToday)            
        ////if(!jobScheduledForToday)
        //{
        //    jobScheduledForToday = true;  // 오늘 작업이 이미 실행됨을 표시
        //    weatherDBInsert();            
        //}

        ////새로운 날이 되면 다시 작업을 스케줄링할 수 있도록 초기화
        //if (now.DayOfWeek != DayOfWeek.Monday)
        //{
        //    // Debug.Log("들어옴??");
        //    jobScheduledForToday = false;
        //}

    }


    public Image[] weatherImages; // img1 ~ img5에 해당하는 이미지 컴포넌트를 배열로 저장
    [SerializeField] private Sprite sunnySprite;    // 맑음
    [SerializeField] private Sprite cloudySprite;   // 구름
    [SerializeField] private Sprite rainSprite;     // 비
    [SerializeField] private Sprite snowSprite;     // 눈
    [SerializeField] private Sprite thunderSprite;  // 천둥
    [SerializeField] private Sprite hailSprite;     // 우박

    // API로부터 받은 날씨 데이터를 처리하는 함수
    private void SetWeatherIcons(string description, Image img)
    {        

        if (description.Contains("맑음"))
        {
            img.sprite = sunnySprite;
        }
        else if (description.Contains("구름"))
        {
            img.sprite = cloudySprite;
        }
        else if (description.Contains("비"))
        {
            img.sprite = rainSprite;
        }
        else if (description.Contains("눈"))
        {
            img.sprite = snowSprite;
        }
        else if (description.Contains("천둥"))
        {
            img.sprite = thunderSprite;
        }
        else if (description.Contains("우박"))
        {
            img.sprite = hailSprite;
        }
        else
        {
            Debug.LogWarning($"알 수 없는 날씨 설명: {description}");
        }


        dayCount++;
    }

}

internal class Week_WeatherVO
{
    public int weather_cd { get; set; }
    public string weather_description { get; set; }
    public string weather_img { get; set; }
    public string weather_dt { get; set; }
    public string weather_temperature { get; set; }
    public string weather_reg_dt { get; set; }
    
}
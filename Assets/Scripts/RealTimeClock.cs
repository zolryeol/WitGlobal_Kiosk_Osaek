using UnityEngine;
using TMPro;
using System;

public class RealTimeClock : MonoBehaviour
{
    private TextMeshProUGUI timeText;  // 현재 시간을 표시할 TextMeshProUGUI 컴포넌트

    private void Awake()
    {
        timeText = GetComponent<TextMeshProUGUI>();
        DateTime currentDateTime = DateTime.Now;

        // 현재 언어를 불러온다.
        string currentLanguage = LanguageService.getCurrentLanguage();

        // 요일을 언어에 맞게 변환한다.
        string dayOfWeek = GetDayOfWeekInLanguage(currentDateTime.DayOfWeek, "EN");
        string formattedDate = currentDateTime.ToString("yyyy-MM-dd") + $"({dayOfWeek})";
        timeText.text = formattedDate;
    }

    // 언어에 맞춰 요일을 반환하는 메서드
    string GetDayOfWeekInLanguage(DayOfWeek dayOfWeek, string language)
    {
        string[] daysKR = { "일", "월", "화", "수", "목", "금", "토" };
        string[] daysEN = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        string[] daysJP = { "日", "月", "火", "水", "木", "金", "土" };
        string[] daysCN = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };

        switch (language)
        {
            case "KR":
                return daysKR[(int)dayOfWeek];
            case "EN":
                return daysEN[(int)dayOfWeek];
            case "JP":
                return daysJP[(int)dayOfWeek];
            case "CN":
                return daysCN[(int)dayOfWeek];
            default:
                return daysEN[(int)dayOfWeek];  // 기본값은 영어
        }
    }
}

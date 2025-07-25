using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayDate : MonoBehaviour
{
    TextMeshProUGUI dateText;
    string[] daysEN = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
    private void Awake()
    {
        dateText = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        DisPlayDate();
    }

    public void DisPlayDate()
    {
        DateTime currentDateTime = DateTime.Now;

        var dow = daysEN[(int)currentDateTime.DayOfWeek];

        string formattedDate = currentDateTime.ToString("yyyy-MM-dd") + $"({dow})";
        dateText.text = formattedDate;
    }
}

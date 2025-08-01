using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherButton : MonoBehaviour
{
    Button button;
    WeatherAPI_JH weatherAPI;

    private void Awake()
    {
        button = GetComponent<Button>();
        weatherAPI = FindObjectOfType<WeatherAPI_JH>();
    }

    public void Init()
    {
        button.onClick.AddListener(() => Debug.Log("날씨버튼 클릭"));
        button.onClick.AddListener(ChangeWeatherVideo);
    }

    void ChangeWeatherVideo()
    {
        VideoPlayManager.Instance.PlayVideo(weatherAPI.WeatherVideoType);
    }
}

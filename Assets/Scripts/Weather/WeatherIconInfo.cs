using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherIconInfo : MonoBehaviour
{
    [SerializeField]
    private Sprite clearSky;
    [SerializeField]
    private Sprite fewClouds;
    [SerializeField]
    private Sprite scatteredClouds;
    [SerializeField]
    private Sprite brokenClouds;
    [SerializeField]
    private Sprite showerRain;
    [SerializeField]
    private Sprite rain;
    [SerializeField]
    private Sprite thunderstorm;
    [SerializeField]
    private Sprite snow;
    [SerializeField]
    private Sprite mist;

    public Sprite GetIcon(string iconCode)
    {
        switch (iconCode)
        {
            case "01d":
            case "01n":
                return clearSky;
            case "02d":
            case "02n":
                return fewClouds;
            case "03d":
            case "03n":
                return scatteredClouds;
            case "04d":
            case "04n":
                return brokenClouds;
            case "09d":
            case "09n":
                return showerRain;
            case "10d":
            case "10n":
                return rain;
            case "11d":
            case "11n":
                return thunderstorm;
            case "13d":
            case "13n":
                return snow;
            case "50d":
            case "50n":
                return mist;
            default:
                return clearSky;
        }
    }
}

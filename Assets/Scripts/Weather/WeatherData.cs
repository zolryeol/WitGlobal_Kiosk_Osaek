using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherData
{
    public string cod { get; set; }
    public int message { get; set; }
    public int cnt { get; set; }
    public List<WeatherItem> list { get; set; }
}

public class WeatherItem
{
    public long dt { get; set; }
    public WeatherMain main { get; set; }
    public List<Weather> weather { get; set; }
    public Clouds clouds { get; set; }
    public Wind wind { get; set; }
    public int visibility { get; set; }
    public double pop { get; set; }
    public Sys sys { get; set; }
    public string dt_txt { get; set; }
}

public class WeatherMain
{
    public double temp { get; set; }
    public double feels_like { get; set; }
    public double temp_min { get; set; }
    public double temp_max { get; set; }
    public int pressure { get; set; }
    public int sea_level { get; set; }
    public int grnd_level { get; set; }
    public int humidity { get; set; }
    public double temp_kf { get; set; }
}

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}

public class Clouds
{
    public int all { get; set; }
}

public class Wind
{
    public double speed { get; set; }
    public int deg { get; set; }
    public double gust { get; set; }
}

public class Sys
{
    public string pod { get; set; }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustData
{
    public DustCoord coord { get; set; }
    public List<DustItem> list { get; set; }
}

public class DustCoord 
{
    public string lon { get; set; }
    public string lat { get; set; }
}

public class DustItem
{
    public DustMain main { get; set; }
    public DustComponents components { get; set; }
    public long dt;
}

public class DustMain
{   
    public int aqi { get; set; }
}

public class DustComponents
{
    public double co { get; set; }
    public double no { get; set; }
    public double no2 { get; set; }
    public double o3 { get; set; }
    public double so2 { get; set; }
    public double pm2_5 { get; set; }
    public double pm10 { get; set; }
    public double nh3 { get; set; }
}
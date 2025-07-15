using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIListData
{
    public int count { get; set; }
    public int err_code { get; set; }
    public List<AIResponseList> response { get; set; }
}

public class AIDetailData
{
    public int err_code { get; set; }
    public AIResponseDetail response { get; set; }
}

public class AIResponseList
{
    public string address { get; set; }
    public string ca2 { get; set; }
    public string img_url_1 { get; set; }
    public string img_url_2 { get; set; }
    public string img_url_3 { get; set; }
    public string img_url_4 { get; set; }
    public string img_url_5 { get; set; }
    public string img_url_6 { get; set; }
    public string intro { get; set; }
    public string keyword { get; set; }
    public string link { get; set; }
    public string main { get; set; }
    public string name { get; set; }
    public string naver_score { get; set; }
    public string store_no { get; set; }
    public string store { get; set; }
    public string tel_no { get; set; }
    public string time { get; set; }
}

public class AIResponseDetail
{
    public string address { get; set; }
    public string address_k { get; set; }
    public string ca2 { get; set; }
    public string img_url_1 { get; set; }
    public string img_url_2 { get; set; }
    public string img_url_3 { get; set; }
    public string img_url_4 { get; set; }
    public string img_url_5 { get; set; }
    public string img_url_6 { get; set; }
    public string intro { get; set; }
    public string keyword { get; set; }
    public string link { get; set; }
    public string main { get; set; }
    public string name { get; set; }
    public string naver_score { get; set; }
    public string store_no { get; set; }
    public string tel_no { get; set; }
    public string time { get; set; }
}
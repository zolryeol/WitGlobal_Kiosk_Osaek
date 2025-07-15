using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchListData
{
    public int count { get; set; }
    public int err_code { get; set; }
    public List<SearchResponseList> response { get; set; }
}

public class SearchDetailData
{
    public int err_code { get; set; }
    public ResponseDetail response { get; set; }
}

public class SearchResponseList
{
    public string address { get; set; }
    public string category { get; set; }
    public string crt_date { get; set; }
    public string distance { get; set; }
    public string img_url_1 { get; set; }
    public string img_url_2 { get; set; }
    public string img_url_3 { get; set; }
    public string img_url_4 { get; set; }
    public string img_url_5 { get; set; }
    public string img_url_6 { get; set; }
    public string intro { get; set; }
    public string keyword { get; set; }
    public string link { get; set; }
    public string naver_score { get; set; }
    public string no { get; set; }
    public string store { get; set; }
    public string tel_no { get; set; }
    public string time { get; set; }
}
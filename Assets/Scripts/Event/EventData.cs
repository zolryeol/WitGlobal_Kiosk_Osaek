using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListData
{
    public int count { get; set; }
    public int err_code { get; set; }
    public List<ResponseEventList> response { get; set; }
}

public class EventDetailData
{
    public int err_code { get; set; }
    public ResponseEventDetail response { get; set; }
}

public class ResponseEventList
{
    public string event_id { get; set; }
    public string event_address { get; set; }
    public string event_age { get; set; }
    public string event_company_nm { get; set; }
    public string event_date { get; set; }
    public string event_description { get; set; }
    public string event_fee { get; set; }
    public string event_hashtag { get; set; }
    public string event_name { get; set; }
    public string event_telno { get; set; }
    public string image_path { get; set; }
    public string kiosk_location { get; set; }
    public string link { get; set; }    
}

public class ResponseEventDetail
{
    public string event_id { get; set; }
    public string event_address { get; set; }
    public string event_age { get; set; }
    public string event_company_nm { get; set; }
    public string event_date { get; set; }
    public string event_description { get; set; }
    public string event_fee { get; set; }
    public string event_hashtag { get; set; }
    public string event_name { get; set; }
    public string event_telno { get; set; }
    public string image_path { get; set; }
    public string kiosk_location { get; set; }
    public string link { get; set; }
}
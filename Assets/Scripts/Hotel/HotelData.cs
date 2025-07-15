using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelListData
{
    public int count { get; set; }
    public int err_code { get; set; }
    public List<ResponseList> response { get; set; }
}

public class HotelDetailData
{
    public int err_code { get; set; }
    public ResponseDetail response { get; set; }
}
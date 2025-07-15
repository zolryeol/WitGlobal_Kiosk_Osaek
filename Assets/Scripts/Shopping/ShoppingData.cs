using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingListData
{
    public int count { get; set; }
    public int err_code { get; set; }
    public List<ResponseList> response { get; set; }
}

public class ShoppingDetailData
{
    public int err_code { get; set; }
    public ResponseDetail response { get; set; }
}
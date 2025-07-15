using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RegionData", menuName = "Scriptable Object/RegionData", order = int.MinValue)]
public class RegionData : ScriptableObject
{
    public bool   isInsa;
    public string regionId;
    public string regionName;
    public string lat;
    public string lon;
}

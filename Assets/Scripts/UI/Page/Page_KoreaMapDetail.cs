using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page_KoreaMapDetail : MonoBehaviour
{
    [SerializeField] List<GameObject> koreaMap = new();
    Transform body;
    public void Init()
    {
        body = transform.Find("Body");
        koreaMap.Clear();
        foreach (Transform child in body)
        {
            koreaMap.Add(child.gameObject);
        }
    }

    public void OnMap(string targetMapStr)
    {
        koreaMap.ForEach(map => map.SetActive(false));

        Debug.Log($"TargetStr = {targetMapStr}");
        koreaMap.Find(t => t.name == targetMapStr)?.SetActive(true);
    }
}

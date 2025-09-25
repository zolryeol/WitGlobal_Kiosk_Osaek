using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Page_KoreaMapDetail : MonoBehaviour
{
    [SerializeField] List<GameObject> koreaMap = new(); // 도,시 단위
    Transform body;
    Transform SetLoaction;
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

        var target = koreaMap.Find(t => t.name == targetMapStr);
        if (target) target.SetActive(true);
    }

    // 설치된것 찾아서 색칠하기 기능필요?
}
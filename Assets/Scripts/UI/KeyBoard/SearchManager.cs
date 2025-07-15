using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchManager : MonoBehaviour
{
    public static SearchManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Search(string keyword)
    {
        Debug.Log($"[검색] '{keyword}' 검색 수행 중...");
        // 여기에 검색 결과 필터링/출력 로직 구현
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class CurrentSearchKeyword : MonoBehaviour
{
    // 검색어 들어올곳
    [SerializeField]
    private TextMeshProUGUI text;

    // 검색어 갱신
    private void Start()
    {
        string history1 = string.IsNullOrEmpty(PlayerPrefs.GetString("museumSearchHistory1")) ? "검색어" : PlayerPrefs.GetString("museumSearchHistory1");
        string history2 = string.IsNullOrEmpty(PlayerPrefs.GetString("museumSearchHistory2")) ? "검색어" : PlayerPrefs.GetString("museumSearchHistory2");
        string history3 = string.IsNullOrEmpty(PlayerPrefs.GetString("museumSearchHistory3")) ? "검색어" : PlayerPrefs.GetString("museumSearchHistory3");
        string history4 = string.IsNullOrEmpty(PlayerPrefs.GetString("museumSearchHistory4")) ? "검색어" : PlayerPrefs.GetString("museumSearchHistory4");
        string history5 = string.IsNullOrEmpty(PlayerPrefs.GetString("museumSearchHistory5")) ? "검색어" : PlayerPrefs.GetString("museumSearchHistory5");
        string history6 = string.IsNullOrEmpty(PlayerPrefs.GetString("museumSearchHistory6")) ? "검색어" : PlayerPrefs.GetString("museumSearchHistory6");

        text.text = $"<color=#FE6C50>•</color>  {history1}        <color=#FE6C50>•</color>  {history2}        <color=#FE6C50>•</color>  {history3}        <color=#FE6C50>•</color>  {history4}        <color=#FE6C50>•</color>  {history5}        <color=#FE6C50>•</color>  {history6}";
    }

    // 검색어 추가
    public static void AddMuseumSearchKeyword(string keyword)
    {
        // 검색어는 최대 6개까지
        PlayerPrefs.SetString("museumSearchHistory6", PlayerPrefs.GetString("museumSearchHistory5"));
        PlayerPrefs.SetString("museumSearchHistory5", PlayerPrefs.GetString("museumSearchHistory4"));
        PlayerPrefs.SetString("museumSearchHistory4", PlayerPrefs.GetString("museumSearchHistory3"));
        PlayerPrefs.SetString("museumSearchHistory3", PlayerPrefs.GetString("museumSearchHistory2"));
        PlayerPrefs.SetString("museumSearchHistory2", PlayerPrefs.GetString("museumSearchHistory1"));
        PlayerPrefs.SetString("museumSearchHistory1", keyword);
    }
}

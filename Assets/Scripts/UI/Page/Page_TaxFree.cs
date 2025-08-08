using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

/// <summary>
/// 텍스프리 페이지
/// </summary>
public class Page_TaxFree : MonoBehaviour
{
    [SerializeField] HomeButton homeButton;
    [SerializeField] BackButton backButton;

    [SerializeField] CanvasWebViewPrefab webViewPrefab;
    [SerializeField] Button webBackButton;

    [SerializeField] string url = "https://www.naver.com/"; // 텍스프리 URL

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        webBackButton.onClick.AddListener(() => webViewPrefab.WebView.GoBack());
        homeButton.onClick.AddListener(() => ReLoadWeb());
        backButton.onClick.AddListener(() => ReLoadWeb());
    }

    public void ReLoadWeb()
    {
        webViewPrefab.WebView.LoadUrl(url);
    }

}

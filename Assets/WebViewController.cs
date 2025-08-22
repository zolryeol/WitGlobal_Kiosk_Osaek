using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class WebViewController : MonoBehaviour
{
    [SerializeField] CanvasWebViewPrefab _canvasWebViewPrefab;

    private void Awake()
    {
        _canvasWebViewPrefab = FindAnyObjectByType<CanvasWebViewPrefab>();
    }
    public async void Back()
    {
        if (await _canvasWebViewPrefab.WebView.CanGoBack())
        {
            _canvasWebViewPrefab.WebView.GoBack();
        }
    }
}

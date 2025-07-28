using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HanbokContentButton : MonoBehaviour, ISelectableButton
{
    public bool IsSelected { get; private set; } = false;

    [SerializeField] string hanbokFileName = "";
    [SerializeField] Image hanbokSprite; // 반드시 Inspector에 할당하거나 자동 연결됨

    Button button;
    Image backGroundImage; // 버튼 배경

    [SerializeField] int tempIndex = 0;
    private void Awake()
    {
        button = GetComponent<Button>();
        backGroundImage = GetComponent<Image>();

        if (hanbokSprite == null)
        {
            hanbokSprite = GetComponentsInChildren<Image>(true).FirstOrDefault(img => img.gameObject != this.gameObject);
        }
    }

    private void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    public void FetchHanbokSprite(string _fileName, Sprite _sprite)
    {
        hanbokFileName = _fileName;
        hanbokSprite.sprite = _sprite;

        if (int.TryParse(_fileName, out int result))
        {
            tempIndex = result;
        }
    }

    public void SetElgamoHanbokIndex()
    {
        var elgato = FindAnyObjectByType<ElgatoController>();
        string numberPart = new string(hanbokFileName.Where(char.IsDigit).ToArray());
        int index = int.TryParse(numberPart, out int result) ? result : -1;

        Debug.Log($"한복인덱스 = {index}");

        elgato.hanbokIndex = index + 20;
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (IsSelected)
        {
            backGroundImage.sprite = ResourceManager.Instance.HanbokSelected_Background;
            SetElgamoHanbokIndex();
        }
        else
        {
            backGroundImage.sprite = ResourceManager.Instance.HanbokNormal_Background;
        }
    }

    public void OnButtonClicked()
    {
        UIManager.Instance.DeselectAllCustomButtons(UIManager.Instance.HanbokContentButtons);
        SetSelected(true);
    }

    /// <summary>
    /// 한복 이미지가 비율 유지되도록 초기 설정을 적용합니다.
    /// </summary>
    private void ConfigureHanbokImageTransform()
    {
        if (hanbokSprite == null) return;

        RectTransform rt = hanbokSprite.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(100, 100); // 초기값 (기준용)

        var fitter = hanbokSprite.GetComponent<AspectRatioFitter>();
        if (fitter == null)
        {
            fitter = hanbokSprite.gameObject.AddComponent<AspectRatioFitter>();
        }
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

        var layout = hanbokSprite.GetComponent<LayoutElement>();
        if (layout == null)
        {
            layout = hanbokSprite.gameObject.AddComponent<LayoutElement>();
        }
        layout.ignoreLayout = true;
    }

    internal void FetchHanbokSprite(int v, (int, Sprite) value)
    {
        throw new NotImplementedException();
    }
}
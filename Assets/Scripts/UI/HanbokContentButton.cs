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

    [SerializeField]
    string hanbokFileName = "";


    Button button;

    Image backGroundImage; // 사각 배경 클릭하면 이미지 교체
    Image hanbokSprite;

    private void Awake()
    {
        button = GetComponent<Button>();
        backGroundImage = GetComponent<Image>();

        hanbokSprite = transform.GetComponentsInChildren<Image>().FirstOrDefault(c => c.gameObject != this.gameObject);
    }

    private void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    public void FetchHanbokSprite(string _fileName, Sprite _sprite)
    {
        hanbokFileName = _fileName;
        hanbokSprite.sprite = _sprite;
    }

    public void SetElgamoHanbokIndex()
    {
        var elgato = FindAnyObjectByType<ElgatoController>();

        string numberPart = new string(hanbokFileName.Where(char.IsDigit).ToArray());

        int index = int.TryParse(numberPart, out int result) ? result : -1;

        elgato.hanbokIndex = index;
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (IsSelected == true)
        {
            backGroundImage.sprite = ResourceManager.Instance.HanbokSelected_Background;
            //CommonFunction.ChangeColorBtnAndTxt(transform);
            SetElgamoHanbokIndex(); // 한복인덱스 설정
        }
        else
        {
            backGroundImage.sprite = ResourceManager.Instance.HanbokNormal_Background;
            //CommonFunction.ChangeColorBtnAndTxt(transform, false);
        }
    }

    public void OnButtonClicked()
    {
        UIManager.Instance.DeselectAllCustomButtons(UIManager.Instance.HanbokContentButtons);
        SetSelected(true);
    }

    internal void FetchHanbokSprite(int v, (int, Sprite) value)
    {
        throw new NotImplementedException();
    }
}

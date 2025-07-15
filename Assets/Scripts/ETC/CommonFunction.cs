using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

static class CommonFunction
{
    public static string GetBaseCategoryString(Category_Base category) // 인사동 전용
    {
        switch (category)
        {
            case Category_Base.Default:
                break;
            case Category_Base.ToEat:
                return "인사 뭐먹지";
            case Category_Base.ToBuy:
                return "인사 뭐사지";
            case Category_Base.ToGallery:
                return "인사동 미술관";
            case Category_Base.ToHelp:
                return "인사 도와줘";
            case Category_Base.ToStay:
                return "인사동 숙박";
            default:
                return "default";
        }
        return null;
    }

    public static string GetSecondCategoryString(Category_ToEat secondCategory) // 인사동 전용
    {
        switch (secondCategory)
        {
            case Category_ToEat.Default:
                return "0-default";
            case Category_ToEat.KoreanBBQ:
                return "1-코리안 바베큐";
            case Category_ToEat.Korean:
                return "2-한식";
            case Category_ToEat.koreanTraditional:
                return "3-한정식";
            case Category_ToEat.Snack:
                return "4-분식";
            case Category_ToEat.TempleFood:
                return "5-사찰음식";
            case Category_ToEat.Vegetarian:
                return "6-채식/비건";
            case Category_ToEat.Asian:
                return "7-아시안";
            case Category_ToEat.Chinese:
                return "8-중식";
            case Category_ToEat.TraditionalTea:
                return "9-전통차";
            case Category_ToEat.Cafe:
                return "10-카페";
            case Category_ToEat.ETC:
                return "11-기타";
            default:
                return "default";
        }
    }

    // 문자열을 1파라미터로 받는다, 2파라미터문자를 기준으로 sprlit한다.  int를 파라미터로 받아 앞 혹은 뒤문자열을 Trim하여 반환한다.
    public static string SplitAndTrim(string input, char separator, int index = 0)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        var parts = input.Split(separator);
        if (index < 0 || index >= parts.Length)
            return string.Empty;
        return parts[index].Trim();
    }

    /// 하위 모든 자식에서 이름이 일치하는 GameObject를 찾음 (없으면 null 반환)
    public static GameObject FindDeepChild(GameObject parent, string name)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == name)
                return child.gameObject;

            var result = FindDeepChild(child.gameObject, name);
            if (result != null)
                return result;
        }
        return null;
    }

    public static int GetCountAllChildren(Transform parent) // 직계자식뿐만아닌 모든 자식들의 카운트
    {
        int count = 0;
        foreach (Transform child in parent)
        {
            count++;
            count += GetCountAllChildren(child);  // 재귀
        }
        return count;
    }
    public static void ChangeColorBtnAndTxt(Transform _target, bool isSelected = true) // 이미지로 되어있어서 이미지의 색을 바꾼다.
    {
        if (isSelected == true)
        {
            _target.GetComponent<Image>().color = UIColorPalette.SelectedColor;
            _target.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.SelectedTextColor;
        }
        else
        {
            _target.GetComponent<Image>().color = UIColorPalette.NormalColor;
            _target.GetChild(0).GetComponent<TextMeshProUGUI>().color = UIColorPalette.NormalTextColor;
        }
    }

    public static Texture2D GenerateQRCode(string text)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 256,
                Width = 256
            }
        };

        Color32[] pixels = writer.Write(text);
        Texture2D texture = new Texture2D(256, 256);
        texture.SetPixels32(pixels);
        texture.Apply();
        return texture;
    }

    public static Texture2D ConvertSpriteToTexture(Sprite sprite)
    {
        if (sprite == null) return null;

        // 스프라이트가 참조하는 텍스처에서 필요한 부분만 잘라내기
        var rect = sprite.textureRect;
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height);
        tex.SetPixels(sprite.texture.GetPixels(
            (int)rect.x,
            (int)rect.y,
            (int)rect.width,
            (int)rect.height
        ));
        tex.Apply();
        return tex;
    }
}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ReplaceTextToTMP : MonoBehaviour
{
    [MenuItem("Tools/Replace All Text with TMP (Scene Only)")]
    public static void ReplaceText()
    {
        Text[] allText = GameObject.FindObjectsOfType<Text>(true);

        foreach (Text text in allText)
        {
            GameObject go = text.gameObject;

            string content = text.text;
            int fontSize = text.fontSize;
            Color color = text.color;
            TextAnchor alignment = text.alignment;

            RectTransform rt = go.GetComponent<RectTransform>();
            Vector2 anchoredPos = rt.anchoredPosition;
            Vector2 sizeDelta = rt.sizeDelta;

            // 기존 Text 삭제
            DestroyImmediate(text);

            // TMP 추가
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = ConvertAlignment(alignment);

            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Debug.Log($"Replaced: {go.name}");

            tmp.font = Resources.Load<TMP_FontAsset>("Fonts/NotoSans/NotoSansKR-Regular SDF");
        }

        Debug.Log("All Text components replaced with TMP in current scene.");
    }

    static TextAlignmentOptions ConvertAlignment(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
            case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
            case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
            default: return TextAlignmentOptions.Left;
        }

    }
}
#endif

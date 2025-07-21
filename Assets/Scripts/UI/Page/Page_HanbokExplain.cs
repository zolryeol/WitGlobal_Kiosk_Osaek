using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Page_HanbokExplain : MonoBehaviour, IPrefabInstancer
{
    [SerializeField] Transform Content;

    public void CreateContentInstance()
    {
        // 1~4번 항목만 필터링
        var filteredDic = ResourceManager.Instance.HanbokSpritesDic
            .Where(pair => int.TryParse(pair.Key.Split(')')[0], out var idx) && idx >= 1 && idx <= 4)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        // 모든 Sprite들 Instantiate
        foreach (var kvp in filteredDic)
        {
            string folderName = kvp.Key;
            List<(string fileName, Sprite sprite)> spriteList = kvp.Value;

            foreach (var (fileName, _sprite) in spriteList)
            {
                var hanbokImage = GameObject.Instantiate(PrefabManager.Instance.HanbokExplainPrefab, Content);
                hanbokImage.transform.GetChild(0).GetComponent<Image>().sprite = _sprite;

                // 필요 시 텍스트 등도 설정 가능
                // hanbokImage.GetComponentInChildren<Text>().text = fileName;
            }
        }
    }
}

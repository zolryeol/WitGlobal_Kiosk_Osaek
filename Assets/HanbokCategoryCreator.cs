using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HanbokCategoryCreator : MonoBehaviour, IPrefabInstancer
{
    [SerializeField] Transform hanbokContentButtonParent;

    void IPrefabInstancer.CreateContentInstance()
    {
        int index = 1;

        foreach (var hanbokcategoryButton in ResourceManager.Instance.HanbokSpritesDic)
        {
            var _hanbokcategoryButton = Instantiate(PrefabManager.Instance.HanbokCategoryButtonPrefab, this.transform);

            var hanbokBtn = _hanbokcategoryButton.GetComponent<HanbokCategoryButton>();

            hanbokBtn.CategoryButtonIndex = index;
            hanbokBtn.GetComponentInChildren<TextMeshProUGUI>().text = hanbokcategoryButton.Key;

            hanbokBtn.HanbokSpriteList = hanbokcategoryButton.Value;

            UIManager.Instance.HanbokCategorieButtons.Add(hanbokBtn);

            index++;
        }

        CreateHanbokContent();
    }
    void CreateHanbokContent() // 실질적으로 클릭할 수 있는 한복버튼
    {
        var maxEntry = ResourceManager.Instance.HanbokSpritesDic.OrderByDescending(pair => pair.Value.Count).First();

        for (int i = 0; i < maxEntry.Value.Count; ++i)
        {
            var instance = Instantiate(PrefabManager.Instance.HanbokContentButtonPrefab, hanbokContentButtonParent);
            UIManager.Instance.HanbokContentButtons.Add(instance.GetComponent<HanbokContentButton>());
            instance.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 버튼 10개 만든다.
// contentinfo prefab으로 더미 미리 만들어둔다.

public class ShopContentCreator : MonoBehaviour, IPrefabInstancer
{
    [SerializeField] Transform contentParent;
    [SerializeField] private Transform buttonParent;
    public static int MaxContentCount;

    void CreateSecondCategoryButtons()
    {
        var childCount = buttonParent.childCount;

        var addedCount = UIManager.SecondCategoryMaxCount - childCount;

        for (int i = 0; i < addedCount; i++)
        {
            var secondCategoryButton = Instantiate(PrefabManager.Instance.SecondCategoryButtonPrefab, buttonParent);
            secondCategoryButton.SetActive(false);

            var btn = secondCategoryButton.GetComponent<SecondCategoryButton>();
            btn.SecondCategoryButtonIndex = i;  // 1번부터 시작 버튼마다 인덱스 설정
            btn.onClick.AddListener(() => UIManager.Instance.FetchingContent(btn.SecondCategoryButtonIndex));
            UIManager.Instance.SecondCategorieButtons.Add(btn);
        }
    }
    public void CreateContentInstance()
    {
        MaxContentCount = LoadManager.Instance.GetBaseCategoryMaxCount();

        var um = UIManager.Instance;

        var childCount = contentParent.childCount;

        var addedCount = MaxContentCount - childCount;

        for (int i = 0; i < addedCount; i++)
        {
            var content = Instantiate(PrefabManager.Instance.ContentItemInfoPrefab, contentParent);
            content.SetActive(false);
            um.ShopContents.Add(content.GetComponent<ShopContent>());
        }

        Debug.Log($"콘텐츠 {MaxContentCount} 개 인스턴싱");


        CreateSecondCategoryButtons();
    }
}

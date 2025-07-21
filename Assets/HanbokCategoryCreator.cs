// 개선된 HanbokCategoryCreator.cs
// 한 페이지에 6개씩 배치하며, 6개 초과 시 페이지 생성

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HanbokCategoryCreator : MonoBehaviour, IPrefabInstancer
{
    [SerializeField] Transform hanbokPageContainer; // ScrollRect의 Content 역할

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

            var lt = hanbokBtn.transform.GetChild(0).AddComponent<LocalizerText>(); // 한복 카테고리 텍스트 로컬라이저
            lt.SetKey($"Hanbok_{index}"); 
            index++;
        }

        CreateHanbokPages();
    }

    void CreateHanbokPages()
    {
        if (ResourceManager.Instance.HanbokSpritesDic == null || ResourceManager.Instance.HanbokSpritesDic.Count == 0)
        {
            Debug.LogWarning("[HanbokCategoryCreator] HanbokSpritesDic에 데이터가 없습니다.");
            return;
        }

        foreach (Transform child in hanbokPageContainer)
        {
            child.gameObject.SetActive(false);
        }

        // 가장 많은 콘텐츠 수를 가진 카테고리를 기준으로 생성
        var maxEntry = ResourceManager.Instance.HanbokSpritesDic
            .OrderByDescending(pair => pair.Value.Count)
            .First();

        int total = maxEntry.Value.Count;
        int itemsPerPage = 6;
        int totalPages = Mathf.CeilToInt(total / (float)itemsPerPage);

        for (int page = 0; page < totalPages; page++)
        {
            var pageGO = Instantiate(PrefabManager.Instance.HanbokPagePrefab, hanbokPageContainer);
            var grid = pageGO.GetComponent<GridLayoutGroup>();

            for (int i = 0; i < itemsPerPage; i++)
            {
                int idx = page * itemsPerPage + i;
                if (idx >= total) break;

                var contentBtnGO = Instantiate(PrefabManager.Instance.HanbokContentButtonPrefab, pageGO.transform);
                var contentBtn = contentBtnGO.GetComponent<HanbokContentButton>();
                UIManager.Instance.HanbokContentButtons.Add(contentBtn);

                contentBtnGO.SetActive(false); // 카테고리 선택 시 활성화됨
            }
        }
    }
}
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using Unity.VisualScripting;
//using UnityEngine;

//public class HanbokCategoryCreator : MonoBehaviour, IPrefabInstancer
//{
//    [SerializeField] Transform hanbokContentButtonParent;

//    void IPrefabInstancer.CreateContentInstance()
//    {
//        int index = 1;

//        foreach (var hanbokcategoryButton in ResourceManager.Instance.HanbokSpritesDic)
//        {
//            var _hanbokcategoryButton = Instantiate(PrefabManager.Instance.HanbokCategoryButtonPrefab, this.transform);

//            var hanbokBtn = _hanbokcategoryButton.GetComponent<HanbokCategoryButton>();


//            hanbokBtn.CategoryButtonIndex = index;
//            hanbokBtn.GetComponentInChildren<TextMeshProUGUI>().text = hanbokcategoryButton.Key;

//            hanbokBtn.HanbokSpriteList = hanbokcategoryButton.Value;

//            UIManager.Instance.HanbokCategorieButtons.Add(hanbokBtn);

//            var lt = hanbokBtn.transform.GetChild(0).AddComponent<LocalizerText>();
//            lt.SetKey($"Hanbok_{index}");

//            index++;
//        }

//        CreateHanbokContent();
//    }
//    void CreateHanbokContent() // 실질적으로 클릭할 수 있는 한복버튼
//    {
//        if (ResourceManager.Instance.HanbokSpritesDic == null || ResourceManager.Instance.HanbokSpritesDic.Count == 0)
//        {
//            Debug.LogWarning("[HanbokCategoryCreator] HanbokSpritesDic에 데이터가 없습니다.");
//            return;
//        }

//        var maxEntry = ResourceManager.Instance.HanbokSpritesDic
//            .OrderByDescending(pair => pair.Value.Count)
//            .First();

//        for (int i = 0; i < maxEntry.Value.Count; ++i)
//        {
//            var instance = Instantiate(PrefabManager.Instance.HanbokContentButtonPrefab, hanbokContentButtonParent);
//            UIManager.Instance.HanbokContentButtons.Add(instance.GetComponent<HanbokContentButton>());
//            instance.SetActive(false);
//        }
//    }
//}

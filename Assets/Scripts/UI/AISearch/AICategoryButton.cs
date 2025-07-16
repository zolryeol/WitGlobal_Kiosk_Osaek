using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AICategoryButton : MonoBehaviour, ILocalizable
{
    Button button;

    Page_AISelect aiSelector;

    TextMeshProUGUI categoryText;
    public bool IsSelected { get; private set; } = false;

    public AICategory category;

    public void Init(Page_AISelect _aiselector = null)
    {
        if (TryGetComponent<Button>(out Button _button))
        {
            button = _button;
        }
        else button = this.AddComponent<Button>();

        categoryText = GetComponentInChildren<TextMeshProUGUI>();
        if (_aiselector == null) Debug.LogError("[AICategoryButton] AISelector가 할당되지 않았습니다. Init 메서드에 AISelector를 전달해주세요.");
        aiSelector = _aiselector;
        button.onClick.AddListener(OnSelected);
    }
    public void SetCategoryInfo(int index)
    {
        // index를 버튼의 인덱스와 카테고리의 Num으로 사용한다고 가정
        category = ShopManager.Instance.AICategorieList.Find(t => t.Num == index + 1);

        if (category != null)
        {
            // 현재 언어에 맞는 카테고리명 표시 (예시: 한국어)
            var oriText = category.AICategoryString[(int)UIManager.Instance.NowLanguage];
            var result = CommonFunction.SplitAndTrim(oriText, '-', 1); // -의 뒷부분
            categoryText.text = result;

            //categoryText.text = category.AICategoryString[(int)UIManager.Instance.NowLanguage];
        }
        else
        {
            categoryText.text = "Unknown";
            Debug.LogWarning($"[AICategoryButton] 카테고리 인덱스({index})에 해당하는 카테고리가 없습니다.");
        }
    }

    public void OnSelected()
    {
        if (IsSelected == false)
        {
            if (3 <= aiSelector.aiSelectedCount)
            {
                Debug.LogWarning("[AICategoryButton] 최대 선택 개수를 초과했습니다.");
                return;
            }

            IsSelected = true;
            CommonFunction.ChangeColorBtn(transform);

            aiSelector.aiSelectedCount++;
            aiSelector.AddSelectedCategory(category);
        }
        else
        {
            IsSelected = false;
            CommonFunction.ChangeColorBtn(transform, false);

            aiSelector.aiSelectedCount--;
            aiSelector.RemoveSelectedCategory(category);
        }

        aiSelector.UpdateGenerateButtonState();
    }

    public void DeSelected()
    {
        IsSelected = false;
        CommonFunction.ChangeColorBtn(transform, false);
        aiSelector.aiSelectedCount--;
        aiSelector.RemoveSelectedCategory(category);
    }


    public void UpdateLocalizedString(string str = null)
    {
        SetCategoryInfo(category.Num);

        if (str == null)
        {
            Debug.LogWarning("텍스트 NUll");
            return;
        }
        categoryText.text = str;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HanbokPage : MonoBehaviour
{
    [SerializeField] Transform buttonParent; // 버튼들이 배치될 부모 오브젝트 (자기 자신도 가능)
    public List<HanbokContentButton> Buttons { get; private set; } = new();

    /// <summary>
    /// 콘텐츠 버튼 슬롯을 하나 생성합니다.
    /// </summary>
    public HanbokContentButton CreateSlot()
    {
        GameObject buttonGO = Instantiate(PrefabManager.Instance.HanbokContentButtonPrefab, buttonParent != null ? buttonParent : this.transform);
        var button = buttonGO.GetComponent<HanbokContentButton>();
        Buttons.Add(button);
        return button;
    }

    /// <summary>
    /// 이 페이지의 모든 콘텐츠 버튼을 비활성화합니다.
    /// </summary>
    public void HideAllButtons()
    {
        foreach (var btn in Buttons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 이 페이지의 모든 콘텐츠 버튼을 활성화합니다.
    /// </summary>
    public void ShowAllButtons()
    {
        foreach (var btn in Buttons)
        {
            btn.gameObject.SetActive(true);
        }
    }

}

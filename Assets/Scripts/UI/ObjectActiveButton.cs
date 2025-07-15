using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 그냥 담아둔 오브젝트 껐다 켰다하는 용도의 버튼
/// </summary>
public class ObjectActiveButton : MonoBehaviour
{
    Button button;

    [SerializeField] GameObject targetObject;
    private void Awake()
    {
        button = GetComponent<Button>();

        if (targetObject == null)
        {
            Debug.LogError("오브젝트 할당필요" + this.gameObject.name);
            return;
        }

        button.onClick.AddListener(OnTargetObject);
    }

    public void OnTargetObject()
    {
        if (targetObject == null)
        {
            Debug.LogError("오브젝트 할당필요" + this.gameObject.name);
            return;
        }

        if (targetObject.activeSelf)
        {
            targetObject.SetActive(false);
        }
        else
        {
            targetObject.SetActive(true);
        }
    }
}

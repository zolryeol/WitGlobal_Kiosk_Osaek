using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrivacyPolicyController : MonoBehaviour
{
    [SerializeField]
    private GameObject privacyPolicyButton;

    [SerializeField]
    private GameObject PrivacyPolicyModal;

    [SerializeField]
    private GameObject targetObject;

    [SerializeField]
    private GameObject xbutton;

    public void onClick()
    {
        //Debug.LogError("PrivacyButton Click");
        PrivacyPolicyModal.SetActive(true);

    }
    //void Update()
    //{
    //    // 마우스 클릭 감지
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (IsPointerOverGameObject(targetObject))
    //        {
    //            // true 영역 내부
    //            //Debug.Log("GameObject 내부를 클릭했습니다!");
    //        }
    //        else
    //        {
    //            // false 영역 외부
    //            OnOutsideClick();
    //        }
                
    //    }
    //}

    private bool IsPointerOverGameObject(GameObject gameObject)
    {
        // 특정 GameObject 영역 확인
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return false; // RectTransform이 없으면 UI 영역이 아님
        }

        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(localMousePosition);
    }

    private void OnOutsideClick()
    {
        // 영역 외부 클릭 시 실행할 이벤트
        //Debug.Log("GameObject 외부를 클릭했습니다!");
        PrivacyPolicyModal.SetActive(false);
    }

    public void XButtonClick()
    {
        //Debug.LogError("XButtonClick");
        PrivacyPolicyModal.SetActive(false);
    }
}

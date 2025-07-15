using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierFreeButtonController : MonoBehaviour
{

    [SerializeField]
    private GameObject barrierFreeButton;

    [SerializeField]
    private GameObject barrierFreeModal;

    [SerializeField]
    private GameObject targetObject;

    [SerializeField]
    private GameObject xbutton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void onClick()
    {
        GlobalManager.Instance.barrierFreeFlag = true;
        barrierFreeModal.SetActive(true);
        //Debug.LogError("BarrierFreeButton Click");
        //Debug.LogError($"BarrierFreeFlag : {GlobalManager.Instance.barrierFreeFlag}");

    }
    void Update()
    {
        // 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverGameObject(targetObject))
            {
                // true 영역 내부
                //Debug.Log("GameObject 내부를 클릭했습니다!");
            }
            else
            {
                // false 영역 외부
                OnOutsideClick();
            }

        }
    }

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
        barrierFreeModal.SetActive(false);
    }

    public void XButtonClick()
    {
        //Debug.LogError("XButtonClick");
        GlobalManager.Instance.barrierFreeFlag = false;
        barrierFreeModal.SetActive(false);
        //Debug.LogError($"BarrierFreeFlag : {GlobalManager.Instance.barrierFreeFlag}");
    }
}

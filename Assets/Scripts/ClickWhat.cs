using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickWhat : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // 클릭 위치를 스크린 좌표에서 월드 좌표로 변환
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 레이캐스트로 클릭된 객체 확인
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("클릭된객체 : " + hit.collider.gameObject.name);
            }

            // UI 요소 클릭 감지
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            RaycastResult raycastResult = new RaycastResult();
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    Debug.Log("클릭된객체 : " + result.gameObject.name);
                }
            }
        }
    }
}

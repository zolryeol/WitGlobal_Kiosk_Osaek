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
            // Ŭ�� ��ġ�� ��ũ�� ��ǥ���� ���� ��ǥ�� ��ȯ
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ����ĳ��Ʈ�� Ŭ���� ��ü Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Ŭ���Ȱ�ü : " + hit.collider.gameObject.name);
            }

            // UI ��� Ŭ�� ����
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
                    Debug.Log("Ŭ���Ȱ�ü : " + result.gameObject.name);
                }
            }
        }
    }
}

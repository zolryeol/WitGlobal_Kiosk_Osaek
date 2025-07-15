using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveWeatherScroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private float speed;
    [SerializeField]
    private RectTransform content;

    private bool isScrolling = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isScrolling = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isScrolling = false;
    }

    private void Update()
    {
        if (isScrolling)
        {
            moveScroll();
        }
    }

    public void moveScroll()
    {
        Vector3 position = content.anchoredPosition;
        position += direction * speed * Time.deltaTime; // 시간에 따라 부드럽게 이동
        content.anchoredPosition = position;
    }
}

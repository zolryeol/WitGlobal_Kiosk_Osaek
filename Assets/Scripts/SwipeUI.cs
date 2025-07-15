using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/*
 * 캐러셀 구현을 위한 스크립트
 */
public class SwipeUI : MonoBehaviour
{
    [SerializeField]
    private Image[] imageArr;                       // 현재 선택되고 있는 이미지를 바꾸기 위한 변수
    [SerializeField]
    private Sprite basicImg;                       // 기본 이미지
    [SerializeField]
    private Sprite highlightImg;                   // 강조 이미지
    [SerializeField]
    private Scrollbar scrollBar;                      // ScrollBar의 위치를 바탕으로 현재 페이지 검사
    [SerializeField]
    private float swipeTime = 0.2f;               // 페이지가 스와이프 되는 시간
    [SerializeField]
    private float pageSize = 588.0f;              // 각 페이지의 크기
    [SerializeField]
    private float spacing = 50.0f;                // 페이지 사이의 간격

    private float[] scrollPageValues;               // 각 페이지의 위치 값 [0.0 - 1.0]
    private float valueDistance = 0;              // 각 페이지 사이의 거리
    public int currentPage = 0;                // 현재 페이지
    private int maxPage = 0;                    // 최대 페이지
    private float startTouchX;                    // 터치 시작 위치
    //private float       endTouchX;                      // 터치 종료 위치
    private bool isSwipeMode = false;            // 현재 Swipe가 되고 있는지 체크

    [SerializeField]
    private float swipeThreshold = 50f; // 스와이프 판정 거리

    private void Awake()
    {
        // 페이지 개수
        int childCount = transform.childCount;

        // 스크롤 되는 페이지의 각 value 값을 저장하는 배열 메모리 할당
        scrollPageValues = new float[childCount];

        // 스크롤 되는 페이지 사이의 거리
        valueDistance = (pageSize + spacing) / ((pageSize + spacing) * (childCount - 1));

        // 스크롤 되는 페이지의 각 value 위치 설정 [0 <= value <= 1]
        for (int i = 0; i < scrollPageValues.Length; i++)
        {
            scrollPageValues[i] = valueDistance * i;
        }

        // 최대 페이지의 수
        maxPage = childCount;
    }

    private void Start()
    {
        // 최초 시작시 0번 페이지 설정 및 이미지 강조
        SetScrollBarValue(0);
        imageArr[currentPage].sprite = highlightImg;
    }

    private void SetScrollBarValue(int index)
    {
        currentPage = index;
        scrollBar.value = scrollPageValues[index];
    }

    public void onClickLeft()
    {
        UpdateSwipe(true);
    }
    public void onClickRight()
    {
        UpdateSwipe(false);
    }

    private void UpdateSwipe(bool isLeft)
    {
        // 현재 스와이프가 진행중이면 return
        if (isSwipeMode == true) return;

        // 기존 페이지 인덱스
        int prevPage = currentPage;

        if (isLeft)
        {
            // 현재페이지가 왼쪽 끝이면 종료
            if (currentPage == 0) return;

            // 현재 페이지 1 감소
            currentPage--;
        }
        else
        {
            // 현재 페이지가 오른쪽 끝이면 종료
            if (currentPage == maxPage - 1) return;

            // 오른쪽으로 이동을 위해 현재 페이지를 1 증가
            currentPage++;
        }

        // 현재 페이지 강조효과
        imageArr[currentPage].sprite = highlightImg;
        // 기존 페이지 강조효과 삭제
        imageArr[prevPage].sprite = basicImg;

        // currentIndex번째 페이지로 swipe해서 이동
        StartCoroutine(OnSwipeOneStep(currentPage));

    }

    // 페이지를 한장 옆으로 넘기는 swipe 효과 재생
    private IEnumerator OnSwipeOneStep(int index)
    {
        float start = scrollBar.value;
        float current = 0;
        float percent = 0;

        isSwipeMode = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / swipeTime;

            scrollBar.value = Mathf.Lerp(start, scrollPageValues[index], percent);

            yield return null;
        }

        isSwipeMode = false;
    }

    // 터치 입력 감지
    //    private void Update()
    //    {
    //        // 터치 시작 감지
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            startTouchX = Input.mousePosition.x;
    //        }

    //        // 터치 종료 감지
    //        if (Input.GetMouseButtonUp(0))
    //        {
    //            float endTouchX = Input.mousePosition.x;
    //            float deltaX = endTouchX - startTouchX;

    //            // 스와이프 거리 확인
    //            if (Mathf.Abs(deltaX) > swipeThreshold)
    //            {
    //                // 스와이프 방향에 따라 페이지 변경
    //                if (deltaX > 0)
    //                {
    //                    onClickLeft(); // 왼쪽으로 스와이프
    //                }
    //                else
    //                {
    //                    onClickRight(); // 오른쪽으로 스와이프
    //                }
    //            }
    //        }
    //    }
}

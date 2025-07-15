using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
    public class CustomEventSystem : EventSystem
    {
        // 길게 누르기 방지 관련 변수
        private Dictionary<int, float> touchStartTimes = new Dictionary<int, float>();
        public float maxPressDuration = 0.2f; // 길게 누르기 제한 시간 (초)

        // 스와이프 방지 관련 변수
        private Vector2 swipeStartPos; // 스와이프 시작 위치
        public float swipeThreshold = 6.3f; // Y축 스와이프 감지 최소 거리

        protected override void Update()
        {
            int touchCount = Input.touchCount;

            // 다중 터치 방지
            if (touchCount > 1)
            {
                Debug.Log($"다중 터치 감지 - 손가락 {touchCount}개, 동작 중단");
                ClearAllTouches();
                return;
            }

            // 터치가 있는 경우 처리
            if (touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                // 아래에서 위로 스와이프 방지
                if (HandleSwipePrevention(touch))
                {
                    Debug.Log("아래에서 위로 스와이프 차단됨");
                    return; // 스와이프 동작 무효화
                }

                // 길게 누르기 방지
                HandleLongPressPrevention(touch);
            }

            base.Update(); // 기본 EventSystem 로직 처리
        }

        private bool HandleSwipePrevention(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                // 터치 시작 위치 저장
                swipeStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // 스와이프 방향 계산
                Vector2 swipeEndPos = touch.position;
                float deltaY = swipeEndPos.y - swipeStartPos.y;

                // 아래에서 위로 스와이프 확인
                if (swipeStartPos.y < Screen.height * 0.1f // 화면 하단 20%에서 시작
                    && deltaY > swipeThreshold // Y축 변화가 최소 임계값 초과
                    && Mathf.Abs(swipeEndPos.x - swipeStartPos.x) < swipeThreshold) // 대각선 방지
                {
                    return true; // 아래에서 위로 스와이프 감지
                }
            }

            return false; // 스와이프가 아닌 경우
        }

        private void HandleLongPressPrevention(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                // 터치 시작 시간 기록
                if (!touchStartTimes.ContainsKey(touch.fingerId))
                {
                    touchStartTimes.Add(touch.fingerId, Time.time);
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // 터치 지속 시간 계산
                if (touchStartTimes.TryGetValue(touch.fingerId, out float startTime))
                {
                    float duration = Time.time - startTime;

                    if (duration > maxPressDuration)
                    {
                        Debug.Log($"Finger {touch.fingerId}: 길게 누르기로 간주, 동작 취소됨");
                    }
                    else
                    {
                        Debug.Log($"Finger {touch.fingerId}: 정상적인 터치로 간주, 동작 실행");
                    }

                    // 기록 삭제
                    touchStartTimes.Remove(touch.fingerId);
                }
            }
        }

        private void ClearAllTouches()
        {
            // 모든 터치 데이터를 초기화하여 동작 차단
            touchStartTimes.Clear();
            Debug.Log("모든 터치 데이터 초기화됨");
        }
    }
}

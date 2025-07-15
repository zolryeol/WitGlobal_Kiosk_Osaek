using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HanbokConfirm : MonoBehaviour
{
    // 어느 한복이 선택됐냐를 가져오기 위한 변수들
    [SerializeField]
    private HanbokButtonController hanbokButtonController;
    [SerializeField]
    private SwipeUI[] swipeUI;

    // 한복 프리펩 모음
    [Header("Hanbok")]
    [SerializeField]
    private GameObject[] womenHanbok;
    [SerializeField]
    private GameObject[] menHanbok;
    [SerializeField]
    private GameObject[] palaceHanbok;
    [SerializeField]
    private GameObject[] eventHanbok;

    // 사진이 다 찍힐때까지 화면 클릭 방지용
    [SerializeField]
    private GameObject blocking;
    [SerializeField]
    private GameObject cameraFocus;

    // 선택완료 버튼 눌러야 사진찍기 버튼 나오기
    [SerializeField]
    private GameObject photoButton;
    
    [SerializeField]
    private GameObject photoButtonText;

    // 버튼 클릭시 생성될 AR화면 및 요소들
    [SerializeField]
    private GameObject rawImage;
    [SerializeField]
    private GameObject photoText;
    [SerializeField]
    private GameObject countDown;
    [SerializeField]
    private GameObject arController;

    // 한복 생성 위치
    [SerializeField]
    private float hanbokPosX;

    // 한복 및 AR화면 부모 위치
    [SerializeField]
    private Transform arTransform;

    private int buttonIndex = 0;
    private int hanbokIndex = 0;

    // 선택완료 버튼 누를시 확정 및 동영상 끊기
    public void onClick()
    {
        // 사진찍기 버튼 나오기
        photoButtonText.SetActive(true);
        photoButton.SetActive(true);

        // Text 게임 오브젝트가 Image보다 앞에 위치하도록 설정
        photoButtonText.transform.SetSiblingIndex(photoButton.transform.GetSiblingIndex() + 1);

        // 선택된 한복 index
        buttonIndex = FindObjectOfType<HanbokButtonController>().currentIndex;
        hanbokIndex = swipeUI[buttonIndex].currentPage;

        /* 베트남 API Check 필수 
         * 여자한복 4 남자한복 1 궁중한복 3 인사 2
         * index 1 : 여자 한복 흰색+자주
         * index 2 : 여자 궁중한복 (2번째)
         * index 3 : 여자 인사 교복 (2번째)
         * index 4 : 남자 선비
         * index 5 : 여자 여자한복 민소매
         * index 6 : 남자 궁중한복 임금(3번째)
         * index 7 : 여자 궁중한복 분홍+파랑 (1번째)
         * index 8 : 여자 여자한복 빨강+녹색치마(2번째)
         * index 9 : 여자 여자한복 검정 치마(4번째)
         * index 10 : 여자 인사 파티복 (1번째)
         * index 11 : 여자 인사 페이퍼 드레스 (3번째)
         */


        //여자한복 흰색+자주
        if (buttonIndex == 0 && hanbokIndex == 2)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 1);
        }
        // 궁중한복 공주옷
        else if (buttonIndex == 2 && hanbokIndex == 1)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 2);
        }
        // 인사 교복
        else if (buttonIndex == 3 && hanbokIndex == 1)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 3);
        }
        // 남자한복 선비
        else if (buttonIndex == 1 && hanbokIndex == 0)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 4);
        }
        // 여자한복 민소매
        else if (buttonIndex == 0 && hanbokIndex == 0)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 5);
        }
        // 궁중한복 임금
        else if (buttonIndex == 2 && hanbokIndex == 2)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 6);
        }
        // 여자 궁중한복 분홍+파랑 (1번째)
        else if (buttonIndex == 2 && hanbokIndex == 0)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 7);
        }
        //  여자 여자한복 빨강+녹색치마(2번째)
        else if (buttonIndex == 0 && hanbokIndex == 1)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 8);
        }
        //  여자 여자한복 검정 치마(4번째)
        else if (buttonIndex == 0 && hanbokIndex == 3)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 9);
        }
        //  여자 인사 파티복(1번째)
        else if (buttonIndex == 3 && hanbokIndex == 0)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 10);
        }
        //  여자 인사 페이퍼 드레스(3번째)
        else if (buttonIndex == 3 && hanbokIndex == 2)
        {
            PlayerPrefs.SetInt("testHanbokIndex", 11);
        }


    }

    // 사진찍기 버튼 누를시 시작
    public void onPhotoButtonClick()
    {
        // 클릭 방지
        // 원본 blocking size 1334, 1980 -> scale x, y 1 인데 1.3 로 수정
        blocking.SetActive(true);

        // 화살표 표시
        cameraFocus.SetActive(true);

        // 카메라 화면 불러오기
        rawImage.SetActive(true);
        photoText.SetActive(true);
        countDown.SetActive(true);
        arController.SetActive(true);

    }
}

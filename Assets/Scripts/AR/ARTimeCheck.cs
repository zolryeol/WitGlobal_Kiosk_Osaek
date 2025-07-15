using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARTimeCheck : MonoBehaviour
{
    // AR안내메시지
    [SerializeField]
    private GameObject arTimeAlert;

    // AR 버튼 클릭시
    public void OnClickArButton()
    {
        int time = int.Parse(DateTime.Now.ToString(("HHmm")));

        // 시간이 맞으면 페이지 이동
        //if (time >= 1000 && time <= 2000)
        //{
        SceneManager.LoadSceneAsync("ArSelect");
        //}
        //// 시간이 안맞으면 블로킹
        //else
        //{
        //    arTimeAlert.SetActive(true);
        //}
    }

    // X버튼 클릭 이벤트
    public void OnClickCloseButton()
    {
        arTimeAlert.SetActive(false);
    }
}

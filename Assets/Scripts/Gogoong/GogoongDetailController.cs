using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement;

public class GogoongDetailController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] googoongDetails;

    [SerializeField]
    private Sprite qr_preImage;

    private void Start()
    {
        UpdateDetailPage();
        
        StartCoroutine(LoadSceneAfterDelay(3f * 60f));

    }
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    private void UpdateDetailPage()
    {
        // 어떤 고궁을 통해 들어왔는지 확인 후 해당하는 고궁 띄워줌
        int index = PlayerPrefs.GetInt("googoongIndex");
        Debug.Log("index : " + index);
        googoongDetails[index].SetActive(true);

        RawImage qrImage = GameObject.Find("QRRaw").GetComponentInChildren<RawImage>();
        qrImage.texture = qr_preImage.texture;

        switch (index)
        {
            case 0:
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("서울특별시 종로구 사직로 161")))}&flag=help&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("경복궁")))}&apn=com.witdiocianapp", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());                
                QRCreator.CreateQR($"https://map.naver.com/p/entry/place/11571707?c=13.00,0,0,0,dh", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());                
                break;
            case 1:
            //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("서울특별시 종로구 율곡로 99")))}&flag=help&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("창덕궁")))}&apn=com.witdiocianapp", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());                
            QRCreator.CreateQR($"https://map.naver.com/p/entry/place/12261493?c=15.00,0,0,0,dh", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());                
                break;
            case 2:
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("서울특별시 종로구 창경궁로 185")))}&flag=help&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("창경궁")))}&apn=com.witdiocianapp", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());                
                QRCreator.CreateQR($"https://map.naver.com/p/entry/place/11571740?c=15.00,0,0,0,dh", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());
                break;
            case 3:
                //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/QcRv?addr={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("서울특별시 중구 세종대로 99")))}&flag=help&restnm={Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("덕수궁")))}&apn=com.witdiocianapp", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());
                QRCreator.CreateQR($"https://map.naver.com/p/entry/place/11571730?c=7.00,0,0,0,dh", GameObject.Find("QRRaw").GetComponentInChildren<RawImage>());
                break;
        }


    }
}

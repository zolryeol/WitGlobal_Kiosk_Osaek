using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveToGogoongDetail : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;

    private void Start()
    {
        // 각 버튼에 클릭 이벤트를 할당
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // 캡처한 인덱스
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnButtonClick(int index)
    {
        PlayerPrefs.SetInt("googoongIndex", index);
        SceneManager.LoadScene("GogoongDetail");
    }
}

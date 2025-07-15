using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveScene : MonoBehaviour
{
    [SerializeField] private string targetScene; // 이동할 씬

    // 씬 이동 메서드 (페이드 아웃 효과 후 씬 이동)
    public void moveScene()
    {
        if (targetScene == "TransGuide_Footer") 
        {
            PlayerPrefs.SetString("FooterMap", "ACTIVE");
            targetScene = "TransGuide";
        }

        SceneManager.LoadSceneAsync(targetScene);
        //ResolutionController.Instance.LoadSceneWithResolutionAdjustment(targetScene);
        Debug.LogWarning($"targetScene : {targetScene}");

        
    }

}
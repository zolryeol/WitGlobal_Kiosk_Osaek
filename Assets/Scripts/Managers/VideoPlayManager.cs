using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayManager : MonoBehaviour
{
    public static VideoPlayManager Instance { get; private set; }
    public VideoPlayer _VideoPlayer; // 
    public RenderTexture Display2Texture; // 디스플레이2용 RenderTexture

    public VideoClip DefaultClip;
    public VideoClip SelectPhotoHanbok;
    public VideoClip WaitCreatePhoto;


    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ActivateDisplay2();
    }

    // 영상다오면 파라미터로 받아서 해당 영상 플레이시킬것
    public void PlayVideo(VideoType videoType)
    {
        if (_VideoPlayer.clip == ResourceManager.Instance.VideoClipDic[videoType])
        {
            Debug.Log("같은영상 " + videoType);
            return;
        }
        _VideoPlayer.clip = ResourceManager.Instance.VideoClipDic[videoType];
        _VideoPlayer.Play();
    }

    public void ActivateDisplay2()
    {
        if (Display.displays.Length > 1)
        {
            // Display 2 활성화
            Display.displays[1].Activate();

            // VideoPlayer 설정
            if (_VideoPlayer != null && Display2Texture != null)
            {
                if (_VideoPlayer.isActiveAndEnabled) return;

                _VideoPlayer.targetTexture = Display2Texture;
                _VideoPlayer.Play();
            }
            else
            {
                Debug.LogError("VideoPlayer 또는 RenderTexture가 설정되지 않았습니다.");
            }
        }
        else
        {
            //StartCoroutine(RetryFindDisplay());
            Debug.LogWarning("Display 2가 감지되지 않았습니다.");
        }
    }

}

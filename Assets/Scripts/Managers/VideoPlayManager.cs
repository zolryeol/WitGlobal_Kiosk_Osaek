using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayManager : MonoBehaviour
{
    public static VideoPlayManager Instance { get; private set; }
    public VideoPlayer videoPlayer; // 
    public RenderTexture display2Texture; // 디스플레이2용 RenderTexture
    public Dictionary<string, VideoClip> VideoClip = new();

    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ActivateDisplay2();
    }

    public void ActivateDisplay2()
    {
        if (Display.displays.Length > 1)
        {
            // Display 2 활성화
            Display.displays[1].Activate();

            // VideoPlayer 설정
            if (videoPlayer != null && display2Texture != null)
            {
                if (videoPlayer.isActiveAndEnabled) return;

                videoPlayer.targetTexture = display2Texture;
                videoPlayer.Play();
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

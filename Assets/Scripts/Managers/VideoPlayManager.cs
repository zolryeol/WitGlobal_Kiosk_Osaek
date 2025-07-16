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
    public Dictionary<VideoType, VideoClip> VideoClipDic = new();

    public VideoClip DefaultClip;
    public VideoClip SelectPhotoHanbok;
    public VideoClip WaitCreatePhoto;


    public void Init()
    {
        LoadVideos();

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ActivateDisplay2();
    }

    // 영상다오면 파라미터로 받아서 해당 영상 플레이시킬것
    public void PlayVideo(VideoType videoType)
    {
        _VideoPlayer.clip = VideoClipDic[videoType];
        _VideoPlayer.isLooping = true;
        _VideoPlayer.Play();
    }

    public void LoadVideos()
    {
        var videos = Resources.LoadAll<VideoClip>("Video");
        foreach (var v in videos)
        {
            if (Enum.TryParse<VideoType>(v.name, out var _videoType))
            {
                VideoClipDic.Add(_videoType, v);
            }
            else
            {
                Debug.LogWarning($"'{v.name}'는 VideoType enum에 존재하지 않습니다.");
            }
        }
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

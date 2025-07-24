using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    public TextMeshProUGUI SubTitle;
    public GameObject PackLogo;

    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ActivateDisplay2();
    }

    // 영상다오면 파라미터로 받아서 해당 영상 플레이시킬것
    public void PlayVideo(VideoType videoType)
    {
        bool isSameClip = (_VideoPlayer.clip == ResourceManager.Instance.VideoClipDic[videoType]);

        // 로고 표시 여부
        PackLogo.SetActive(videoType == VideoType.Default);

        // 클립 설정
        _VideoPlayer.clip = ResourceManager.Instance.VideoClipDic[videoType];
        _VideoPlayer.Stop();               // 영상 초기화
        _VideoPlayer.time = 0;             // 0초로 강제 이동
        _VideoPlayer.Prepare();            // 준비 시작

        // 준비되면 재생
        _VideoPlayer.prepareCompleted += (vp) =>
        {
            vp.Play();
            ShowSubtitle(videoType);
            Debug.Log((isSameClip ? "같은" : "다른") + " 영상 재생됨: " + videoType);
        };
    }
    private void ShowSubtitle(VideoType videoType)
    {
        string key = videoType.ToString(); // ex: "Default"
        var subtitleData = LoadManager.Instance.VideoSubTitleList
            .FirstOrDefault(sub => sub.key == key);

        if (subtitleData != null)
        {
            var langIdx = (int)UIManager.Instance.NowLanguage;
            SubTitle.text = subtitleData.SubtitleString[langIdx];
        }
        else
        {
            SubTitle.text = ""; // 자막 없을 경우 비우기
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

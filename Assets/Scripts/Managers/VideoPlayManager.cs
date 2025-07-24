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
    private VideoType _currentVideoType;

    private Coroutine _retryDisplayCoroutine;
    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _retryDisplayCoroutine = StartCoroutine(TryActivateDisplay2());
    }

    // 영상다오면 파라미터로 받아서 해당 영상 플레이시킬것
    //public void PlayVideo(VideoType videoType)
    //{
    //    bool isSameClip = (_VideoPlayer.clip == ResourceManager.Instance.VideoClipDic[videoType]);

    //    if (isSameClip)
    //    {
    //        Debug.Log("같은 영상이 이미 재생 중입니다. 재생 생략: " + videoType);
    //        return;
    //    }

    //    // 로고 표시 여부
    //    PackLogo.SetActive(videoType == VideoType.Default);

    //    // 클립 설정
    //    _VideoPlayer.clip = ResourceManager.Instance.VideoClipDic[videoType];
    //    _VideoPlayer.Stop();               // 영상 초기화
    //    _VideoPlayer.time = 0;             // 0초로 강제 이동
    //    _VideoPlayer.Prepare();            // 준비 시작

    //    // 준비되면 재생
    //    _VideoPlayer.prepareCompleted += (vp) =>
    //    {
    //        vp.Play();
    //        ShowSubtitle(videoType);
    //        Debug.Log((isSameClip ? "같은" : "다른") + " 영상 재생됨: " + videoType);
    //    };
    //}

    public void PlayVideo(VideoType videoType)
    {
#if UNITY_EDITOR
        if (!ResourceManager.Instance.VideoClipDic.TryGetValue(videoType, out var clip))
        {
            Debug.LogError($"[VideoPlayManager] VideoClip 없음: {videoType}");
            return;
        }

        bool isSameClip = (_VideoPlayer.clip == clip);
        if (isSameClip)
        {
            Debug.Log($"[VideoPlayManager] 같은 영상 생략: {videoType}");
            return;
        }

        PackLogo.SetActive(videoType == VideoType.Default);

        _VideoPlayer.prepareCompleted -= OnPrepareCompleted;
        _VideoPlayer.source = VideoSource.VideoClip;
        _VideoPlayer.clip = clip;
        _VideoPlayer.Stop();
        _VideoPlayer.time = 0;
        _VideoPlayer.prepareCompleted += OnPrepareCompleted;
        _VideoPlayer.Prepare();

#else
    if (!ResourceManager.Instance.TryGetPreloadedVideoPlayer(videoType, out var preparedVP))
    {
        Debug.LogError($"[VideoPlayManager] PreloadedVideoPlayer 없음: {videoType}");
        return;
    }

    if (!preparedVP.isPrepared)
    {
        Debug.LogWarning($"[VideoPlayManager] 준비 안됨: {videoType}");
        return;
    }

    PackLogo.SetActive(videoType == VideoType.Default);

    _VideoPlayer.prepareCompleted -= OnPrepareCompleted;
    _VideoPlayer.source = VideoSource.Url;
    _VideoPlayer.url = preparedVP.url;
    _VideoPlayer.Stop();
    _VideoPlayer.time = 0;
    _VideoPlayer.prepareCompleted += OnPrepareCompleted;
    _VideoPlayer.Prepare();
#endif

        _currentVideoType = videoType;
    }

    private void OnPrepareCompleted(VideoPlayer vp)
    {
        vp.Play();
        ShowSubtitle(_currentVideoType);
        Debug.Log($"[VideoPlayManager] 영상 재생됨: {_currentVideoType}");
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

                PlayVideo(VideoType.Default); // 기본 영상 재생
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


    IEnumerator TryActivateDisplay2()
    {
        int maxRetryCount = 10;
        float retryInterval = 1f; // 1초 간격
        int currentRetry = 0;

        while (currentRetry < maxRetryCount)
        {
            if (Display.displays.Length > 1)
            {
                Display.displays[1].Activate();

                if (_VideoPlayer != null && Display2Texture != null)
                {
                    _VideoPlayer.targetTexture = Display2Texture;

                    PlayVideo(VideoType.Default); // 기본 영상 재생
                    Debug.Log("[VideoPlayManager] Display 2 활성화 및 기본 영상 재생 성공");
                    yield break;
                }
                else
                {
                    Debug.LogError("[VideoPlayManager] VideoPlayer 또는 RenderTexture가 설정되지 않았습니다.");
                    yield break;
                }
            }
            else
            {
                Debug.LogWarning($"[VideoPlayManager] Display 2 감지 실패, 재시도 {currentRetry + 1}/{maxRetryCount}...");
                currentRetry++;
                yield return new WaitForSeconds(retryInterval);
            }
        }

        Debug.LogError("[VideoPlayManager] Display 2를 최종적으로 감지하지 못했습니다.");
    }
}

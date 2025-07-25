using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

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

    private Dictionary<VideoType, int> videoPlayIndexMap = new();

    private VideoSubtitleData nextSubtitleData;
    private VideoType currentPlayingType;

    public void PlayVideo(VideoType type)
    {
        if (!ResourceManager.Instance.VideoMap.TryGetValue(type, out var list) || list.Count == 0)
        {
            Debug.LogWarning($"[VideoPlayManager] 자막 리스트 없음: {type}");
            return;
        }

        // 현재 재생 중인 타입 저장
        currentPlayingType = type;

        // 순차 인덱스 계산
        if (!videoPlayIndexMap.TryGetValue(type, out int currentIndex))
        {
            currentIndex = 0;
        }

        var selected = list[currentIndex];
        videoPlayIndexMap[type] = (currentIndex + 1) % list.Count;

        if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out var player))
        {
            Debug.LogWarning($"[VideoPlayManager] 비디오 파일 없음: {selected.fileName}");
            return;
        }

        _VideoPlayer.prepareCompleted -= OnVideoPrepared;
        _VideoPlayer.prepareCompleted += OnVideoPrepared;

        _VideoPlayer.loopPointReached -= OnVideoFinished;
        _VideoPlayer.loopPointReached += OnVideoFinished;

        _VideoPlayer.source = VideoSource.Url;
        _VideoPlayer.url = player.url;

        // 다음 자막 표시용 임시 저장
        nextSubtitleData = selected;

        _VideoPlayer.Prepare();
    }
    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        ShowSubtitle(nextSubtitleData);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("📽 영상 재생 완료 → 다음 영상으로");
        PlayVideo(currentPlayingType);
    }

    private void ShowSubtitle(VideoSubtitleData data)
    {
        int langIndex = (int)UIManager.Instance.NowLanguage;
        SubTitle.text = data.SubtitleString[langIndex];
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


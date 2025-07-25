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

    // BackButton을 위한 전 비디오 저장
    private VideoType previousPlayingType;
    private int previousPlayingIndex;

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
        bool fallbackToDefault = false;

        if (!ResourceManager.Instance.VideoMap.TryGetValue(type, out var list) || list.Count == 0)
        {
            Debug.LogWarning($"[VideoPlayManager] 자막 리스트 없음: {type} → Default로 대체");

            if (!ResourceManager.Instance.VideoMap.TryGetValue(VideoType.Default, out list) || list.Count == 0)
            {
                Debug.LogError("[VideoPlayManager] Default 자막 리스트도 없습니다. 재생 불가");
                return;
            }

            type = VideoType.Default;
            fallbackToDefault = true;
        }

        // 🔧 1. 순차 인덱스 계산
        int currentIndex = 0;
        if (!fallbackToDefault && videoPlayIndexMap.TryGetValue(type, out int nextIndex))
        {
            currentIndex = nextIndex;
        }

        // ✅ 2. 이전 재생 정보 저장 (fallback 아닐 때만)
        if (!fallbackToDefault)
        {
            previousPlayingType = currentPlayingType;
            previousPlayingIndex = currentIndex;
        }

        // 🔄 3. 현재 타입 저장 및 인덱스 갱신
        currentPlayingType = type;

        if (!fallbackToDefault)
        {
            videoPlayIndexMap[type] = (currentIndex + 1) % list.Count;
        }

        var selected = list[currentIndex];

        // 다음 인덱스로 갱신 (순차 or 무한 루프)
        if (!fallbackToDefault)
            videoPlayIndexMap[type] = (currentIndex + 1) % list.Count;

        // 3. 비디오 파일 경로 가져오기
        if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out var player))
        {
            Debug.LogWarning($"[VideoPlayManager] 비디오 파일 없음: {selected.fileName} → Default 영상 재생, 자막은 유지");

            // Default 영상으로 대체
            if (!ResourceManager.Instance.VideoMap.TryGetValue(VideoType.Default, out var defaultList) || defaultList.Count == 0)
            {
                Debug.LogError("[VideoPlayManager] Default 자막 리스트도 없습니다. 재생 불가");
                return;
            }

            var defaultVideoData = defaultList[0];
            if (!ResourceManager.Instance.TryGetVideoPlayer(defaultVideoData.fileName, out player))
            {
                Debug.LogError("[VideoPlayManager] Default 비디오 파일도 없습니다. 재생 불가");
                return;
            }

            // ✅ 주의: 자막은 selected 그대로 사용
        }

        _VideoPlayer.Pause();

        // 이벤트 중복 제거 후 등록
        _VideoPlayer.prepareCompleted -= OnVideoPrepared;
        _VideoPlayer.prepareCompleted += OnVideoPrepared;

        _VideoPlayer.loopPointReached -= OnVideoFinished;
        _VideoPlayer.loopPointReached += OnVideoFinished;

        _VideoPlayer.source = VideoSource.Url;
        _VideoPlayer.url = player.url;

        nextSubtitleData = selected;

        _VideoPlayer.Prepare();
    }

    public void PlayPreviousVideo()
    {
        if (!ResourceManager.Instance.VideoMap.TryGetValue(previousPlayingType, out var list) || list.Count == 0)
        {
            Debug.LogWarning("[VideoPlayManager] 이전 자막 리스트 없음");
            return;
        }

        if (previousPlayingIndex < 0 || previousPlayingIndex >= list.Count)
        {
            Debug.LogError($"[VideoPlayManager] 이전 인덱스 범위 초과: {previousPlayingIndex} (list.Count: {list.Count})");
            return;
        }

        var selected = list[previousPlayingIndex];

        // 🎯 영상이 없을 경우 default 영상으로 대체
        if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out var player))
        {
            Debug.LogWarning($"[VideoPlayManager] 이전 비디오 파일 없음: {selected.fileName} → Default 영상으로 대체");


            // Default 영상 가져오기
            if (!ResourceManager.Instance.VideoMap.TryGetValue(VideoType.Default, out var defaultList) || defaultList.Count == 0)
            {
                Debug.LogError("[VideoPlayManager] Default 자막 리스트도 없습니다. 재생 불가");
                return;
            }

            var defaultSelected = defaultList[0];
            if (!ResourceManager.Instance.TryGetVideoPlayer(defaultSelected.fileName, out player))
            {
                Debug.LogError("[VideoPlayManager] Default 비디오 파일도 없습니다. 재생 불가");
                return;
            }
        }

        // ✅ 자막은 항상 이전 selected로 지정
        nextSubtitleData = selected;

        currentPlayingType = previousPlayingType;
        videoPlayIndexMap[previousPlayingType] = (previousPlayingIndex + 1) % list.Count;

        _VideoPlayer.Pause();

        _VideoPlayer.prepareCompleted -= OnVideoPrepared;
        _VideoPlayer.prepareCompleted += OnVideoPrepared;

        _VideoPlayer.loopPointReached -= OnVideoFinished;
        _VideoPlayer.loopPointReached += OnVideoFinished;

        _VideoPlayer.source = VideoSource.Url;
        _VideoPlayer.url = player.url;

        _VideoPlayer.Prepare();
    }

    public void PlayPreviousVideoIfValid()
    {
        if (!ResourceManager.Instance.VideoMap.TryGetValue(previousPlayingType, out var list) || list.Count == 0)
        {
            Debug.Log("[VideoPlayManager] 이전 비디오 없음 → 재생 생략");
            return;
        }

        if (previousPlayingIndex < 0 || previousPlayingIndex >= list.Count)
        {
            Debug.Log("[VideoPlayManager] 이전 인덱스가 잘못됨 → 재생 생략");
            return;
        }

        PlayPreviousVideo();
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


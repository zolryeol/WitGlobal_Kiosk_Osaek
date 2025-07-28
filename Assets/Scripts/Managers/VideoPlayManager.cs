using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class VideoPlayManager : MonoBehaviour
{
    public static VideoPlayManager Instance { get; private set; }
    public VideoPlayer _VideoPlayer; // 
    public VideoPlayer _VideoPlayer2;

    public RenderTexture Display2Texture; // 디스플레이2용 RenderTexture
    public RenderTexture Display2Texture2; // 디스플레이2용 RenderTexture

    public RawImage targetRawImage;

    private VideoPlayer activePlayer;
    private VideoPlayer standbyPlayer;

    private RenderTexture activeTexture;
    private RenderTexture standbyTexture;

    public TextMeshProUGUI SubTitle;
    public TextMeshProUGUI SubTitle2;

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

        activePlayer = _VideoPlayer;
        standbyPlayer = _VideoPlayer2;

        activeTexture = Display2Texture;
        standbyTexture = Display2Texture2;

        _VideoPlayer.targetTexture = Display2Texture;
        _VideoPlayer2.targetTexture = Display2Texture2;

        targetRawImage.texture = Display2Texture;
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

    public void PlayVideoBuffered(string videoUrl, VideoSubtitleData subtitleData = null)
    {
        standbyPlayer.Stop();
        standbyPlayer.source = VideoSource.Url;
        standbyPlayer.url = videoUrl;
        standbyPlayer.audioOutputMode = VideoAudioOutputMode.None;

        standbyPlayer.prepareCompleted -= OnStandbyPrepared;
        standbyPlayer.prepareCompleted += OnStandbyPrepared;

        nextSubtitleData = subtitleData;
        standbyPlayer.Prepare();
    }

    private void OnStandbyPrepared(VideoPlayer vp)
    {
        activePlayer.Stop();

        (activePlayer, standbyPlayer) = (standbyPlayer, activePlayer);
        (activeTexture, standbyTexture) = (standbyTexture, activeTexture);

        targetRawImage.texture = activeTexture;

        // ✅ loopPointReached 재설정
        activePlayer.loopPointReached -= OnVideoFinished;
        activePlayer.loopPointReached += OnVideoFinished;

        activePlayer.Play();

        if (nextSubtitleData != null)
            ShowSubtitle(nextSubtitleData);
    }

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

        int currentIndex = 0;
        if (!fallbackToDefault && videoPlayIndexMap.TryGetValue(type, out int nextIndex))
        {
            currentIndex = nextIndex;
        }

        if (!fallbackToDefault)
        {
            previousPlayingType = currentPlayingType;
            previousPlayingIndex = currentIndex;
        }

        currentPlayingType = type;

        if (!fallbackToDefault)
        {
            videoPlayIndexMap[type] = (currentIndex + 1) % list.Count;
        }

        var selected = list[currentIndex];

        if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out var player))
        {
            Debug.LogWarning($"[VideoPlayManager] 비디오 파일 없음: {selected.fileName} → Default 영상으로 대체");

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

            selected = defaultSelected; // 자막까지 Default로 대체
        }


        string videoUrl = player.url;
        PlayVideoBuffered(videoUrl, selected);
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

        if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out var player))
        {
            Debug.LogWarning($"[VideoPlayManager] 이전 비디오 파일 없음: {selected.fileName} → Default 영상으로 대체");

            if (!ResourceManager.Instance.VideoMap.TryGetValue(VideoType.Default, out var defaultList) || defaultList.Count == 0)
            {
                Debug.LogError("[VideoPlayManager] Default 자막 리스트도 없습니다. 재생 불가");
                return;
            }

            selected = defaultList[0];
            if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out player))
            {
                Debug.LogError("[VideoPlayManager] Default 비디오 파일도 없습니다. 재생 불가");
                return;
            }
        }

        currentPlayingType = previousPlayingType;
        videoPlayIndexMap[previousPlayingType] = (previousPlayingIndex + 1) % list.Count;

        string url = player.url;
        PlayVideoBuffered(url, selected);
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
        SubTitle2.text = data.SubtitleString[langIndex];

    }

    public void ActivateDisplay2()
    {
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();

            if (_VideoPlayer != null && Display2Texture != null &&
                _VideoPlayer2 != null && Display2Texture2 != null)
            {
                _VideoPlayer.targetTexture = Display2Texture;
                _VideoPlayer2.targetTexture = Display2Texture2;

                targetRawImage.texture = activeTexture;

                PlayVideo(VideoType.Default);
            }
            else
            {
                Debug.LogError("둘 중 하나의 VideoPlayer 또는 RenderTexture가 비어 있습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Display 2가 감지되지 않았습니다.");
        }

        targetRawImage.texture = activeTexture;
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


                if (_VideoPlayer != null && Display2Texture != null &&
                    _VideoPlayer2 != null && Display2Texture2 != null)
                {
                    _VideoPlayer.targetTexture = Display2Texture;
                    _VideoPlayer2.targetTexture = Display2Texture2;

                    activePlayer = _VideoPlayer;
                    standbyPlayer = _VideoPlayer2;
                    activeTexture = Display2Texture;
                    standbyTexture = Display2Texture2;

                    targetRawImage.texture = activeTexture;

                    PlayVideo(VideoType.Default);
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


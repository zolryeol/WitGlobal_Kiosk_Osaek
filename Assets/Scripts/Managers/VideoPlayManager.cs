using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayManager : MonoBehaviour
{
    public static VideoPlayManager Instance { get; private set; }

    public VideoPlayer _VideoPlayer;

    [Header("더블 버퍼링용 텍스처")]
    public RenderTexture bufferA;
    public RenderTexture bufferB;
    private bool useBufferA = true;

    public RawImage targetRawImage;
    public TextMeshProUGUI SubTitle;
    public GameObject PackLogo;

    private VideoType currentPlayingType;
    private VideoType previousPlayingType;
    private int previousPlayingIndex;

    private Dictionary<VideoType, int> videoPlayIndexMap = new();
    private Coroutine _retryDisplayCoroutine;
    private VideoSubtitleData nextSubtitleData;

    private readonly HashSet<VideoType> forceFirstOnlyTypes = new() // 0번째 인덱스부터 재생되어야할 비디오
{
    VideoType.Greeting_Stretching,
};

    private readonly HashSet<VideoType> weatherTypes = new()
    {
    VideoType.Weather_Sunny,
    VideoType.Weather_Rain,
    VideoType.Weather_Cold,
    };

    public VideoType CurrentPlayingType => currentPlayingType;

    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _retryDisplayCoroutine = StartCoroutine(TryActivateDisplay2());

        _VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;

        _VideoPlayer.loopPointReached += OnVideoFinished;
        _VideoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private RenderTexture GetNextBuffer()
    {
        useBufferA = !useBufferA;
        return useBufferA ? bufferA : bufferB;
    }

    public void PlayVideo(VideoType type, bool forceReset = false)
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

        bool isForceResetType = forceFirstOnlyTypes.Contains(type);
        int currentIndex = 0;

        if (!fallbackToDefault)
        {
            if (forceReset && isForceResetType)
            {
                videoPlayIndexMap[type] = 0;
            }

            if (videoPlayIndexMap.TryGetValue(type, out int nextIndex))
            {
                currentIndex = nextIndex;
            }
        }

        if (!fallbackToDefault)
        {
            previousPlayingType = currentPlayingType;
            previousPlayingIndex = currentIndex;

            videoPlayIndexMap[type] = (currentIndex + 1) % list.Count;
        }

        currentPlayingType = type;
        var selected = list[currentIndex];

        if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out var player))
        {
            Debug.LogWarning($"[VideoPlayManager] 비디오 파일 없음: {selected.fileName} → Default 영상으로 대체");

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

        nextSubtitleData = selected;

        var nextTexture = GetNextBuffer();
        _VideoPlayer.targetTexture = nextTexture;
        targetRawImage.texture = nextTexture;

        _VideoPlayer.source = VideoSource.Url;
        _VideoPlayer.url = player.url;
        _VideoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        ShowSubtitle(nextSubtitleData);
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (weatherTypes.Contains(currentPlayingType))
        {
            PlayVideo(VideoType.Default); // 날씨 영상이 끝나면 기본 영상으로 돌아감 예외처리
        }
        else
        {
            PlayVideo(currentPlayingType);
        }
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

        currentPlayingType = previousPlayingType;
        videoPlayIndexMap[previousPlayingType] = (previousPlayingIndex + 1) % list.Count;

        var selected = list[previousPlayingIndex];
        if (!ResourceManager.Instance.TryGetVideoPlayer(selected.fileName, out var player))
        {
            Debug.LogError("[VideoPlayManager] 이전 영상도 없습니다.");
            return;
        }

        nextSubtitleData = selected;

        var nextTexture = GetNextBuffer();
        _VideoPlayer.targetTexture = nextTexture;
        targetRawImage.texture = nextTexture;

        _VideoPlayer.source = VideoSource.Url;
        _VideoPlayer.url = player.url;
        _VideoPlayer.Prepare();
    }

    private void ShowSubtitle(VideoSubtitleData data)
    {
        int langIndex = (int)UIManager.Instance.NowLanguage;
        SubTitle.text = data.SubtitleString[langIndex];
    }

    IEnumerator TryActivateDisplay2()
    {
        int maxRetryCount = 10;
        float retryInterval = 1f;
        int currentRetry = 0;

        while (currentRetry < maxRetryCount)
        {
            if (Display.displays.Length > 1)
            {
                Display.displays[1].Activate();

                if (_VideoPlayer != null && bufferA != null && bufferB != null)
                {
                    _VideoPlayer.targetTexture = bufferA;
                    targetRawImage.texture = bufferA;
                    PlayVideo(VideoType.Default);
                }
                else
                {
                    Debug.LogError("[VideoPlayManager] VideoPlayer 또는 RenderTexture가 설정되지 않았습니다.");
                    yield break;
                }

                yield break;
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

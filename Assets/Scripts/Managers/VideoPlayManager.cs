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
    public RenderTexture Display2Texture;
    public RawImage targetRawImage;

    public TextMeshProUGUI SubTitle;
    public GameObject PackLogo;

    private VideoType currentPlayingType;
    private VideoType previousPlayingType;
    private int previousPlayingIndex;

    private Dictionary<VideoType, int> videoPlayIndexMap = new();
    private Coroutine _retryDisplayCoroutine;
    private VideoSubtitleData nextSubtitleData;

    private Texture2D lastFrameTexture;
    private bool _waitingForFirstFrame = false;
    public VideoType CurrentPlayingType => currentPlayingType;

    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _retryDisplayCoroutine = StartCoroutine(TryActivateDisplay2());

        _VideoPlayer.targetTexture = Display2Texture;
        _VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;

        targetRawImage.texture = Display2Texture;

        _VideoPlayer.loopPointReached += OnVideoFinished;
        _VideoPlayer.prepareCompleted += OnVideoPrepared;
        _VideoPlayer.frameReady += OnFirstFrameReady;
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

            selected = defaultSelected;
        }

        nextSubtitleData = selected;

        // 🔄 마지막 프레임 캡처 후 정지화면 유지
        CaptureLastFrame();
        targetRawImage.texture = lastFrameTexture;

        _VideoPlayer.Stop(); // 이제 깜빡임 문제 없음
        _VideoPlayer.source = VideoSource.Url;
        _VideoPlayer.url = player.url;

        _VideoPlayer.frameReady -= OnFirstFrameReady;
        _VideoPlayer.frameReady += OnFirstFrameReady;

        _waitingForFirstFrame = false; // frameReady 안 씀

        _VideoPlayer.Play(); // ✅ Prepare() 없이 바로 Play()
        StartCoroutine(ForceSwapAfterDelay());

        _VideoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        ShowSubtitle(nextSubtitleData);
        Debug.LogWarning($"[Video] 재생 시작됨: {vp.clip?.name}, 프레임 = {vp.frame}, length = {vp.frameCount}");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("📽 영상 재생 완료 → 다음 영상으로");
        PlayVideo(currentPlayingType);
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

        nextSubtitleData = selected;

        // 🔄 깜빡임 방지용 마지막 프레임 유지
        CaptureLastFrame();
        targetRawImage.texture = lastFrameTexture;

        _VideoPlayer.Stop();
        _VideoPlayer.source = VideoSource.Url;
        _VideoPlayer.url = player.url;

        _VideoPlayer.frameReady -= OnFirstFrameReady;
        _VideoPlayer.frameReady += OnFirstFrameReady;

        _waitingForFirstFrame = true;

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

                if (_VideoPlayer != null && Display2Texture != null)
                {
                    _VideoPlayer.targetTexture = Display2Texture;
                    targetRawImage.texture = Display2Texture;
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

    private void CaptureLastFrame()
    {
        var rt = _VideoPlayer.targetTexture;
        if (rt == null) return;

        RenderTexture.active = rt;

        if (lastFrameTexture == null || lastFrameTexture.width != rt.width || lastFrameTexture.height != rt.height)
        {
            lastFrameTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        }

        lastFrameTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        lastFrameTexture.Apply();
        RenderTexture.active = null;
    }

    private void OnFirstFrameReady(VideoPlayer source, long frameIdx)
    {
        if (!_waitingForFirstFrame) return;

        targetRawImage.texture = Display2Texture; // ▶️ 프레임 올 때 연결
        _waitingForFirstFrame = false;
    }

    private IEnumerator ForceSwapAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // 첫 프레임 출력 시간 대기

        targetRawImage.texture = Display2Texture;
        Debug.Log("[Video] 강제 전환: RenderTexture로 스왑 완료");
    }
}

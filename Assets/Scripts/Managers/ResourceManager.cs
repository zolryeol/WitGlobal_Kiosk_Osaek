using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

// 동영상 및 이미지 리소스 관리할 매니저

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public Dictionary<string, List<Sprite>> ShopSpritesDic { get; private set; } = new();
    public Dictionary<int, List<Sprite>> PalaceSpritesDic { get; private set; } = new();

    public Sprite LanguageSelect_Background;
    public Sprite LanguageNormal_Background;
    public Sprite Check_Icon;
    public Sprite NonCheck_Icon;
    public Sprite[] LanNormal_Icon = new Sprite[4];
    public Sprite[] LanSelect_Icon = new Sprite[4];
    public Sprite HanbokSelected_Background;
    public Sprite HanbokNormal_Background;

    public Sprite NoQRImage;

    public Sprite[] PhotoBlockingImage = new Sprite[4];

    //public Dictionary<string, List<Sprite>> HanbokSpritesDic { get; private set; } = new();
    public Dictionary<string, List<(string, Sprite)>> HanbokSpritesDic { get; private set; } = new(); // 폴더명, 파일명,이미지


    [Header("비디오")]
    private Dictionary<VideoType, VideoPlayer> PreloadedPlayers = new();
    private Dictionary<VideoType, string> VideoPaths = new();

    public Dictionary<VideoType, VideoClip> VideoClipDic = new();
    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadShopImages();
        LoadPalaceImages();
        LoadHanbokImages();
#if UNITY_EDITOR
        LoadVideos();
#else
        PreloadLocalVideos();
#endif
    }

    private void LoadHanbokImages()
    {
        HanbokSpritesDic.Clear();

#if UNITY_EDITOR
        string hanbokPath = Path.Combine(Application.dataPath, "Editor", "HanbokForEditor");
#else
        string hanbokPath = Path.Combine(Application.dataPath, "../Data/Hanbok");
#endif

        if (!Directory.Exists(hanbokPath))
        {
            Debug.LogWarning($"Hanbok 폴더가 존재하지 않습니다: {hanbokPath}");
            return;
        }

        string[] subFolders = Directory.GetDirectories(hanbokPath);

        foreach (string folderPath in subFolders)
        {
            string folderName = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(folderName)) continue;

            string[] imageFiles = GetImageFiles(folderPath);
            if (imageFiles.Length == 0)
            {
                Debug.LogWarning($"[ResourceManager] Hanbok/{folderName} 폴더에 이미지가 없습니다.");
                HanbokSpritesDic[folderName] = new List<(string, Sprite)>();
                continue;
            }

            List<(string, Sprite)> spriteList = new();

            foreach (string ipath in imageFiles)
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(ipath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(bytes);

                    // Full Rect로 Sprite 생성
                    Rect fullRect = new Rect(0, 0, tex.width, tex.height);
                    Sprite sprite = Sprite.Create(tex, fullRect, new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
                    sprite.name = Path.GetFileNameWithoutExtension(ipath);

                    spriteList.Add((sprite.name, sprite));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[ResourceManager] Hanbok 이미지 로드 실패: {ipath} / {ex.Message}");
                }
            }

            HanbokSpritesDic[folderName] = spriteList;
            Debug.Log($"[ResourceManager] Hanbok/{folderName} 폴더에서 {spriteList.Count}개 이미지 로드 완료");
        }

        Debug.Log($"[ResourceManager] Hanbok 총 {HanbokSpritesDic.Count}개 카테고리 로드 완료");
    }


    private void LoadShopImages() //  ShopSprite 불러오기
    {
        ShopSpritesDic.Clear();

        string shopImagePath = GetShopImageRootPath();

        if (!Directory.Exists(shopImagePath))
        {
            Debug.LogError($"ShopImage 폴더가 존재하지 않습니다: {shopImagePath}");
            return;
        }

        string[] subFolders = Directory.GetDirectories(shopImagePath);  // target폴더 내 하부파일들 저장

        foreach (string folderPath in subFolders)
        {
            string _folder = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(_folder)) continue;

            string[] imageFiles = GetImageFiles(folderPath);
            if (imageFiles.Length == 0)
            {
                Debug.LogWarning($"[rResourceManager] {_folder} 폴더에 이미지가 없습니다.");
                continue;
            }

            List<Sprite> spriteList = new();

            foreach (string ipath in imageFiles)
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(ipath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(bytes);

                    // 이미지 크기만큼 sprite생성 피봇은 센터
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                    sprite.name = Path.GetFileNameWithoutExtension(ipath);

                    spriteList.Add(sprite);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[ResourceManager] 이미지 로드 실패: {ipath} / {ex.Message}");
                }

            }

            ShopSpritesDic[_folder] = spriteList;
            Debug.Log($"[ResourceManager] {_folder} 폴더에서 {spriteList.Count}개 이미지 로드 완료");
        }

        Debug.Log($"[ResourceManager] 총 {ShopSpritesDic.Count}개 카테고리 로드 완료");
    }

    //private void LoadShopImages() // TXT로 불러오기;
    //{
    //    // ShopImage 폴더 하위의 모든 폴더명 가져오기
    //    TextAsset folderListAsset = Resources.Load<TextAsset>("ShopImageFolders");
    //    if (folderListAsset == null)
    //    {
    //        Debug.LogError("ShopImageFolders.txt를 Resources에 넣어주세요.");
    //        return;
    //    }

    //    string[] folderNames = folderListAsset.text.Split('\n');

    //    foreach (string rawFolderName in folderNames)
    //    {
    //        string folderName = rawFolderName.Trim();
    //        if (string.IsNullOrEmpty(folderName))

    //            continue;

    //        // 각 폴더의 모든 Sprite 로드
    //        Sprite[] sprites = Resources.LoadAll<Sprite>($"ShopImage/{folderName}");
    //        if (sprites.Length == 0)
    //        {
    //            Debug.LogWarning($"ShopImage/{folderName} 폴더에 이미지가 없습니다.");
    //            continue;
    //        }

    //        ShopSpritesDic[folderName] = new List<Sprite>(sprites);
    //    }

    //    Debug.Log($"[ResourceManager] ShopImage 폴더별 이미지 로드 완료: {ShopSpritesDic.Count}개 폴더");
    //}

    private void LoadPalaceImages()
    {
        for (int i = 1; i <= 4; i++)
        {
            // 폴더명은 "1", "2", "3", "4"
            string folderName = i.ToString();
            Sprite[] sprites = Resources.LoadAll<Sprite>($"Image/Palace/{folderName}");
            if (sprites.Length == 0)
            {
                Debug.LogWarning($"Image/Palace/{folderName} 폴더에 이미지가 없습니다.");
                continue;
            }
            PalaceSpritesDic[i] = new List<Sprite>(sprites);
        }
    }

    private void PreloadLocalVideos()
    {
        string videoFolder = Path.Combine(Application.dataPath, "../Data/Video");

        if (!Directory.Exists(videoFolder))
        {
            Debug.LogError($"[ResourceManager] 영상 폴더가 존재하지 않습니다: {videoFolder}");
            return;
        }

        string[] mp4Files = Directory.GetFiles(videoFolder, "*.mp4");

        foreach (string fullPath in mp4Files)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullPath); // ex: "Intro"
            if (!System.Enum.TryParse<VideoType>(fileName, out var videoType))
            {
                Debug.LogWarning($"[ResourceManager] {fileName}는 VideoType enum에 없습니다.");
                continue;
            }

            string fileUrl = "file:///" + fullPath.Replace("\\", "/"); // Windows 경로 대응

            GameObject go = new GameObject($"VideoPlayer_{videoType}");
            go.transform.SetParent(this.transform);
            var vp = go.AddComponent<VideoPlayer>();
            vp.playOnAwake = false;
            vp.source = VideoSource.Url;
            vp.url = fileUrl;
            vp.audioOutputMode = VideoAudioOutputMode.None;
            vp.Prepare();

            PreloadedPlayers[videoType] = vp;
            VideoPaths[videoType] = fileUrl;
        }

        Debug.Log($"[ResourceManager] 영상 {PreloadedPlayers.Count}개 Preload 완료");
    }
    public bool TryGetPreloadedVideoPlayer(VideoType type, out VideoPlayer vp)
    {
        return PreloadedPlayers.TryGetValue(type, out vp);
    }

    public void LoadVideos()
    {
        VideoClipDic.Clear();

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

    private string[] GetImageFiles(string folderPath) // png,jpg,jpeg 가져오기
    {
        return Directory.GetFiles(folderPath).Where(file =>
           file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
           file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
           file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private string GetShopImageRootPath()
    {
#if UNITY_EDITOR
        return Path.Combine(Application.dataPath, "Editor", "ShopImageForEditor");
#else
    // 빌드 시: 실행파일 위치 기준 외부 Data 폴더 사용
    return Path.Combine(Application.dataPath, "../Data/ShopImage");
#endif
    }
}

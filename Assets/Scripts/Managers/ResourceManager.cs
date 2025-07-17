using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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


    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadShopImages();
        LoadPalaceImages();
        LoadHanbokImages();
    }

    private void LoadHanbokImages()
    {
        string hanbokPath = Path.Combine(Application.dataPath, "Resources/Hanbok");

        if (!Directory.Exists(hanbokPath))
        {
            Debug.LogWarning("Hanbok 폴더가 존재하지 않습니다.");
            return;
        }

        string[] subDirs = Directory.GetDirectories(hanbokPath);

        foreach (string dirPath in subDirs)
        {
            string folderName = Path.GetFileName(dirPath); // ex. "Modern", "Traditional"
            string resourcePath = $"Hanbok/{folderName}";

            Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);

            if (sprites.Length <= 0) //  빈폴더 예외처리
            {
                var emptyList = new List<(string, Sprite)>();
                HanbokSpritesDic.Add(folderName, emptyList);
                //Debug.LogWarning($"[{folderName}] 폴더에서 스프라이트를 찾지 못했습니다.");
                continue;
            }

            if (sprites != null && sprites.Length > 0)
            {
                var spriteList = new List<(string, Sprite)>(); // ← 수정된 부분

                foreach (var sprite in sprites)
                {
                    string filename = sprite.name; // .png 없이 파일명
                    spriteList.Add((filename, sprite));
                }

                HanbokSpritesDic[folderName] = spriteList;

                Debug.Log($"[{folderName}] 폴더에서 {sprites.Length}개의 스프라이트를 로드했습니다.");
            }
            else
            {
                Debug.LogWarning($"[{folderName}] 폴더에서 스프라이트를 찾지 못했습니다.");
            }
        }
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

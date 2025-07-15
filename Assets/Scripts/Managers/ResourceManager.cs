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

    public Sprite hanbokSelected_Background;
    public Sprite hanbokNormal_Background;

    public Sprite noQRImage;

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
        string shopImagePath = Path.Combine(Application.dataPath, "Resources/ShopImage");

        if (!Directory.Exists(shopImagePath))
        {
            Debug.LogError("Resources/ShopImage 폴더가 존재하지 않습니다.");
            return;
        }

        string[] subFolders = Directory.GetDirectories(shopImagePath);

        foreach (string folderPath in subFolders)
        {
            string folderName = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(folderName))
                continue;

            Sprite[] sprites = Resources.LoadAll<Sprite>($"ShopImage/{folderName}");
            if (sprites.Length == 0)
            {
                Debug.LogWarning($"ShopImage/{folderName} 폴더에 로드 가능한 스프라이트가 없습니다.");
                continue;
            }

            ShopSpritesDic[folderName] = new List<Sprite>(sprites);
        }

        Debug.Log($"[ResourceManager] ShopImage 폴더별 이미지 로드 완료: {ShopSpritesDic.Count}개 폴더");
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
}

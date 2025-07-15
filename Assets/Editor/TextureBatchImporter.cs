using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureBatchImporter : EditorWindow
{
    [MenuItem("Tools/Convert Images to Sprites")]
    public static void ConvertToSprites()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Select Folder with Images", "Assets", "");

        if (string.IsNullOrEmpty(folderPath))
            return;

        string relativePath = "Assets" + folderPath.Replace(Application.dataPath, "");

        string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

        int count = 0;

        foreach (string file in files)
        {
            if (!file.EndsWith(".png") && !file.EndsWith(".jpg"))
                continue;

            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
                count++;
            }
        }

        Debug.Log($"✅ 총 {count}개의 이미지가 Sprite로 변환되었습니다.");
    }
}

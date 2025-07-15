using UnityEditor;
using UnityEngine;
using System.IO;

public class ShopImageFolderExporter
{
    [MenuItem("Tools/Export ShopImage Folders")]
    public static void ExportFolders()
    {
        string rootPath = Application.dataPath + "/Resources/ShopImage";

        Debug.Log(rootPath);

        if (!Directory.Exists(rootPath))
        {
            Debug.LogError("ShopImage 폴더가 없습니다.");
            return;
        }
        string[] dirs = Directory.GetDirectories(rootPath);

        using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/ShopImageFolders.txt", false))
        {
            foreach (var dir in dirs)
            {
                string folderName = new DirectoryInfo(dir).Name;
                sw.WriteLine(folderName);
            }
        }
        Debug.Log("ShopImage 폴더 목록 저장 완료");
        AssetDatabase.Refresh();
    }
}
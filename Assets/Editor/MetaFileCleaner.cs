using UnityEditor;
using UnityEngine;
using System.IO;

public class MetaFileCleaner : EditorWindow
{
    [MenuItem("Tools/Delete desktop.ini.meta")]
    public static void DeleteDesktopIniMetaFiles()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "desktop.ini.meta", SearchOption.AllDirectories);
        int deleted = 0;

        foreach (string file in files)
        {
            File.Delete(file);
            deleted++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"✅ desktop.ini.meta 파일 {deleted}개 삭제 완료");
    }
}

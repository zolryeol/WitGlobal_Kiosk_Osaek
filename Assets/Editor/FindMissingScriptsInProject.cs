using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

public class FindMissingScriptsInProject
{
    [MenuItem("Tools/Find Missing Scripts In Project")]
    static void FindMissingScriptsInProjectFunc()
    {
        int missingCount = 0;

        // 1. 현재 열려 있는 모든 씬 검사
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                missingCount += ProcessGameObjectHierarchy(root, scene.name);
            }
        }

        // 2. 프로젝트 내 모든 프리팹 검사
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                missingCount += ProcessGameObjectHierarchy(prefab, path);
            }
        }

        Debug.Log($"✅ Missing Script 검사 완료: 총 {missingCount}개 오브젝트에서 스크립트 누락");
    }

    static int ProcessGameObjectHierarchy(GameObject obj, string context)
    {
        int count = 0;
        Component[] components = obj.GetComponents<Component>();

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                Debug.LogWarning($"🛑 Missing Script in '{GetGameObjectPath(obj)}' ({context})", obj);
                count++;
            }
        }

        foreach (Transform child in obj.transform)
        {
            count += ProcessGameObjectHierarchy(child.gameObject, context);
        }

        return count;
    }

    static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
}

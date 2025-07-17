using UnityEditor;
using UnityEngine;

public class MissingScriptFinder
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    static void FindMissingScripts()
    {
        int count = 0;
        GameObject[] objects = Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in objects)
        {
            Component[] components = go.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"Missing script in: {GetHierarchyPath(go)}", go);
                    count++;
                }
            }
        }

        Debug.Log($"🔍 Missing Script 검사 완료 - 총 {count}개 오브젝트에 누락됨");
    }

    static string GetHierarchyPath(GameObject obj)
    {
        return obj.transform.parent == null ? obj.name : GetHierarchyPath(obj.transform.parent.gameObject) + "/" + obj.name;
    }
}

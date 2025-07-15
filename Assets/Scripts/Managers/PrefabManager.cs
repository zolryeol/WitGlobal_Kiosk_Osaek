using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    public GameObject ContentItemInfoPrefab;
    public GameObject SecondCategoryButtonPrefab;
    public GameObject EventInfoPrefab;
    public GameObject EventInfoDetailPrefab;
    public GameObject HanbokCategoryButtonPrefab;
    public GameObject HanbokContentButtonPrefab;
    public Sprite NoImageSprite;

    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

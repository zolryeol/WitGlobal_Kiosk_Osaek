using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private ShopManager shopManger;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PrefabManager prefabManager;
    [SerializeField] private ExchangeRateManager exchangeRateManager;
    [SerializeField] private VideoPlayManager videoPlayManager;

    public static string PhotoPostUrl = "http://158.247.207.5:8000/api/process_image";

    [SerializeField] GameObject Loading;

    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        prefabManager = FindObjectOfType<PrefabManager>();
        shopManger = FindObjectOfType<ShopManager>();
        uiManager = FindObjectOfType<UIManager>();
        exchangeRateManager = FindObjectOfType<ExchangeRateManager>();
        videoPlayManager = FindObjectOfType<VideoPlayManager>();

        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        prefabManager.Init();

        resourceManager.Init();

        videoPlayManager.Init();

        await shopManger.Init(); // 데이터 로드가 끝날 때까지 대기 // 테스트용으로 잠시 꺼둠

        uiManager.Init();

        exchangeRateManager.Init();

        Debug.Log("<color=green>[Core] 모든 매니저 초기화 완료</color>");

        Destroy(Loading);
    }
}

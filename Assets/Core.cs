using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private LoadManager loadManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PrefabManager prefabManager;
    [SerializeField] private ExchangeRateManager exchangeRateManager;
    [SerializeField] private VideoPlayManager videoPlayManager;

    public static string PhotoPostUrl = "http://158.247.207.5:8000/api/process_image";
    public static string PhotoPostUrl_together = "http://158.247.207.5:8000/api/process_and_combine";

    [SerializeField] public static string KioskName = "KIOSK_WITGLOBAL_Osaek";
    //
    //"KIOSK_WITGLOBALHQ"; // 인사
    // 디버그용

    [SerializeField] GameObject Loading;
    [SerializeField] TextMeshProUGUI debugging;
    private string logBuffer = "";
    private const int MaxLines = 30;

    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        prefabManager = FindObjectOfType<PrefabManager>();
        loadManager = FindObjectOfType<LoadManager>();
        uiManager = FindObjectOfType<UIManager>();
        exchangeRateManager = FindObjectOfType<ExchangeRateManager>();
        videoPlayManager = FindObjectOfType<VideoPlayManager>();

#if UNITY_EDITOR
        KioskName = "KIOSK_LEECOM";
#else
        KioskName = "KIOSK_WITGLOBALHQ";
#endif

        DontDestroyOnLoad(gameObject);

        Application.logMessageReceived += HandleLog;
    }
    private async void Start()
    {
        JsonLoader.Init();

        prefabManager.Init();

        resourceManager.Init();

        GoogleSheetReader.InitCredential();

        await loadManager.Init(); // 데이터 로드가 끝날 때까지 대기 // 테스트용으로 잠시 꺼둠

        uiManager.Init();

        exchangeRateManager.Init();

        videoPlayManager.Init();

        Debug.Log("<color=green>[Core] 모든 매니저 초기화 완료</color>");

        Loading.SetActive(false);
        Application.logMessageReceived -= HandleLog;
        debugging.gameObject.SetActive(false);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Warning)
            return;
        if (type == LogType.Log)
            return;

        string color = type switch
        {
            LogType.Error => "#FF5555",
            LogType.Warning => "#FFCC00",
            LogType.Log => "#AAAAAA",
            _ => "#FFFFFF"
        };

        string logLine = $"<color={color}>{logString}</color>\n";
        logBuffer += logLine;

        var lines = logBuffer.Split('\n');
        if (lines.Length > MaxLines)
        {
            logBuffer = string.Join("\n", lines[^MaxLines..]);
        }

        if (debugging != null)
        {
            debugging.text = logBuffer;
        }
    }
}

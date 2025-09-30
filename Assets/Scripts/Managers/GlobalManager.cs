using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;

    public string kioskName;
    public string domain;
    public string upload_api;
    public string search_event_api;
    public string exchange_rate_api;
    public string record_click_api;
    public string insa_api;
    public string post_api;

    public bool barrierFreeFlag = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReadJsonFile()
    {
        // 실행 파일과 동일한 경로에서 config.json 파일 읽기
        string configPath = Path.Combine(Application.streamingAssetsPath, "config.json");

        if (File.Exists(configPath))
        {
            // JSON 파일 읽기
            string jsonContent = File.ReadAllText(configPath);
            KioskConfig config = JsonUtility.FromJson<KioskConfig>(jsonContent);

            // 키오스크 이름 가져오기
            kioskName = config.kioskName;

            domain = config.domain;
            upload_api = config.upload_api;
            search_event_api = config.search_event_api;
            exchange_rate_api = config.exchange_rate_api;
            record_click_api = config.record_click_api;
            insa_api = config.insa_api;
            post_api = config.post_api;


            //KioskLogger.Error(
            //    $"JSon File Read " +
            //    $"\nKioskName : {kioskName}" +
            //    $"\ndomain : {domain}" +
            //    $"\nupload_api : {upload_api}" +
            //    $"\nsearch_event : {search_event_api}" +
            //    $"\nexchange_rate_api : {exchange_rate_api}" +
            //    $"\nrecord_click : {record_click_api}" +
            //    $"\ninsa_api : {insa_api}" +
            //    $"\npost_api : {post_api}"
            //    );   
        }
        else
        {
            KioskLogger.Error($"Config file not found at: {configPath}");
        }

    }

    [System.Serializable]
    public class KioskConfig
    {
        public string kioskName;
        public string domain;
        public string upload_api;
        public string search_event_api;
        public string exchange_rate_api;
        public string record_click_api;
        public string insa_api;
        public string post_api;
    }
}

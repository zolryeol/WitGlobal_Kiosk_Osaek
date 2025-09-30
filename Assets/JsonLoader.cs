using System.IO;
using System.Text;
using UnityEngine;

public class JsonLoader : MonoBehaviour
{
    // 파일명과 확장자까지 명확히!
    private const string FileName = "config.json";
    public static DeviceConfig Config;

    public static void Init()
    {
        var fullPath = GetConfigFilePath();
        Debug.Log($"[JsonLoader] 시도 경로 = {fullPath}");

        if (!File.Exists(fullPath))
        {
            KioskLogger.Error($"[JsonLoader] 파일이 존재하지 않습니다: {fullPath}");
            return;
        }

        try
        {
            // UTF-8 BOM 포함 파일도 안전하게 읽기
            string json = File.ReadAllText(fullPath, Encoding.UTF8);

            // 내용 확인 로그(필요시)
            // Debug.Log($"[JsonLoader] 원본 JSON = {json}");

            Config = JsonUtility.FromJson<DeviceConfig>(json);
            if (Config == null)
            {
                KioskLogger.Error("[JsonLoader] JSON 파싱 실패: 루트가 객체형({ ... })인지 확인");
                return;
            }

            Debug.Log($"포트: {Config.PortName}, 속도: {Config.BaudRate}");
        }
        catch (System.Exception ex)
        {
            KioskLogger.Error($"[JsonLoader] 읽기/파싱 중 예외: {ex.GetType().Name} - {ex.Message}");
        }
    }

    static string GetConfigFilePath()
    {
#if UNITY_EDITOR
        // 예: D:/Data/Config_Editor/config.json
        string dir = "D:/Data";
        return Path.Combine(dir, FileName);
#else
       string dir = Path.Combine(Application.dataPath, "../Data");
        return Path.Combine(dir, FileName);                      
#endif
    }
}

[System.Serializable]
public class DeviceConfig
{
    public string PortName;
    public int BaudRate;

    public string Province;
    public string District;
    public string MarketName;
}
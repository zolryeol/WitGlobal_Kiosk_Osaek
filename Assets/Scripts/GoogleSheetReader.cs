using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class GoogleSheetReader
{
    static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static readonly string ApplicationName = "WitglobalKiosk";
    static readonly string DataSheet_Insa = "1AVZoyepjrlWIUtwXamGRYU6TWkKKKgVb7fzwEtbRDaw";

    static readonly string AICategoryRange = "AICategory_Insa!A3:E";
    static readonly string ShopDataRange = "ShopData_Insa!A1:AH";
    static readonly string PalaceInfoDataRange = "PalaceInfo_Insa!A3:AD";
    static readonly string EventInfoDataRange = "Event_Insa!A3:AG";
    static readonly string VideoSubTitleRange = "VideoSubtitle_Insa!A3:G";
    static readonly string LocalizationDataRange = "Localization_Insa!A1:F";

    static readonly string fileName = "kiosk-insadatasheet-52e5347a7b9c.json";

    // ✅ 캐시된 객체
    private static GoogleCredential _credential;
    private static SheetsService _service;

    public static async Task<ValueRange> ReadShopDataSheetAsync()
    {
        return await ReadSheetAsync(ShopDataRange);
    }

    public static async Task<ValueRange> ReadAICategoryAsync()
    {
        return await ReadSheetAsync(AICategoryRange);
    }

    public static async Task<ValueRange> ReadLocalizationDataRange()
    {
        return await ReadSheetAsync(LocalizationDataRange);
    }

    public static async Task<ValueRange> ReadPalaceInfoDataRange()
    {
        return await ReadSheetAsync(PalaceInfoDataRange);
    }

    public static async Task<ValueRange> ReadEventInfoDataRange()
    {
        return await ReadSheetAsync(EventInfoDataRange);
    }

    public static async Task<ValueRange> ReadVideoSubTitleDataRange()
    {
        return await ReadSheetAsync(VideoSubTitleRange);
    }

    // ✅ 공통 처리 함수
    private static async Task<ValueRange> ReadSheetAsync(string range)
    {
        var credential = GetCredential();
        var service = GetService(credential);

        if (service == null)
        {
            Debug.LogError("[GoogleSheetReader] SheetsService is null.");
            return null;
        }

        try
        {
            var request = service.Spreadsheets.Values.Get(DataSheet_Insa, range);
            var response = await request.ExecuteAsync();
            return response;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Google Sheets API request failed: {ex.Message}");
            return null;
        }
    }

    private static GoogleCredential GetCredential()
    {
        if (_credential != null)
            return _credential;

        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        try
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                _credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                return _credential;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Credential load failed: {ex.Message}");
            return null;
        }
    }

    private static SheetsService GetService(GoogleCredential credential)
    {
        if (_service != null)
            return _service;

        try
        {
            _service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return _service;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] SheetsService creation failed: {ex.Message}");
            return null;
        }
    }
}

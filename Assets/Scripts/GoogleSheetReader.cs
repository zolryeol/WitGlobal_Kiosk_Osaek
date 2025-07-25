using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public static class GoogleSheetReader
{
    static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static readonly string ApplicationName = "WitglobalKiosk";
    static readonly string DataSheet_Insa = "1AVZoyepjrlWIUtwXamGRYU6TWkKKKgVb7fzwEtbRDaw";

    static readonly string AICategoryRange = "AICategory_Insa!A3:E";
    static readonly string ShopDataRange = "ShopData_Insa!A1:AH"; // 가게 정보
    static readonly string PalaceInfoDataRange = "PalaceInfo_Insa!A3:AD"; // 고궁정보
    static readonly string EventInfoDataRange = "Event_Insa!A3:AG"; // 이벤트 정보
    static readonly string VideoSubTitleRange = "VideoSubtitle_Insa!A3:G"; // 동영상 자막 정보

    static readonly string LocalizationDataRange = "Localization_Insa!A1:F";

    static readonly string fileName = "kiosk-insadatasheet-52e5347a7b9c.json";


    public static async Task<ValueRange> ReadShopDataSheetAsync() // shopdata 시트
    {
        GoogleCredential credential = GetCredential(); ; // 인증 정보 로드(자격증명) 
        SheetsService service = GetService(credential);

        var request = service.Spreadsheets.Values.Get(DataSheet_Insa, ShopDataRange);

        try
        {
            var sheet = await request.ExecuteAsync();
            Debug.Log($"Row 0: {string.Join(", ", sheet.Values[0])}");
            return sheet;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Google Sheets API request failed: {ex.Message}");
            return null;
        }
    }
    public static async Task<ValueRange> ReadAICategoryAsync() // AICategory 시트
    {
        GoogleCredential credential = GetCredential(); ; // 인증 정보 로드(자격증명) 
        SheetsService service = GetService(credential);

        var request = service.Spreadsheets.Values.Get(DataSheet_Insa, AICategoryRange);
        try
        {
            var sheet = await request.ExecuteAsync();
            return sheet;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Google Sheets API request failed: {ex.Message}");
            return null;
        }
    }

    public static async Task<ValueRange> ReadLocalizationDataRange()
    {
        GoogleCredential credential = GetCredential(); ; // 인증 정보 로드(자격증명) 
        SheetsService service = GetService(credential);

        var request = service.Spreadsheets.Values.Get(DataSheet_Insa, LocalizationDataRange);

        try
        {
            var sheet = await request.ExecuteAsync();
            return sheet;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Google Sheets API request failed: {ex.Message}");
            return null;
        }
    }
    public static async Task<ValueRange> ReadPalaceInfoDataRange()
    {
        GoogleCredential credential = GetCredential(); ; // 인증 정보 로드(자격증명) 
        SheetsService service = GetService(credential);

        var request = service.Spreadsheets.Values.Get(DataSheet_Insa, PalaceInfoDataRange);
        try
        {
            var sheet = await request.ExecuteAsync();
            return sheet;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Google Sheets API request failed: {ex.Message}");
            return null;
        }
    }

    public static async Task<ValueRange> ReadVideoSubTitleDataRange()
    {
        GoogleCredential credential = GetCredential(); ; // 인증 정보 로드(자격증명) 
        SheetsService service = GetService(credential);
        var request = service.Spreadsheets.Values.Get(DataSheet_Insa, VideoSubTitleRange);
        try
        {
            var sheet = await request.ExecuteAsync();
            return sheet;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Google Sheets API request failed: {ex.Message}");
            return null;
        }
    }

    public static async Task<ValueRange> ReadEventInfoDataRange()
    {
        GoogleCredential credential = GetCredential(); ; // 인증 정보 로드(자격증명) 
        SheetsService service = GetService(credential);

        var request = service.Spreadsheets.Values.Get(DataSheet_Insa, EventInfoDataRange);
        try
        {
            var sheet = await request.ExecuteAsync();
            return sheet;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] Google Sheets API request failed: {ex.Message}");
            return null;
        }
    }
    private static GoogleCredential GetCredential()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        GoogleCredential credential; // 인증 정보 로드(자격증명)
        try
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
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
        SheetsService service;
        try
        {
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GoogleSheetReader] SheetsService creation failed: {ex.Message}");
            return null;
        }
    }


}


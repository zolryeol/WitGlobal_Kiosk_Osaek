using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TL3800DemoUI : MonoBehaviour
{
    [Header("Refs")]
    public TL3800Driver driver;
    public TMP_InputField comInput;
    public TMP_InputField baudInput;
    public Button connectBtn;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI logText;

    [Header("Web input fallback")]
    public TMP_InputField barcodeInputFallback; // 3D WebView가 없을 때 바코드를 여기로 씁니다.

    [Header("Optional WebView(Vuplex)")]
    public bool useVuplexWebView = false;
#if VUPLEX_3DWEBVIEW
    public Vuplex.WebView.WebViewPrefab webView;
#endif

    CancellationTokenSource _opCts;

    [SerializeField] float minScanIntervalSec = 0.8f; // 연속 이벤트 노이즈 방지

    bool _scanArmed = true;         // 스캔 허용 상태(장전)
    double _lastScanAt = -9999;     // 마지막 이벤트 시각

    void Awake()
    {
        connectBtn.onClick.AddListener(OnConnectToggle);

        driver.OnConnected += () => Append($"[SYS] Connected");
        driver.OnDisconnected += () => Append($"[SYS] Disconnected");
        driver.OnLog += Append;

        // 응답 프레임 수신
        driver.OnResponse += OnResponse;

        // @ 이벤트 (카드/QR 등)
        driver.OnEventAt += (code, raw) =>
        {
            if (code == 'Q')
            {
                // 디바운스: 너무 빠른 중복 이벤트 무시
                if (Time.timeAsDouble - _lastScanAt < minScanIntervalSec)
                {
                    Append("@:Q (무시: 디바운스)");
                    return;
                }
                _lastScanAt = Time.timeAsDouble;

                if (!_scanArmed)
                {
                    Append("@:Q (무시: 스캔 미장전 상태)");
                    return;
                }

                // 한 번 처리 후 장전 해제
                _scanArmed = false;
                Append("@:Q 바코드 인식 → W 조회");
                _ = CallW();  // 최근 바코드 조회 → WebInput에 주입됨
                return;
            }

            // 나머지 이벤트 로그
            switch (code)
            {
                case 'I': Append("@:I IC 카드 인식"); break;
                case 'R': Append("@:R RF 카드 인식"); break;
                case 'M': Append("@:M MS 카드 인식"); break;
                case 'O': Append("@:O IC 카드 제거"); break;
                case 'F': Append("@:F Fallback(불량 IC)"); break;
                default: Append($"@: {code} ({raw})"); break;
            }
        };
    }

    void OnDestroy() { _opCts?.Cancel(); _opCts?.Dispose(); }

    void Append(string s)
    {
        if (logText) logText.text = (s + "\n") + logText.text;
        if (statusText) statusText.text = s;
        Debug.Log(s);
    }

    public void OnConnectToggle()
    {
        try
        {
            if (!driver.IsOpen)
            {
                string com = string.IsNullOrWhiteSpace(comInput?.text) ? driver.portName : comInput.text.Trim();
                int baud = int.TryParse(baudInput?.text, out var b) ? b : driver.baudRate;
                driver.Open(com, baud);
            }
            else
            {
                driver.Close();
            }
        }
        catch (Exception ex) { Append("[ERR] " + ex.Message); }
    }

    // ====== UI hooks ======
    public async void Click_DeviceCheck() => await SafeCall(() => driver.A_DeviceCheck(CT()));
    public async void Click_Version() => await SafeCall(() => driver.V_Version(CT()));
    public async void Click_KeyRenew() => await SafeCall(() => driver.K_KeyRenew(CT()));
    public async void Click_Reset() => await SafeCall(() => driver.R_Reset(CT()));
    public async void Click_LastApproval() => await SafeCall(() => driver.L_LastApproval(CT()));
    public async void Click_QueryW() => await CallW();

    public async void Click_Approve_1000() => await Approve(1000);
    public async void Click_Approve_5000() => await Approve(5000);
    public async void Click_Approve_12000() => await Approve(12000);
    public async void Click_Cancel_Last() => await SafeCall(() => driver.C_Cancel_Last(CT()));

    async Task Approve(long amount)
    {
        await SafeCall(() => driver.B_approveWrapper(amount, CT())); // 아래 확장 호출
    }

    async Task CallW()
    {
        await SafeCall(() => driver.W_Query(CT()));
    }

    CancellationToken CT()
    {
        _opCts?.Cancel(); _opCts?.Dispose();
        _opCts = new CancellationTokenSource();
        return _opCts.Token;
    }

    async Task SafeCall(Func<Task<TL3800Driver.Frame?>> api)
    {
        try
        {
            statusText.text = "Processing...";
            var frame = await api();
            if (frame == null) { Append("[TIMEOUT] 응답 없음"); return; }
            Append($"[OK] job={frame.Value.Job} len={frame.Value.DataLength}");
        }
        catch (Exception ex)
        {
            Append("[ERR] " + ex.Message + "\n→ 필요시 마지막 승인(L)로 이중승인 여부 확인 권장"); // :contentReference[oaicite:24]{index=24}
        }
    }

    // TL3800DemoUI.cs에 추가

    /// <summary>
    /// 사용자 취소 → 즉시 IDLE 복귀 (장치체크 A 경로)
    /// </summary>
    public async void Click_AbortAndIdle_A()
    {
        try
        {
            // 우리 쪽 대기 중인 요청이 있다면 취소
            _opCts?.Cancel();

            statusText.text = "Cancelling... (IDLE 복귀)";
            bool ok = await driver.RecoverToIdleWithA(CT());
            Append(ok ? "[IDLE] A 경로로 상태 동기화 완료" : "[IDLE] A 응답 없음");
        }
        catch (Exception ex)
        {
            Append("[ERR] " + ex.Message);
        }
    }

    /// <summary>
    /// 사용자 취소 → 즉시 IDLE 복귀 (마지막 승인 확인 L 경로; 이중승인 우려시)
    /// </summary>
    public async void Click_AbortAndIdle_L()
    {
        try
        {
            _opCts?.Cancel();

            statusText.text = "Cancelling... (마지막 승인 확인)";
            bool ok = await driver.RecoverToIdleWithL(CT());
            Append(ok ? "[IDLE] L 경로로 상태 동기화 완료" : "[IDLE] L 응답 없음");
        }
        catch (Exception ex)
        {
            Append("[ERR] " + ex.Message);
        }
    }

    // 응답 해석(간단)
    void OnResponse(TL3800Driver.Frame f)
    {
        switch (f.Job)
        {
            case 'a': // 장치체크 응답(4바이트)  :contentReference[oaicite:25]{index=25}
                if (f.DataLength >= 4)
                {
                    var s = Encoding.ASCII.GetString(f.Data, 0, 4);
                    Append($"[a] 카드모듈={s[0]}, RF={s[1]}, VAN={s[2]}, LINK={s[3]}");
                }
                break;

            case 'b': // 거래승인 응답(요약 표시)  :contentReference[oaicite:26]{index=26}
            case 'g': // 확장응답
                ShowApprovalBrief(f);
                break;

            case 'w': // 바코드/마지막 내역 조회 결과(원문 그대로 표시)
                {
                    string txt = TryAscii(f.Data);
                    Append($"[w] {txt}");
                    // WebView or Fallback 인젝션
                    TypeToWebOrInput(txt);
                }
                break;

            default:
                Append($"[RESP {f.Job}] {TryAscii(f.Data)}");
                break;
        }
    }

    void ShowApprovalBrief(TL3800Driver.Frame f)
    {
        // 안전하게 ASCII 부분만 간이 파싱
        string all = TryAscii(f.Data);
        Append($"[{f.Job}] 응답 일부: {all}");
        // 실제 상용에서는 문서의 각 필드(거래구분/매체/카드번호/승인금액/승인번호/매출일시/단말기번호 등)를
        // 고정 길이로 정확히 substring 하여 저장/영수증 출력/정산 DB 적재하세요.  :contentReference[oaicite:27]{index=27}
    }

    string TryAscii(byte[] d)
    {
        try { return Encoding.ASCII.GetString(d).Trim('\0'); }
        catch { return BitConverter.ToString(d); }
    }

    void TypeToWebOrInput(string text)
    {
#if VUPLEX_3DWEBVIEW
        if (useVuplexWebView && webView != null && webView.WebView != null)
        {
            // 포커스된 input에 값 세팅
            // 간단한 방법: 클립보드 붙여넣기 시뮬레이션 대신 JS로 value 설정
            string js = $"(function(){{var el=document.activeElement;if(el&&('value' in el)){{el.value={EscapeJs(text)}; if(el.dispatchEvent) el.dispatchEvent(new Event('input'));}}}})();";
            webView.WebView.ExecuteJavaScript(js);
            Append("[WEB] 입력필드에 바코드 주입");
            return;
        }
#endif
        if (barcodeInputFallback != null)
        {
            barcodeInputFallback.text = text;
            barcodeInputFallback.MoveTextEnd(false);
            Append("[TMP] 바코드 텍스트 입력");
        }
    }

#if VUPLEX_3DWEBVIEW
    string EscapeJs(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
#endif
    public void Click_ArmNextScan()
    {
        _scanArmed = true;
        _lastScanAt = Time.timeAsDouble;
        if (barcodeInputFallback) barcodeInputFallback.text = string.Empty;
        Append("[SCAN] 다시 스캔 대기 상태로 전환");
    }


}

// 간단 확장: 승인 버튼에서 호출하기 편하게
static class DriverExt
{
    public static Task<TL3800Driver.Frame?> B_approveWrapper(this TL3800Driver d, long amount, CancellationToken ct)
        => d.B_Approve(amount, 0, 0, 0, true, ct);
}

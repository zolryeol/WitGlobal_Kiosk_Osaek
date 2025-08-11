using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using System.Collections;

public class TL3800 : MonoBehaviour
{
    [Header("Serial")]
    public string portName = "COM8";
    public int baudRate = 115200;       // 현장값 확인
    public int readTimeoutMs = 100;

    [Header("Polling (옵션)")]
    public bool enablePolling = false;   // true면 스캔 대기 중 주기적으로 W 전송
    public float pollIntervalSec = 0.2f; // 150~300ms 권장
    public float pollTimeoutSec = 10f;

    public Action<string> OnBarcode;     // 바코드 수신 콜백(원문 문자열)

    SerialPort _port;
    Thread _rxThread;
    volatile bool _running;
    ConcurrentQueue<Action> _toMain = new();
    Coroutine _pollCo;
    string _lastBarcode;                 // 디바운스용

    void Start()
    {
        OpenPort();

        // 장치 상태 한 번 확인하고 시작하고 싶으면 주석 해제
        //Send(Build('A', Array.Empty<byte>()));
        //Send(Build('W', Array.Empty<byte>()));


        if (enablePolling) StartScanPolling();
    }

    void Update()
    {
        while (_toMain.TryDequeue(out var a)) a?.Invoke();
    }

    void OnDestroy()
    {
        StopScanPolling();
        ClosePort();
    }

    #region 포트 열고 닫기
    void OpenPort()
    {
        try
        {
            _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = readTimeoutMs,
                WriteTimeout = 1000
            };
            _port.Open();

            _running = true;
            _rxThread = new Thread(ReadLoop) { IsBackground = true };
            _rxThread.Start();

            Debug.Log($"[TL3800] Open {_port.PortName} @ {baudRate}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[TL3800] 포트 열기 실패: {ex.Message}");
        }
    }

    void ClosePort()
    {
        try
        {
            _running = false;
            try { _rxThread?.Join(300); } catch { }
            if (_port?.IsOpen == true) _port.Close();
            Debug.Log("[TL3800] Port closed");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[TL3800] 포트 닫기 중 오류: {ex.Message}");
        }
    }
    #endregion

    #region 수신 루프 / 파서
    void ReadLoop()
    {
        var buf = new byte[8192];
        int len = 0;

        while (_running)
        {
            try
            {
                int n = _port.Read(buf, len, buf.Length - len);
                if (n > 0)
                {
                    len += n;
                    TryParse(buf, ref len);
                }
            }
            catch (TimeoutException) { }
            catch (Exception ex)
            {
                Debug.LogError($"[TL3800] Read error: {ex.Message}");
                Thread.Sleep(200);
            }
        }
    }

    void TryParse(byte[] src, ref int len)
    {
        int i = 0;
        while (i < len)
        {
            // STX(0x02) 찾기
            while (i < len && src[i] != 0x02) i++;
            if (i >= len) break;

            // ETX(0x03) 위치 찾기
            int etx = -1;
            for (int j = i + 1; j < len; j++)
                if (src[j] == 0x03) { etx = j; break; }
            if (etx == -1 || etx + 1 >= len) break; // ETX 또는 BCC 미수신

            // BCC(STX~ETX XOR) 확인
            byte calc = 0;
            for (int k = i; k <= etx; k++) calc ^= src[k];
            byte bcc = src[etx + 1];
            if (calc != bcc)
            {
                // 프레이밍 재동기화
                i = etx + 2;
                continue;
            }

            // 프레임 추출 (STX..ETX + BCC)
            int frameLen = (etx + 2) - i;
            var f = new byte[frameLen];
            Buffer.BlockCopy(src, i, f, 0, frameLen);

            HandleFrame(f); // 헤더/데이터 파싱

            i = etx + 2; // 다음 탐색
        }

        // 남은 바이트 압축
        if (i > 0)
        {
            int remain = len - i;
            Buffer.BlockCopy(src, i, src, 0, remain);
            len = remain;
        }
    }

    void HandleFrame(byte[] frame)
    {
        // 헤더 35B: [0]=STX, [1..16]=CAT/MID, [17..30]=YYYYMMDDhhmmss(14), [31]=Job, [32]=Resp(0), [33..34]=DataLen(LE)
        if (frame.Length < 35 + 2) return; // 헤더 + ETX/BCC 최소 보장

        char job = (char)frame[31];
        ushort dataLen = BitConverter.ToUInt16(frame, 33);
        int dataStart = 35;

        // 프레임 내부 기준 ETX 위치
        int etxLocal = frame.Length - 2;           // 마지막 2바이트는 ETX,BCC
        int dataCount = etxLocal - dataStart;      // ETX 전까지가 데이터

        if (job == '@')
        {
            // 이벤트: Data[0] = 'M','R','I','O','F','Q' (바코드는 'Q')
            if (dataCount >= 1)
            {
                char ev = (char)frame[dataStart];
                if (ev == 'Q')
                {
                    _toMain.Enqueue(() =>
                    {
                        Debug.Log("[TL3800] 이벤트 @Q 수신");
                        if (!enablePolling) SendW(); // 이벤트 기반일 때만 그 즉시 조회
                    });
                }
            }
            // 이벤트는 ACK/NACK 없음
            return;
        }

        switch (job)
        {
            case 'a': // 장치 체크 응답
                _toMain.Enqueue(() => Debug.Log("[TL3800] [a] 장치상태 응답 수신"));
                SendAck();
                break;

            case 'w': // 바코드 원문: size(3문자) + data
                if (dataCount >= 3)
                {
                    string szStr = System.Text.Encoding.ASCII.GetString(frame, dataStart, 3);
                    int sz = int.TryParse(szStr, out var tmp) ? tmp : 0;
                    string code = (sz > 0 && dataCount >= 3 + sz)
                        ? System.Text.Encoding.ASCII.GetString(frame, dataStart + 3, sz)
                        : "";
                    if (!string.IsNullOrEmpty(code) && code != _lastBarcode)
                    {
                        _lastBarcode = code;
                        _toMain.Enqueue(() =>
                        {
                            Debug.Log($"[TL3800] [w] 바코드: {code}");
                            OnBarcode?.Invoke(code);
                            // 폴링 모드에서 첫 수신 시 스캔 종료
                            if (enablePolling) StopScanPolling();
                        });
                    }
                }
                SendAck();
                break;

            // 결제 확장 시: b/q/z 등 추가 케이스 작성
            default:
                SendAck(); // 정상 프레임 수신 시 ACK
                break;
        }
    }
    #endregion

    #region 송신/패킷 빌드/ACK
    public void SendW() => Send(Build('W', Array.Empty<byte>()));
    public void SendA() => Send(Build('A', Array.Empty<byte>()));

    void SendAck() { try { _port?.Write(new byte[] { 0x06 }, 0, 1); } catch { } }
    void SendNack() { try { _port?.Write(new byte[] { 0x15 }, 0, 1); } catch { } }

    void Send(byte[] frame)
    {
        try { if (_port?.IsOpen == true) _port.Write(frame, 0, frame.Length); }
        catch (Exception ex) { Debug.LogWarning($"[TL3800] Send error: {ex.Message}"); }
    }

    byte[] Build(char jobCode, byte[] data)
    {
        byte[] tmp = new byte[35 + data.Length + 2]; // 헤더35 + Data + ETX + (BCC 나중)
        int o = 0;
        tmp[o++] = 0x02; // STX

        // CAT/MID 16: 좌측정렬, 나머지 0x00
        o += PutAscii(tmp, o, GetCatOrMid(), 16);
        // DateTime 14: YYYYMMDDhhmmss
        o += PutAscii(tmp, o, DateTime.Now.ToString("yyyyMMddHHmmss"), 14);

        tmp[o++] = (byte)jobCode; // Job
        tmp[o++] = 0x00;          // Resp(미사용)

        // DataLen(LE)
        var dl = BitConverter.GetBytes((ushort)data.Length);
        tmp[o++] = dl[0]; tmp[o++] = dl[1];

        // Data
        Buffer.BlockCopy(data, 0, tmp, o, data.Length);
        o += data.Length;

        // ETX
        tmp[o++] = 0x03;

        // BCC(STX~ETX XOR)
        byte bcc = 0;
        for (int k = 0; k < o; k++) bcc ^= tmp[k];

        // 최종 프레임
        var outF = new byte[o + 1];
        Buffer.BlockCopy(tmp, 0, outF, 0, o);
        outF[o] = bcc;
        return outF;
    }

    int PutAscii(byte[] dst, int ofs, string s, int width)
    {
        var a = System.Text.Encoding.ASCII.GetBytes(s ?? "");
        int n = Mathf.Min(a.Length, width);
        Buffer.BlockCopy(a, 0, dst, ofs, n);
        for (int k = n; k < width; k++) dst[ofs + k] = 0x00;
        return width;
    }

    string GetCatOrMid() => "TESTCATID01"; // 업체 제공 값으로 교체
    #endregion

    #region 폴링 스캔
    public void StartScanPolling()
    {
        StopScanPolling();
        _lastBarcode = null;
        _pollCo = StartCoroutine(CoScanPolling());
    }

    public void StopScanPolling()
    {
        if (_pollCo != null) 
        {
            Debug.Log("[TL3800] 스캔 대기 중지");
            StopCoroutine(_pollCo); _pollCo = null; 
        }
    }

    IEnumerator CoScanPolling()
    {
        Debug.Log("[TL3800] 스캔 대기 시작(폴링)");
        float t = 0f;
        while (t < pollTimeoutSec)
        {
            SendW();
            yield return new WaitForSeconds(pollIntervalSec);
            t += pollIntervalSec;
            if (!string.IsNullOrEmpty(_lastBarcode)) break; // 첫 수신 시 종료
        }
        if (string.IsNullOrEmpty(_lastBarcode))
            Debug.LogWarning("[TL3800] 스캔 타임아웃");
        _pollCo = null;
    }
    #endregion
}

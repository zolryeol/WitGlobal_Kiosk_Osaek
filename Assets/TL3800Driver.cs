using System;
using System.Collections.Concurrent;
using System.Diagnostics; // ★ 추가
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// TL-3800 직결 시리얼 드라이버 + 프레임 빌더/파서 + ACK/NACK/타임아웃/재전송 + 이벤트(@) + 고수준 API(A,B,C2,V,W,L,K,R)
/// </summary>
public class TL3800Driver : MonoBehaviour
{
    // ====== Inspector ======
    [Header("Serial")]
    public string portName = "COM8";
    public int baudRate = 115200;
    public int readTimeoutMs = 200;         // non-blocking 폴링
    public int writeTimeoutMs = 1000;

    [Header("Terminal IDs")]
    [Tooltip("CAT ID 또는 MID (좌측정렬, 나머지 0x00). 테스트용 ID는 업체에서 수령 필요.")]
    public string terminalId = "";          // 16바이트 고정(좌측정렬, 나머지 0x00)
    [Tooltip("Header의 DateTime은 기본 YYYYMMDDhhmmss. 특수 케이스는 문서 주석 참고.")]
    public bool overrideHeaderDateTime = false;
    public string fixedHeaderDateTime14 = "20250101000000";

    [Header("Retry / Timeout")]
    public int ackWaitMs = 3000;            // ACK 대기 3초
    public int ackRetryMax = 3;             // ACK 재전송 3회
    public int respWaitMaxMs = 25000;       // 응답전문 최대 25초 대기 (권장) 

    // ====== Events ======
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<byte[]> OnAck;              // 0x06
    public event Action<byte[]> OnNack;             // 0x15
    public event Action<Frame> OnResponse;          // 정식 응답전문(헤더+바디+ETX+BCC)
    public event Action<char, string> OnEventAt;    // '@' 이벤트: (이벤트코드, 원시 데이터 문자열)
    public event Action<string> OnLog;              // 로그 출력용

    // ====== Internals ======
    SerialPort _sp;
    Thread _rxThread;
    volatile bool _run;
    readonly ConcurrentQueue<Action> _toMain = new();

    // Abort 제어(스레드 안전)
    volatile bool _abortDrainOnce;               // 다음에 오는 응답 1건은 자동 ACK 후 버리기
    static readonly Stopwatch _wall = Stopwatch.StartNew(); // ★ 프로세스 경과시간
    long _abortExpireUntilMs;                    // ★ 만료시각(ms)

    // --------------- Unity lifecycle ---------------
    void OnDisable() => Close();

    // --------------- Public API ---------------
    public bool IsOpen => _sp != null && _sp.IsOpen;

    public void Open(string port, int baud)
    {
        Close();
        portName = port;
        baudRate = baud;

        _sp = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        _sp.ReadTimeout = readTimeoutMs;
        _sp.WriteTimeout = writeTimeoutMs;
        _sp.Open();

        _run = true;
        _rxThread = new Thread(RxLoop) { IsBackground = true };
        _rxThread.Start();

        Post(() => OnConnected?.Invoke());
        Log($"[TL3800] Open {portName} @ {baudRate}");
    }

    public void Close()
    {
        _run = false;
        try { _rxThread?.Join(200); } catch { }
        if (_sp != null)
        {
            try { if (_sp.IsOpen) _sp.Close(); } catch { }
            _sp.Dispose();
            _sp = null;
        }
        Post(() => OnDisconnected?.Invoke());
    }

    /// <summary>
    /// 사용자 취소 등: 우리 쪽 대기를 끊고, 뒤늦게 들어올 수 있는 응답 1건은
    /// 드라이버가 ACK만 보내고 폐기하도록 설정.
    /// </summary>
    public void AbortWaitingGracefully(float validSeconds = 10f)
    {
        _abortDrainOnce = true;
        _abortExpireUntilMs = _wall.ElapsedMilliseconds + (long)(validSeconds * 1000); // ★
        Log("[ABORT] 앱 대기 취소 → 다음 응답 1건은 자동 ACK 후 폐기");
    }

    void Update()
    {
        while (_toMain.TryDequeue(out var a)) a?.Invoke();
    }

    // --------------- Low-level send ---------------
    public async Task<Frame?> SendRequestAsync(char job, byte[] body, CancellationToken token, bool waitForResponse = true)
    {
        if (!IsOpen) throw new InvalidOperationException("Serial not open.");

        // 1) Build frame
        var frame = BuildFrame(job, body);
        byte[] buf = frame.ToBytes();

        // 2) ACK 재시도 (최대 3회)
        int tries = 0;
        while (true)
        {
            token.ThrowIfCancellationRequested();
            _sp.Write(buf, 0, buf.Length);
            Log($"[TX] {job} ({buf.Length}B)  DataLen={frame.DataLength}");

            var result = await WaitForAckOrNackAsync(ackWaitMs, token);
            if (result == AckKind.Ack) { Log("[ACK] 0x06 수신"); break; }

            Log(result == AckKind.Nack ? "[NACK] 0x15 수신 → 재전송" : "[ACK TIMEOUT] 3초 내 미수신");
            if (++tries >= ackRetryMax)
                throw new TimeoutException("ACK 미수신(또는 NACK 3회)으로 실패");
        }

        // (특수) 응답전문을 기다리지 않는 명령 (예: R: Reset 등)
        if (!waitForResponse)
            return null;

        // 3) 응답전문 대기 (최대 25초 권장)
        var resp = await WaitForResponseAsync(respWaitMaxMs, token);
        if (resp == null)
            throw new TimeoutException("응답전문 수신 실패");
        // 4) 응답 수신확인
        WriteByte(0x06);
        Log("[ACK->] 0x06 전송(응답전문 수신확인)");
        return resp;
    }

    // 고수준 헬퍼들 ----------------------------------------------------------
    public Task<Frame?> A_DeviceCheck(CancellationToken ct) => SendRequestAsync('A', Array.Empty<byte>(), ct);
    public Task<Frame?> V_Version(CancellationToken ct) => SendRequestAsync('V', Array.Empty<byte>(), ct);
    public Task<Frame?> K_KeyRenew(CancellationToken ct) => SendRequestAsync('K', Array.Empty<byte>(), ct);
    public Task<Frame?> R_Reset(CancellationToken ct) => SendRequestAsync('R', Array.Empty<byte>(), ct, waitForResponse: false); // ★ ACK only
    public Task<Frame?> L_LastApproval(CancellationToken ct) => SendRequestAsync('L', Array.Empty<byte>(), ct);

    /// <summary>간단 승인(B-1)</summary>
    public Task<Frame?> B_Approve(long totalAmount, long tax = 0, long service = 0, int installmentMonths = 0, bool signLess = true, CancellationToken ct = default)
    {
        string gubun = "1";
        string amt = Num(totalAmount, 10);
        string taxS = Num(tax, 8);
        string svc = Num(service, 8);
        string inst = Num(installmentMonths, 2);
        string sign = signLess ? "1" : "2"; // 1:비서명 2:서명

        var body = Encoding.ASCII.GetBytes(gubun + amt + taxS + svc + inst + sign); // 30B
        return SendRequestAsync('B', body, ct);
    }

    /// <summary>취소(C-2: 직전거래 승인취소)</summary>
    public Task<Frame?> C_Cancel_Last(CancellationToken ct) => SendRequestAsync('C', Encoding.ASCII.GetBytes("2"), ct);

    /// <summary>바코드/마지막내역 조회(W)</summary>
    public Task<Frame?> W_Query(CancellationToken ct) => SendRequestAsync('W', Array.Empty<byte>(), ct);

    // -----------------------------------------------------------------------
    // 프레임 구조
    public struct Frame
    {
        public byte STX;                  // 0x02
        public string TerminalId;         // 16 (ASCII, left aligned, 0x00 pad)
        public string DateTimeOrVAN;      // 14 (YYYYMMddHHmmss 기본)
        public char Job;                  // 1
        public byte RespCode;             // 0x00
        public ushort DataLength;         // 2 (LE)
        public byte[] Data;               // var
        public byte ETX;                  // 0x03
        public byte BCC;                  // XOR(STX..ETX)

        public byte[] ToBytes()
        {
            var bytes = new byte[1 + 16 + 14 + 1 + 1 + 2 + Data.Length + 1 + 1];
            int o = 0;
            bytes[o++] = 0x02;
            o += PutAsciiFixed(bytes, o, TerminalId, 16, padWithZero: true);
            o += PutAsciiFixed(bytes, o, DateTimeOrVAN, 14, padWithZero: false);
            bytes[o++] = (byte)Job;
            bytes[o++] = RespCode;
            // DataLength LE
            bytes[o++] = (byte)(DataLength & 0xFF);
            bytes[o++] = (byte)((DataLength >> 8) & 0xFF);
            Buffer.BlockCopy(Data, 0, bytes, o, Data.Length); o += Data.Length;
            bytes[o++] = 0x03;
            // BCC = XOR from STX..ETX
            byte bcc = 0;
            for (int i = 0; i < o; i++) bcc ^= bytes[i];
            bytes[o++] = bcc;
            return bytes;
        }
    }

    Frame BuildFrame(char job, byte[] body)
    {
        var f = new Frame
        {
            STX = 0x02,
            TerminalId = LeftPadNulls(terminalId, 16),
            DateTimeOrVAN = overrideHeaderDateTime ? fixedHeaderDateTime14 : DateTime.Now.ToString("yyyyMMddHHmmss"),
            Job = job,
            RespCode = 0x00,
            DataLength = (ushort)(body?.Length ?? 0),
            Data = body ?? Array.Empty<byte>(),
            ETX = 0x03,
            BCC = 0
        };
        return f;
    }

    // ============ RX state machine ============
    void RxLoop()
    {
        var headerBuf = new byte[35]; // STX..DataLength 까지
        while (_run && _sp != null && _sp.IsOpen)
        {
            try
            {
                int b = _sp.ReadByte();
                if (b < 0) continue;

                // ACK / NACK 단일바이트
                if (b == 0x06) { Post(() => OnAck?.Invoke(new byte[] { 0x06 })); continue; }
                if (b == 0x15) { Post(() => OnNack?.Invoke(new byte[] { 0x15 })); continue; }

                if (b != 0x02) continue; // STX가 아니면 버림

                // 1) Header 나머지 34바이트 읽기(총 35)
                headerBuf[0] = 0x02;
                ReadExact(headerBuf, 1, 34);

                // 파싱
                string term = ReadAsciiLeft(headerBuf, 1, 16);
                string dt = ReadAsciiLeft(headerBuf, 17, 14);
                char job = (char)headerBuf[31];
                byte rcode = headerBuf[32];
                ushort dLen = (ushort)(headerBuf[33] | (headerBuf[34] << 8)); // LE

                // 2) Data + ETX + BCC
                var data = new byte[dLen];
                ReadExact(data, 0, dLen);
                int etx = _sp.ReadByte();
                int bcc = _sp.ReadByte();

                // 3) BCC 검증
                byte calc = 0;
                for (int i = 0; i < 35; i++) calc ^= headerBuf[i];
                for (int i = 0; i < data.Length; i++) calc ^= data[i];
                calc ^= (byte)etx;
                bool ok = (calc == (byte)bcc) && etx == 0x03;

                var frame = new Frame
                {
                    STX = 0x02,
                    TerminalId = term,
                    DateTimeOrVAN = dt,
                    Job = job,
                    RespCode = rcode,
                    DataLength = dLen,
                    Data = data,
                    ETX = (byte)etx,
                    BCC = (byte)bcc
                };

                if (!ok)
                {
                    Log($"[RX] Frame BCC mismatch. job={job}");
                    WriteByte(0x15); // NACK 회신
                    continue;
                }

                // ----- Abort 흡수 로직: 지연 응답 1건은 ACK 후 드롭 (스레드 안전한 시간 기준) -----
                if (_abortDrainOnce && _wall.ElapsedMilliseconds <= _abortExpireUntilMs) // ★
                {
                    WriteByte(0x06); // 수신확인만 보내고
                    Log("[ABORT] 지연 응답 1건 흡수(ACK 후 드롭)");
                    _abortDrainOnce = false;
                    continue;        // 앱으로 전달하지 않음
                }

                // @ 이벤트는 ACK/NACK 보내지 말 것
                if (job == '@')
                {
                    string ev = Encoding.ASCII.GetString(frame.Data, 0, Math.Min(frame.Data.Length, 1));
                    Post(() => OnEventAt?.Invoke(ev.Length > 0 ? ev[0] : '?', Encoding.ASCII.GetString(frame.Data)));
                    Log($"[@] 이벤트={ev}");
                }
                else
                {
                    Post(() => OnResponse?.Invoke(frame));
                    Log($"[RX] job={job} dataLen={frame.DataLength}");
                }
            }
            catch (TimeoutException) { }
            catch (Exception ex)
            {
                Log("[RX ERR] " + ex.Message);
                Thread.Sleep(50);
            }
        }
    }

    // --------------- helpers ---------------
    void ReadExact(byte[] buf, int off, int len)
    {
        int got = 0;
        while (got < len)
        {
            int n = _sp.Read(buf, off + got, len - got);
            if (n <= 0) throw new TimeoutException("Read timeout");
            got += n;
        }
    }

    void WriteByte(byte v) { _sp.Write(new byte[] { v }, 0, 1); }

    async Task<AckKind> WaitForAckOrNackAsync(int waitMs, CancellationToken ct)
    {
        AckKind result = AckKind.Timeout;
        void onAck(byte[] _) => result = AckKind.Ack;
        void onNack(byte[] _) => result = AckKind.Nack;

        OnAck += onAck; OnNack += onNack;
        int waited = 0;
        try
        {
            while (result == AckKind.Timeout && waited < waitMs)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(20, ct);
                waited += 20;
            }
        }
        finally { OnAck -= onAck; OnNack -= onNack; }
        return result;
    }

    async Task<Frame?> WaitForResponseAsync(int waitMs, CancellationToken ct)
    {
        Frame? got = null;
        void onResp(Frame f) { got = f; }
        OnResponse += onResp;
        int waited = 0;
        try
        {
            while (got == null && waited < waitMs)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(50, ct);
                waited += 50;
            }
        }
        finally { OnResponse -= onResp; }
        return got;
    }
    public async Task<bool> RecoverToIdleWithA(CancellationToken ct, float absorbWindowSeconds = 10f)
    {
        // 1) 우리 쪽 대기 취소 → 다음 응답 1건은 ACK 후 드롭(드라이버가 처리)
        AbortWaitingGracefully(absorbWindowSeconds);

        // 2) 약간의 여유(지연 응답이 들어올 시간)
        await Task.Delay(300, ct);

        // 3) 장치 상태 동기화 (대부분의 케이스에서 Idle 복귀)
        var resp = await A_DeviceCheck(ct); // 응답이 오면 Idle로 정상 회복 판단
        return resp != null;
    }
    /// 위와 동일하되, 마지막 승인 내역을 확인해야 하는 경우 L을 통해 동기화합니다.
    /// (이중승인 방지 플로우: B/G 응답 누락 의심 시)
    /// </summary>
    public async Task<bool> RecoverToIdleWithL(CancellationToken ct, float absorbWindowSeconds = 10f)
    {
        AbortWaitingGracefully(absorbWindowSeconds);
        await Task.Delay(300, ct);

        var resp = await L_LastApproval(ct); // 직전 승인내역을 받아 장비 상태/트랜잭션 종료
        return resp != null;
    }

    static int PutAsciiFixed(byte[] buf, int off, string s, int fixedLen, bool padWithZero)
    {
        byte pad = padWithZero ? (byte)0x00 : (byte)0x20;
        for (int i = 0; i < fixedLen; i++) buf[off + i] = pad;
        if (string.IsNullOrEmpty(s)) return fixedLen;
        var b = Encoding.ASCII.GetBytes(s);
        int copy = Mathf.Min(b.Length, fixedLen);
        Buffer.BlockCopy(b, 0, buf, off, copy);
        return fixedLen;
    }

    static string ReadAsciiLeft(byte[] buf, int off, int len)
    {
        int real = len;
        while (real > 0 && (buf[off + real - 1] == 0x00 || buf[off + real - 1] == 0x20)) real--;
        return Encoding.ASCII.GetString(buf, off, real);
    }

    static string Num(long v, int width)
    {
        if (v < 0) v = 0;
        var s = v.ToString();
        if (s.Length > width) s = s.Substring(s.Length - width, width);
        return s.PadLeft(width, '0'); // 우측정렬, 좌측 '0'
    }

    static string LeftPadNulls(string s, int width)
    {
        if (s == null) s = "";
        if (s.Length > width) s = s.Substring(0, width);
        // 좌측정렬, 나머지 0x00
        return s;
    }

    void Log(string s) => Post(() => OnLog?.Invoke(s));
    void Post(Action a) => _toMain.Enqueue(a);

    enum AckKind { Timeout, Ack, Nack }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class KioskLogger : MonoBehaviour
{
    // ===== 설정 =====
    public string logFolderName = "Logs";     // persistentDataPath/Logs
    public string filePrefix = "kiosk";       // kiosk_2025-09-30_001.log
    public long maxFileBytes = 5 * 1024 * 1024; // 5MB 넘어가면 롤링
    public int maxFilesPerDay = 10;        // 하루 최대 로테이션 파일 수

    // ===== 내부 =====
    static KioskLogger _instance;
    static readonly object _initLock = new();
    string _logDir;
    string _logPath;
    DateTime _currentDate;
    int _sequence = 1;

    readonly ConcurrentQueue<string> _queue = new();
    CancellationTokenSource _cts;
    Thread _writerThread;

    // Unity가 가장 일찍 호출할 수 있게 Hook
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Bootstrap()
    {
        // 이미 존재하면 무시
        if (_instance != null) return;

        // 씬에 오브젝트 자동 생성
        var go = new GameObject("[KioskLogger]");
        _instance = go.AddComponent<KioskLogger>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        // 중복 방지
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;

        // 디렉터리 준비
        _logDir = Path.Combine(Application.dataPath, $"../{logFolderName}");

        Directory.CreateDirectory(_logDir);

        // 파일 경로 결정(날짜+시퀀스)
        _currentDate = DateTime.Now.Date;
        _logPath = NextLogFilePath();

        // 시스템/빌드 정보 헤더 기록
        WriteSync(HeaderText());

        // 이벤트 구독: 모든 Unity 로그/예외
        Application.logMessageReceivedThreaded += OnUnityLog;  // 스레드 안전한 콜백
        // 미관상 중복 방지: 위 Threaded 콜백이면 일반 콜백 불필요

        // 비관측 Task 예외
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        // 도메인 미처리 예외
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        // 백그라운드 쓰레드 시작
        _cts = new CancellationTokenSource();
        _writerThread = new Thread(WriterLoop) { IsBackground = true, Name = "KioskLoggerWriter" };
        _writerThread.Start();
    }

    void OnDestroy()
    {
        // 해제 및 잔여 플러시
        Application.logMessageReceivedThreaded -= OnUnityLog;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;

        _cts?.Cancel();
        _writerThread?.Join(1500);
        Flush();
    }

    void OnApplicationQuit()
    {
        Flush();
    }

    // ===== Unity 로그 훅 =====
    void OnUnityLog(string condition, string stackTrace, LogType type)
    {
        var sb = new StringBuilder(512);
        sb.Append('[').Append(TimeStamp()).Append("] ");
        sb.Append(type switch
        {
            LogType.Error => "[ERROR] ",
            LogType.Assert => "[ASSERT] ",
            LogType.Warning => "[WARN ] ",
            LogType.Log => "[INFO ] ",
            LogType.Exception => "[EXCP ] ",
            _ => "[LOG  ] "
        });
        sb.AppendLine(condition);

        if (!string.IsNullOrWhiteSpace(stackTrace))
            sb.AppendLine(stackTrace.TrimEnd());

        Enqueue(sb.ToString());
    }

    // ===== 예외 훅 =====
    void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        var msg = $"[UNHANDLED] {ex?.GetType().Name}: {ex?.Message}\n{ex?.StackTrace}";
        Enqueue($"[{TimeStamp()}] [FATAL] {msg}\n");
    }

    void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        Enqueue($"[{TimeStamp()}] [TASK ] UnobservedTaskException: {e.Exception}\n");
        e.SetObserved();
    }

    // ===== 퍼블릭 API =====
    public static void Info(string message) => _instance?.Enqueue($"[{_instance.TimeStamp()}] [INFO ] {message}\n");
    public static void Warn(string message) => _instance?.Enqueue($"[{_instance.TimeStamp()}] [WARN ] {message}\n");
    public static void Error(string message) => _instance?.Enqueue($"[{_instance.TimeStamp()}] [ERROR] {message}\n");

    public static string CurrentLogPath => _instance?._logPath;

    // ===== 내부 유틸 =====
    void Enqueue(string line)
    {
        // 날짜 변경/파일 사이즈 체크 후 롤링
        TryRoll();

        _queue.Enqueue(line);
    }

    void TryRoll()
    {
        // 날짜 바뀌면 새 파일
        var today = DateTime.Now.Date;
        if (today != _currentDate)
        {
            _currentDate = today;
            _sequence = 1;
            _logPath = NextLogFilePath();
            WriteSync(HeaderText());
            return;
        }

        // 파일 사이즈 초과 시 롤링
        try
        {
            if (File.Exists(_logPath))
            {
                var len = new FileInfo(_logPath).Length;
                if (len >= maxFileBytes)
                {
                    _sequence = Math.Min(_sequence + 1, maxFilesPerDay);
                    _logPath = NextLogFilePath();
                    WriteSync(HeaderText());
                }
            }
        }
        catch { /* 파일 잠김 등 무시 */ }
    }

    string NextLogFilePath()
    {
        // kiosk_YYYY-MM-DD_seq.log
        string date = _currentDate.ToString("yyyy-MM-dd");
        string seq = _sequence.ToString("D3");
        return Path.Combine(_logDir, $"{filePrefix}_{date}_{seq}.log");
    }

    string HeaderText()
    {
        var sb = new StringBuilder(512);
        sb.AppendLine("===============================================");
        sb.AppendLine($"KIOSK LOG START  : {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        sb.AppendLine($"APP              : {Application.productName} ({Application.version})");
        sb.AppendLine($"UNITY            : {Application.unityVersion}");
        sb.AppendLine($"PLATFORM         : {Application.platform}");
        sb.AppendLine($"DEVICE           : {SystemInfo.deviceModel} / {SystemInfo.deviceName}");
        sb.AppendLine($"OS               : {SystemInfo.operatingSystem}");
        sb.AppendLine($"CPU/GPU/RAM/VRAM : {SystemInfo.processorType} / {SystemInfo.graphicsDeviceName} / {SystemInfo.systemMemorySize}MB / {SystemInfo.graphicsMemorySize}MB");
        sb.AppendLine($"PATH             : {Application.persistentDataPath}");
        sb.AppendLine("===============================================");
        return sb.ToString();
    }

    string TimeStamp() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

    void WriterLoop()
    {
        var token = _cts.Token;
        var batch = new List<string>(64);

        while (!token.IsCancellationRequested)
        {
            try
            {
                // 배치 수집
                if (_queue.TryDequeue(out var line))
                {
                    batch.Clear();
                    batch.Add(line);
                    while (batch.Count < 64 && _queue.TryDequeue(out var more))
                        batch.Add(more);

                    // 파일에 append
                    File.AppendAllLines(_logPath, batch, Encoding.UTF8);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch
            {
                // 디스크 잠김/일시 오류는 다음 루프로
                Thread.Sleep(50);
            }
        }

        // 종료 시 남은 것 플러시
        Flush();
    }

    void WriteSync(string text)
    {
        try
        {
            File.AppendAllText(_logPath, text, Encoding.UTF8);
        }
        catch { /* 무시 */ }
    }

    void Flush()
    {
        var list = new List<string>(1024);
        while (_queue.TryDequeue(out var line))
            list.Add(line);

        if (list.Count > 0)
        {
            try { File.AppendAllLines(_logPath, list, Encoding.UTF8); }
            catch { /* 무시 */ }
        }
    }
}

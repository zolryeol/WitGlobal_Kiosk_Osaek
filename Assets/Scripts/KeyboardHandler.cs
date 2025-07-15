using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardHandler : MonoBehaviour
{
    // user32.dll에서 LoadKeyboardLayout 함수 가져오기
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    private const uint WM_SYSCOMMAND = 0x0112;
    private readonly IntPtr SC_CLOSE = new IntPtr(0xF060);

    // 인풋필드 객체 설정
    [SerializeField]
    private TMP_InputField inputField;

    // 키보드 레이아웃 코드
    [SerializeField]
    private string languageCode = "00000412";   // 한국어

    // 화상키보드 프로세스
    private Process oskProcess;

    private void Start()
    {
        // 씬 언로드 이벤트에 콜백 등록
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        // 씬 언로드 이벤트에서 콜백 해제
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // 키보드가 열릴 때 (EventTrigger 컴포넌트의 PointerClick 이벤트로 연결)
    public void OnKeyboardOpen()
    {
        UnityEngine.Debug.Log("OnKeyboardOpen 호출됨");

        // 한국어 선택시
        if (LanguageService.apiLanguageParse() == "ko")
        {
            languageCode = "00000412";
        }
        // 그 외는 영어로
        else
        {
            languageCode = "00000409";
        }

        // 언어변경
        LoadKeyboardLayout(languageCode, 1);

        // 화상키보드 실행
        try
        {
            TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);

            //foreach (var process in Process.GetProcessesByName("TabTip"))
            //{
            //    process.Kill();
            //}

            //oskProcess = Process.Start(@"C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe");
            //oskProcess = Process.Start("osk.exe");
            UnityEngine.Debug.Log("화상키보드 실행됨");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"화상키보드 실행 실패: {ex.Message}");
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        CloseOnScreenKeyboard();
    }

    private void CloseOnScreenKeyboard()
    {
        // 우리가 실행한 프로세스가 있는지 확인
        //if (oskProcess != null && !oskProcess.HasExited)
        //{
        //    oskProcess.Kill();
        //    oskProcess = null;
        //    UnityEngine.Debug.Log("화상키보드 프로세스 종료됨");
        //}
        //else
        //{
        //    // 프로세스로 찾지 못했다면 윈도우 메시지로 종료 시도
        //    IntPtr keyboardWnd = FindWindow("IPTip_Main_Window", null);

        //    if (keyboardWnd != IntPtr.Zero)
        //    {
        //        // 창에 WM_SYSCOMMAND 메시지로 SC_CLOSE를 보냅니다.
        //        PostMessage(keyboardWnd, WM_SYSCOMMAND, SC_CLOSE, IntPtr.Zero);
        //        UnityEngine.Debug.Log("화상키보드 창 닫힘");
        //    }
        //}

        foreach (var process in Process.GetProcessesByName("TabTip"))
        {
            process.Kill();
            UnityEngine.Debug.Log("화상키보드 프로세스 종료됨");
        }

    }
}

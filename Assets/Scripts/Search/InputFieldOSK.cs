using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using System;

public class InputFieldOSK : MonoBehaviour
{
    public TMP_InputField inputField;

    // Importing the User32.dll to interact with the OSK window
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_CLOSE = 0x0010;

    // Start is called before the first frame update
    void Start()
    {
        // Adding an EventTrigger to the InputField
        EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();

        // Defining the "Select" event
        EventTrigger.Entry selectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select
        };
        selectEntry.callback.AddListener((eventData) => { ShowKeyboard(); });
        trigger.triggers.Add(selectEntry);

        // Listen for deselect events globally
        EventTrigger.Entry deselectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Deselect
        };
        deselectEntry.callback.AddListener((eventData) => { HideKeyboard(); });
        trigger.triggers.Add(deselectEntry);
    }

    private void ShowKeyboard()
    {
        // Check if the touch keyboard is already open
        IntPtr keyboardHandle = FindWindow("IPTip_Main_Window", null);

        if (keyboardHandle == IntPtr.Zero)
        {
            // Launch the on-screen keyboard
            Process.Start("osk.exe");
        }
    }

    private void HideKeyboard()
    {
        IntPtr keyboardHandle = FindWindow("IPTip_Main_Window", null);
        if (keyboardHandle != IntPtr.Zero)
        {
            PostMessage(keyboardHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    void Update()
    {
        // Close the OSK if clicking outside the input field
        if (inputField != EventSystem.current.currentSelectedGameObject)
        {
            HideKeyboard();
        }
    }
}
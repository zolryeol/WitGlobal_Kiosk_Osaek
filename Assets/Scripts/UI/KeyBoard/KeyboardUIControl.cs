using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class KeyboardUIControl : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject keyboard;

    private void Awake()
    {
        button.onClick.AddListener(OnKeyboard);
    }
    private void Start()
    {
        keyboard.SetActive(false);
    }

    void OnKeyboard()
    {
        Debug.Log("키보드 온");
        if (keyboard.activeSelf)
        {
            keyboard.SetActive(false);
        }
        else
        {
            keyboard.SetActive(true);
        }
    }
}
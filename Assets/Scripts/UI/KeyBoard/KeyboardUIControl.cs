using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class KeyboardUIControl : MonoBehaviour
{
    public VirtualKeyboard virtualKeyboard;

    public TMP_InputField searchInput;
    void Start()
    {
        searchInput.onSelect.AddListener(OnInputFieldSelected);
    }
    void OnInputFieldSelected(string _)
    {
        virtualKeyboard.OpenKeyboard(searchInput);
    }
}

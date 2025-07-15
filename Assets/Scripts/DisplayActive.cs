using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayActive : MonoBehaviour
{
    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);

        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate(); 
        }
        else
        {
            Debug.Log("displays not connected.");
        }
    }
}

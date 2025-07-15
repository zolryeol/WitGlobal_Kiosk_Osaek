using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrierFreeInitializer : MonoBehaviour
{
    [SerializeField]
    public GameObject barrierFreePanel;

    void Start()
    {
        //Debug.LogError($"AR Select BarrierFreeFlag : {GlobalManager.Instance.barrierFreeFlag}");
        if (GlobalManager.Instance.barrierFreeFlag)
        {
            barrierFreePanel.SetActive(true);
        }
        else
        {
            barrierFreePanel.SetActive(false);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeButton : Button
{
    protected override void Start()
    {
        base.Start();
        this.onClick.AddListener(() => UIManager.Instance.CloseAllPages());
        this.onClick.AddListener(() => UIManager.Instance.DeselectAllCustomButtons(UIManager.Instance.SecondCategorieButtons));
        this.onClick.AddListener(() => UIManager.Instance.DeselectAllCustomButtons(UIManager.Instance.HanbokCategorieButtons));
    }
}

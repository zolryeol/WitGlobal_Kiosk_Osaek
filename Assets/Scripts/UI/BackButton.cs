using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : Button
{
    protected override void Start()
    {
        base.Start();

        this.onClick.AddListener(() => VideoPlayManager.Instance.PlayPreviousVideoIfValid());
        this.onClick.AddListener(() =>
            UIManager.Instance.ClosePage((CanvasGroup)UIManager.Instance.PageStack.Pop()));

    }
}

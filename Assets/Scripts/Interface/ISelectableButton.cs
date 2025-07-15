using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableButton
{
    public void SetSelected(bool selected);
    public void OnButtonClicked();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrderable
{
    public void AwakeOrder();
    public void StartOrder();

    public void UpdateOrder();
}

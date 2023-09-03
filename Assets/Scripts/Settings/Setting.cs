using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Setting<T> : MonoBehaviour
{
    public string Key;
    public T Value;
    public bool IsPendingChange;

    public abstract void Restore();
    public abstract void OnValueChange(T value);
}

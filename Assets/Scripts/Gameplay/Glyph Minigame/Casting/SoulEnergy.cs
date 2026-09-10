using System;
using UnityEngine;

public class SoulEnergy : MonoBehaviour
{
    public bool IsAvailable { get; private set; }

    public event Action OnSoulReleased;

    public void ReleaseSoul()
    {
        if (IsAvailable) return;
        IsAvailable = true;
        OnSoulReleased?.Invoke();
    }

    public void Consume()
    {
        IsAvailable = false;
    }
}

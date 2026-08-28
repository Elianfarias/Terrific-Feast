using System;
using UnityEngine;

public class Wand : MonoBehaviour
{
    public event Action OnFailed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water") || other.CompareTag("Wall"))
        {
            OnFailed?.Invoke();
        }
    }
}
using System;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Wand : MonoBehaviour
{
    public event Action OnFailed;
    public event Action OnReachedGoal;

    private bool isActive = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Water") || other.CompareTag("Wall"))
        {
            OnFailed?.Invoke();
        }
        else if (other.CompareTag("Goal"))
        {
            OnReachedGoal?.Invoke();
        }
    }
    public void Init(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        isActive = true;
    }
    public void SetActiveState(bool value) => isActive = value;
    public void ResetToPosition(Vector3 position)
    {
        transform.position = position;
    }
}
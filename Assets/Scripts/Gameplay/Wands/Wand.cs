using System;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Wand : MonoBehaviour
{
    // El agua sigue reiniciando el laberinto. Las paredes ya no reinician,
    // solo avisan para que se pueda dar feedback (ej: vibrar la cámara).
    public event Action OnFailed;
    public event Action OnHitWall;
    public event Action OnReachedGoal;

    private bool isActive = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Water"))
        {
            OnFailed?.Invoke();
        }
        else if (other.CompareTag("Wall"))
        {
            OnHitWall?.Invoke();
        }
        else if (other.CompareTag("Goal"))
        {
            OnReachedGoal?.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;

        if (collision.collider.CompareTag("Water"))
        {
            OnFailed?.Invoke();
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            OnHitWall?.Invoke();
        }
        else if (collision.collider.CompareTag("Goal"))
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
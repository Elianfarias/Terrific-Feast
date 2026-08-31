using DG.Tweening;
using UnityEngine;

// Sacude la cámara sin tocar su posición base (tween sobre un offset local,
// nunca sobre transform.position directamente, para no pisar otros scripts).
public class CameraShake : MonoBehaviour
{
    [SerializeField] private float defaultDuration = 0.25f;
    [SerializeField] private float defaultStrength = 0.4f;
    [SerializeField] private int vibrato = 20;

    private Vector3 restingPosition;

    private void Awake() => restingPosition = transform.localPosition;

    public void Shake() => Shake(defaultDuration, defaultStrength);

    public void Shake(float duration, float strength)
    {
        transform.DOKill();
        transform.localPosition = restingPosition;
        transform.DOShakePosition(duration, strength, vibrato)
            .SetUpdate(true)
            .OnComplete(() => transform.localPosition = restingPosition);
    }
}

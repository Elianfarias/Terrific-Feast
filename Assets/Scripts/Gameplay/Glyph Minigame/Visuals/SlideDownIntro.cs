using System;
using DG.Tweening;
using UnityEngine;

public class SlideDownIntro : MonoBehaviour
{
    [SerializeField] private float dropDistance = 10f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    public event Action OnComplete;

    private void Awake()
    {
        Vector3 restingPosition = transform.position;
        transform.position = restingPosition + Vector3.up * dropDistance;
        transform.DOMove(restingPosition, duration).SetEase(ease).OnComplete(() => OnComplete?.Invoke());
    }
}

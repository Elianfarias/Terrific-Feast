using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SoulCreatureView : MonoBehaviour
{
    [SerializeField] private SoulEnergy soul;
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup creatureGroup;
    [SerializeField] private Sprite aliveSprite;
    [SerializeField] private Sprite deadSprite;
    [SerializeField] private float holdDeadDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float reviveFadeDuration = 0.2f;

    public event Action OnCreatureGone;

    private void OnEnable() => soul.OnSoulReleased += HandleSoulReleased;
    private void OnDisable() => soul.OnSoulReleased -= HandleSoulReleased;

    private void HandleSoulReleased()
    {
        StopAllCoroutines();
        StartCoroutine(DeathSequence());
    }

    // Cambia al sprite muerto, espera, se desvanece y avisa con OnCreatureGone.
    private IEnumerator DeathSequence()
    {
        if (image != null) image.sprite = deadSprite;

        yield return new WaitForSeconds(holdDeadDuration);

        creatureGroup.DOKill();
        creatureGroup.DOFade(0f, fadeOutDuration);
        yield return new WaitForSeconds(fadeOutDuration);

        OnCreatureGone?.Invoke();
    }

    public void Revive()
    {
        StopAllCoroutines();

        if (image != null) image.sprite = aliveSprite;

        creatureGroup.DOKill();
        creatureGroup.DOFade(1f, reviveFadeDuration);
    }
}

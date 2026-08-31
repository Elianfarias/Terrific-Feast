using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GlyphResultBanner : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Text label;
    [SerializeField] private float displayDuration = 1.2f;
    [SerializeField] private float fadeDuration = 0.25f;

    public event Action OnDismissed;

    private void OnEnable() => caster.OnInvocationResolved += HandleInvocationResolved;
    private void OnDisable() => caster.OnInvocationResolved -= HandleInvocationResolved;

    private void HandleInvocationResolved(GameObject result, DrinkRecipe usedRecipe, float accuracy)
    {
        StopAllCoroutines();
        StartCoroutine(ShowSequence(usedRecipe, accuracy));
    }

    // Muestra el resultado, espera, se desvanece y avisa con OnDismissed.
    private IEnumerator ShowSequence(DrinkRecipe recipe, float accuracy)
    {
        bool success = recipe != null && accuracy >= recipe.RequiredAccuracy;
        int percent = Mathf.RoundToInt(accuracy * 100f);

        if (label != null)
            label.text = success ? $"¡Glifo exitoso! {percent}%" : $"Glifo fallido {percent}%";

        group.DOKill();
        group.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        group.DOFade(0f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        OnDismissed?.Invoke();
    }
}

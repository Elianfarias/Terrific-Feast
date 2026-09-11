using System;
using UnityEngine;

public class GlyphCastController : MonoBehaviour
{
    [SerializeField] private SoulEnergy soul;
    [SerializeField] private PatternRecognizer recognizer;
    [SerializeField] private DrawTrailRenderer trail;

    [Header("Inicio del trazo")]
    [Tooltip("Tiempo mínimo que debe durar el clic para validar el trazo. El dibujo aparece inmediatamente.")]
    [SerializeField, Min(0f)] private float minimumHoldDuration = 1f;

    private DrinkRecipe currentRecipe;
    private bool pointerDown;
    private bool pendingResolution;
    private float pendingAccuracy;
    private float pointerDownTime;

    public event Action<GameObject, DrinkRecipe, float> OnInvocationResolved;
    public event Action<DrawPattern> OnPatternChanged;
    public event Action<DrinkRecipe> OnRecipeAssigned;
    public event Action OnEnterRequested;

    private void OnEnable() => recognizer.OnPatternComplete += ResolveInvocation;
    private void OnDisable()
    {
        recognizer.OnPatternComplete -= ResolveInvocation;
        pointerDown = false;
        pendingResolution = false;
    }

    // Asigna la receta activa y notifica a quien esté escuchando.
    public void SetRecipe(DrinkRecipe recipe)
    {
        pointerDown = false;
        pendingResolution = false;
        currentRecipe = recipe;
        DrawPattern pattern = recipe != null ? recipe.glyph : null;
        recognizer.SetPattern(pattern);
        OnPatternChanged?.Invoke(pattern);
        OnRecipeAssigned?.Invoke(recipe);
    }

    // Avisa que se apretó Enter; no libera el alma por sí solo.
    public void OnEnterPressed() => OnEnterRequested?.Invoke();

    public void ReleaseSoul() => soul.ReleaseSoul();

    public void OnDrawStart(Vector2 pos)
    {
        if (!soul.IsAvailable || currentRecipe == null) return;

        pointerDown = true;
        pointerDownTime = Time.unscaledTime;
        recognizer.StartDrawing();
        trail.BeginTrail(pos);
    }

    public void OnDrawUpdate(Vector2 pos)
    {
        if (!pointerDown || !soul.IsAvailable || currentRecipe == null) return;

        recognizer.UpdateDrawing(pos);
        trail.AddPoint(pos);

        if (pendingResolution && Time.unscaledTime - pointerDownTime >= minimumHoldDuration)
            CompleteInvocation(pendingAccuracy);
    }

    public void OnDrawEnd()
    {
        if (!pointerDown) return;

        bool heldLongEnough = Time.unscaledTime - pointerDownTime >= minimumHoldDuration;
        pointerDown = false;
        if (!heldLongEnough)
        {
            recognizer.CancelDrawing();
            pendingResolution = false;
            trail.ClearTrail();
            return;
        }

        if (pendingResolution)
        {
            CompleteInvocation(pendingAccuracy);
            return;
        }

        recognizer.StopDrawing();
    }

    // Resuelve el trazo terminado y consume el alma.
    private void ResolveInvocation(float accuracy)
    {
        if (pointerDown && Time.unscaledTime - pointerDownTime < minimumHoldDuration)
        {
            pendingAccuracy = accuracy;
            pendingResolution = true;
            return;
        }

        CompleteInvocation(accuracy);
    }

    private void CompleteInvocation(float accuracy)
    {
        pendingResolution = false;
        soul.Consume();
        trail.ClearTrail();

        GameObject result = currentRecipe.ResolveOutcome(accuracy);
        OnInvocationResolved?.Invoke(result, currentRecipe, accuracy);
    }
}

using System;
using UnityEngine;

public class GlyphCastController : MonoBehaviour
{
    [SerializeField] private SoulEnergy soul;
    [SerializeField] private PatternRecognizer recognizer;
    [SerializeField] private DrawTrailRenderer trail;

    [Header("Inicio del trazo")]
    [Tooltip("Tiempo que hay que mantener el clic para iniciar el glifo.")]
    [SerializeField, Min(0f)] private float minimumHoldDuration = 1f;

    private DrinkRecipe currentRecipe;
    private bool pointerDown;
    private bool drawingStarted;
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
        drawingStarted = false;
    }

    // Asigna la receta activa y notifica a quien esté escuchando.
    public void SetRecipe(DrinkRecipe recipe)
    {
        pointerDown = false;
        drawingStarted = false;
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

        // Un clic corto queda como una intención cancelada: no consume el
        // alma ni dispara un resultado con precisión cero.
        pointerDown = true;
        drawingStarted = false;
        pointerDownTime = Time.unscaledTime;
    }

    public void OnDrawUpdate(Vector2 pos)
    {
        if (!pointerDown || !soul.IsAvailable || currentRecipe == null) return;

        if (!drawingStarted)
        {
            if (Time.unscaledTime - pointerDownTime < minimumHoldDuration)
                return;

            drawingStarted = true;
            recognizer.StartDrawing();
            trail.BeginTrail(pos);
        }

        recognizer.UpdateDrawing(pos);
        trail.AddPoint(pos);
    }

    public void OnDrawEnd()
    {
        if (!pointerDown) return;

        pointerDown = false;
        if (!drawingStarted)
        {
            trail.ClearTrail();
            return;
        }

        drawingStarted = false;
        recognizer.StopDrawing();
    }

    // Resuelve el trazo terminado y consume el alma.
    private void ResolveInvocation(float accuracy)
    {
        soul.Consume();
        trail.ClearTrail();

        GameObject result = currentRecipe.ResolveOutcome(accuracy);
        OnInvocationResolved?.Invoke(result, currentRecipe, accuracy);
    }
}

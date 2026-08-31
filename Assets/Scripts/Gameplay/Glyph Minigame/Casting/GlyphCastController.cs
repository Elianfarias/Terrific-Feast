using System;
using UnityEngine;

public class GlyphCastController : MonoBehaviour
{
    [SerializeField] private SoulEnergy soul;
    [SerializeField] private PatternRecognizer recognizer;
    [SerializeField] private DrawTrailRenderer trail;

    private DrinkRecipe currentRecipe;

    public event Action<GameObject, DrinkRecipe, float> OnInvocationResolved;
    public event Action<DrawPattern> OnPatternChanged;
    public event Action<DrinkRecipe> OnRecipeAssigned;
    public event Action OnEnterRequested;

    private void OnEnable() => recognizer.OnPatternComplete += ResolveInvocation;
    private void OnDisable() => recognizer.OnPatternComplete -= ResolveInvocation;

    // Asigna la receta activa y notifica a quien esté escuchando.
    public void SetRecipe(DrinkRecipe recipe)
    {
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
        recognizer.StartDrawing();
        trail.BeginTrail(pos);
    }

    public void OnDrawUpdate(Vector2 pos)
    {
        if (!soul.IsAvailable || currentRecipe == null) return;
        recognizer.UpdateDrawing(pos);
        trail.AddPoint(pos);
    }

    public void OnDrawEnd()
    {
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

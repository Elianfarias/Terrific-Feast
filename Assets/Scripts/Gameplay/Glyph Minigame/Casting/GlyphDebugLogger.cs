using UnityEngine;

public class GlyphDebugLogger : MonoBehaviour
{
    [SerializeField] private PatternRecognizer recognizer;
    [SerializeField] private GlyphCastController caster;

    private void OnEnable()
    {
        recognizer.OnNodeHit += HandleNodeHit;
        recognizer.OnPatternComplete += HandlePatternComplete;
        caster.OnInvocationResolved += HandleInvocationResolved;
    }

    private void OnDisable()
    {
        recognizer.OnNodeHit -= HandleNodeHit;
        recognizer.OnPatternComplete -= HandlePatternComplete;
        caster.OnInvocationResolved -= HandleInvocationResolved;
    }

    private void HandleNodeHit(int index)
    {
        Debug.Log($"[Glyph] Nodo {index} tocado en orden.");
    }

    private void HandlePatternComplete(float accuracy)
    {
        Debug.Log($"[Glyph] Glifo completo. Precisión: {accuracy:P0}");
    }

    private void HandleInvocationResolved(GameObject result, DrinkRecipe recipe, float accuracy)
    {
        string resultName = result != null ? result.name : "ninguno (sin fallback)";
        bool wasCorrect = recipe != null && accuracy >= recipe.RequiredAccuracy;
        Debug.Log($"[Glyph] Receta '{recipe?.drinkName}' resuelta -> {resultName} " +
                  $"(precisión {accuracy:P0}, requerida {recipe?.RequiredAccuracy:P0}) " +
                  $"({(wasCorrect ? "CORRECTO" : "FALLO")})");
    }
}

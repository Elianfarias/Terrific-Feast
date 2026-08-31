using UnityEngine;

// Dispara la animación de efecto (burbujas, fuego, electricidad, etc.) del
// sabor cargado sobre el vaso, solo cuando el glifo sale exitoso.
public class DrinkPourEffect : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private SpriteSheetFlipbook flipbook;
    [SerializeField] private float duration = 2f;

    private void OnEnable() => caster.OnInvocationResolved += HandleInvocationResolved;
    private void OnDisable() => caster.OnInvocationResolved -= HandleInvocationResolved;

    private void HandleInvocationResolved(GameObject result, DrinkRecipe usedRecipe, float accuracy)
    {
        bool success = usedRecipe != null && accuracy >= usedRecipe.RequiredAccuracy;
        if (!success) return;

        Sprite[] frames = usedRecipe.glyph.pourEffectFrames;
        flipbook.Play(frames, duration);
    }
}

using UnityEngine;

// Muestra el vaso vacío al arrancar. Con cada glifo dibujado, se pinta con
// el color del sabor si salió bien, o queda negro si salió mal, hasta que
// se acierte uno. Vuelve a vacío recién en un trago/cliente nuevo.
[RequireComponent(typeof(SpriteRenderer))]
public class DrinkGlassDisplay : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite failSprite;

    private void Awake()
    {
        spriteRenderer.sprite = emptySprite;
    }

    private void OnEnable() => caster.OnInvocationResolved += HandleInvocationResolved;
    private void OnDisable() => caster.OnInvocationResolved -= HandleInvocationResolved;

    private void HandleInvocationResolved(GameObject result, DrinkRecipe usedRecipe, float accuracy)
    {
        bool success = usedRecipe != null && accuracy >= usedRecipe.RequiredAccuracy;
        Sprite glass = success ? usedRecipe.glyph.glassSprite : failSprite;
        if (glass == null) return;

        spriteRenderer.sprite = glass;
    }

    // Vuelve a mostrar el vaso vacío para un trago/cliente nuevo.
    public void ResetToEmpty()
    {
        spriteRenderer.sprite = emptySprite;
    }
}

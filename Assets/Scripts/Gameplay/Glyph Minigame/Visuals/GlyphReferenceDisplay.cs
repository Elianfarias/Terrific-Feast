using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlyphReferenceDisplay : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private SoulEnergy soul;
    [SerializeField] private SoulCreatureView creature;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 targetWorldSize = new Vector2(8f, 8f);

    private DrawPattern pendingPattern;

    private void OnEnable()
    {
        caster.OnPatternChanged += HandlePatternChanged;
        creature.OnCreatureGone += HandleCreatureGone;
    }

    private void OnDisable()
    {
        caster.OnPatternChanged -= HandlePatternChanged;
        creature.OnCreatureGone -= HandleCreatureGone;
    }

    private void HandlePatternChanged(DrawPattern pattern)
    {
        pendingPattern = pattern;

        if (soul.IsAvailable)
            Reveal();
        else
            spriteRenderer.enabled = false;
    }

    private void HandleCreatureGone() => Reveal();

    private void Reveal()
    {
        Sprite reference = pendingPattern != null ? pendingPattern.referenceSprite : null;
        spriteRenderer.sprite = reference;
        spriteRenderer.enabled = reference != null;

        if (reference != null)
            FitToTargetWorldSize(reference);
    }

    // Escala el sprite para que ocupe siempre targetWorldSize en el mundo,
    // sin importar el tamaño nativo de cada sprite de glifo.
    private void FitToTargetWorldSize(Sprite reference)
    {
        Vector2 nativeSize = reference.bounds.size;
        float scaleX = nativeSize.x > 0f ? targetWorldSize.x / nativeSize.x : 1f;
        float scaleY = nativeSize.y > 0f ? targetWorldSize.y / nativeSize.y : 1f;
        spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}

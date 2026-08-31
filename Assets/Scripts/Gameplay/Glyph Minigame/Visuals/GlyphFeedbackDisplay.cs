using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// Elemento de UI independiente y posicionable a mano dentro de un Canvas.
// Al resolverse un trazo, muestra bien.png o uy.png entrando desde un
// offset, desplegándose, y se oculta sola después de un rato.
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class GlyphFeedbackDisplay : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private Image image;

    [Header("Sprites")]
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    [Header("Animación (en píxeles de UI, no en unidades de mundo)")]
    [SerializeField] private Vector2 entryOffset = new Vector2(-300f, 300f);
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private Ease slideEase = Ease.OutBack;
    [SerializeField] private float holdDuration = 1.2f;
    [SerializeField] private float hideDuration = 0.25f;

    private RectTransform rect;
    private Vector2 restingAnchoredPosition;

    private void Awake()
    {
        rect = (RectTransform)transform;
        restingAnchoredPosition = rect.anchoredPosition;
        image.enabled = false;
    }

    private void OnEnable() => caster.OnInvocationResolved += HandleInvocationResolved;
    private void OnDisable() => caster.OnInvocationResolved -= HandleInvocationResolved;

    private void HandleInvocationResolved(GameObject result, DrinkRecipe usedRecipe, float accuracy)
    {
        bool success = usedRecipe != null && accuracy >= usedRecipe.RequiredAccuracy;
        Sprite feedback = success ? successSprite : failSprite;
        if (feedback == null) return;

        Play(feedback);
    }

    // Entra desde entryOffset desplegándose (escala 0 -> 1), se mantiene
    // un rato y después se desvanece sola.
    private void Play(Sprite feedback)
    {
        rect.DOKill();
        image.DOKill();

        image.sprite = feedback;
        image.enabled = true;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);

        rect.anchoredPosition = restingAnchoredPosition + entryOffset;
        rect.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(rect.DOAnchorPos(restingAnchoredPosition, slideDuration).SetEase(slideEase));
        sequence.Join(rect.DOScale(1f, slideDuration).SetEase(slideEase));
        sequence.AppendInterval(holdDuration);
        sequence.Append(image.DOFade(0f, hideDuration));
        sequence.OnComplete(() => image.enabled = false);
    }
}

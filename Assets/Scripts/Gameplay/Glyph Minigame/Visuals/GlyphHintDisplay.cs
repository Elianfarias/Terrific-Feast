using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// Muestra la pista de preferencias del cliente actual al apretar el botón
// de ayuda, con la misma animación de entrada/salida que el feedback de
// bien/mal (desliza, se despliega, se mantiene y se desvanece sola).
public class GlyphHintDisplay : MonoBehaviour
{
    [SerializeField] private MonsterCustomer customer;
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Text hintText;

    [Header("Animación")]
    [SerializeField] private Vector2 entryOffset = new Vector2(-400f, 0f);
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private Ease slideEase = Ease.OutBack;
    [SerializeField] private float holdDuration = 3f;
    [SerializeField] private float hideDuration = 0.25f;

    private Vector2 restingAnchoredPosition;

    // El chequeo de null es porque AddComponent (en el builder de editor)
    // dispara este Awake antes de que panel/group queden asignados.
    private void Awake()
    {
        if (panel == null || group == null) return;

        restingAnchoredPosition = panel.anchoredPosition;
        group.alpha = 0f;
        panel.gameObject.SetActive(false);
    }

    // Conectar al OnClick del botón de ayuda.
    public void ShowHint()
    {
        string pista = customer.Preferences != null ? customer.Preferences.pista : null;
        if (string.IsNullOrEmpty(pista)) return;

        hintText.text = pista;

        panel.DOKill();
        group.DOKill();

        panel.gameObject.SetActive(true);
        panel.anchoredPosition = restingAnchoredPosition + entryOffset;
        panel.localScale = Vector3.zero;
        group.alpha = 1f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOAnchorPos(restingAnchoredPosition, slideDuration).SetEase(slideEase));
        sequence.Join(panel.DOScale(1f, slideDuration).SetEase(slideEase));
        sequence.AppendInterval(holdDuration);
        sequence.Append(group.DOFade(0f, hideDuration));
        sequence.OnComplete(() => panel.gameObject.SetActive(false));
    }
}

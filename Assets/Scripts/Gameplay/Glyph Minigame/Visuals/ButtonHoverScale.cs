using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float duration = 0.12f;

    [Header("Sonido (opcional)")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private Vector3 baseScale;
    private bool hovering;
    private bool pressed;
    private float externalScale = 1f;

    private void Awake() => baseScale = transform.localScale;

    // Si el panel se desactiva con el mouse encima, OnPointerExit nunca
    // llega a dispararse y "hovering" queda trabado en true. Al reactivarse
    // no hay ningún evento que lo corrija, así que lo reseteamos acá.
    private void OnDisable()
    {
        hovering = false;
        pressed = false;

        transform.DOKill();
        transform.localScale = baseScale * externalScale;
    }

    // Multiplicador externo (ej: "seleccionado") que se combina con el hover.
    public void SetExternalScale(float multiplier)
    {
        externalScale = multiplier;
        Refresh();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        Refresh();

        if (hoverSound != null && AudioController.Instance != null)
            AudioController.Instance.PlaySoundEffect(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        Refresh();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        Refresh();

        if (clickSound != null && AudioController.Instance != null)
            AudioController.Instance.PlaySoundEffect(clickSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        Refresh();
    }

    // Si hay un scale "externo" activo (ej: seleccionado por click), ese es
    // el tope y no se combina con el hover/press para no sumarse entre sí.
    private void Refresh()
    {
        float interactionScale = pressed ? pressScale : (hovering ? hoverScale : 1f);
        float finalScale = externalScale != 1f ? externalScale : interactionScale;

        transform.DOKill();
        // SetUpdate(true) = tiempo sin escalar, para que funcione con
        // Time.timeScale = 0 (ej: menú pausado).
        transform.DOScale(baseScale * finalScale, duration).SetUpdate(true);
    }
}

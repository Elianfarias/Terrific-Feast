using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float duration = 0.12f;

    private Vector3 baseScale;
    private bool hovering;
    private bool pressed;
    private float externalScale = 1f;

    private void Awake() => baseScale = transform.localScale;

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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        Refresh();
    }

    private void Refresh()
    {
        float interactionScale = pressed ? pressScale : (hovering ? hoverScale : 1f);

        transform.DOKill();
        transform.DOScale(baseScale * externalScale * interactionScale, duration);
    }
}

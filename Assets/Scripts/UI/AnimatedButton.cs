using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class AnimatedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [Tooltip("Script for handling ui button animations")]
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float pressScale = 0.94f;
    [SerializeField] private float animSpeed = 12f;

    private RectTransform rect;
    private Vector3 baseScale;
    private Vector3 targetScale;
    private void Awake()
    {
        rect = (RectTransform)transform;
        baseScale = rect.localScale;
        targetScale = baseScale;
    }
    private void Update()
    {
        rect.localScale = Vector3.Lerp(rect.localScale, targetScale, Time.deltaTime * animSpeed);
    }
    public void OnPointerEnter(PointerEventData eventData) => targetScale = baseScale * hoverScale;

    public void OnPointerExit(PointerEventData eventData) => targetScale = baseScale;

    public void OnPointerDown(PointerEventData eventData) => targetScale = baseScale * pressScale;

    public void OnPointerUp(PointerEventData eventData) => targetScale = baseScale * hoverScale;
}
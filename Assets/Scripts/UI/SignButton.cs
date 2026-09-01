using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SignButton : MonoBehaviour, IPointerClickHandler
{
    private CanvasGroup canvasGroup;
    public event Action<SignButton> OnClicked;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (canvasGroup != null && !canvasGroup.blocksRaycasts) return;
        OnClicked?.Invoke(this);
    }
    public void SetInteractable(bool value)
    {
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = value;
        canvasGroup.interactable = value;
    }
}
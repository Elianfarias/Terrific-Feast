using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlavorSlotView : MonoBehaviour
{
    [SerializeField] private DrawPattern flavor;
    [SerializeField] private Button button;
    [SerializeField] private Image glyphIcon;
    [SerializeField] private Text nameLabel;
    [SerializeField] private CanvasGroup slotGroup;
    [SerializeField] private float selectedScale = 1.15f;
    [SerializeField] private float scaleDuration = 0.15f;

    public DrawPattern Flavor => flavor;
    public Image GlyphIcon => glyphIcon;

    public event Action<FlavorSlotView> OnClicked;

    private ButtonHoverScale hoverScale;

    private void Awake()
    {
        if (nameLabel != null && flavor != null)
            nameLabel.text = flavor.glyphName;

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                EventSystem.current?.SetSelectedGameObject(null);
                OnClicked?.Invoke(this);
            });
        }

        if (glyphIcon != null)
            hoverScale = glyphIcon.GetComponent<ButtonHoverScale>();
    }

    // Highlight del glifo elegido, todavía sin confirmar con Enter.
    public void SetSelected(bool selected)
    {
        if (glyphIcon == null) return;

        if (hoverScale != null)
        {
            hoverScale.SetExternalScale(selected ? selectedScale : 1f);
            return;
        }

        glyphIcon.rectTransform.DOKill();
        glyphIcon.rectTransform.DOScale(selected ? selectedScale : 1f, scaleDuration);
    }

    public void SetLocked(bool locked)
    {
        if (button != null)
            button.interactable = !locked;

        if (slotGroup != null)
            slotGroup.alpha = locked ? 0.35f : 1f;
    }
}

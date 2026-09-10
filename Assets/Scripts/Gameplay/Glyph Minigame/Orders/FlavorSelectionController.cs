using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FlavorSelectionController : MonoBehaviour
{
    [SerializeField] private List<FlavorSlotView> slots = new List<FlavorSlotView>();
    [SerializeField] private int maxFlavors = 3;

    [Header("Transición al centro")]
    [SerializeField] private RectTransform flightParent;
    [SerializeField] private RectTransform centerTarget;
    [SerializeField] private Vector2 flightIconSize = new Vector2(140f, 140f);
    [SerializeField] private float flightDuration = 0.4f;
    [SerializeField] private float holdAtCenter = 0.15f;
    [SerializeField] private float fadeOutDuration = 0.2f;

    private readonly HashSet<DrawPattern> usedFlavors = new HashSet<DrawPattern>();

    public bool HasReachedMax => usedFlavors.Count >= maxFlavors;

    // Se dispara apenas se clickea un sabor válido: ya no hace falta
    // confirmar con Enter, el click mata la libélula y arranca el glifo.
    public event System.Action<DrawPattern> OnFlavorChosen;

    private void Awake()
    {
        foreach (var slot in slots)
            slot.OnClicked += HandleSlotClicked;
    }

    private void OnDestroy()
    {
        foreach (var slot in slots)
            if (slot != null) slot.OnClicked -= HandleSlotClicked;
    }

    // Desbloquea todos los slots para un trago nuevo.
    // Reservado para cuando llegue un cliente/trago realmente nuevo.
    public void ResetForNewTrago()
    {
        usedFlavors.Clear();

        foreach (var slot in slots)
            slot.SetLocked(false);
    }

    // Bloquea todos los slots tal cual quedaron: el trago ya se sirvió y no
    // se puede seguir eligiendo hasta que arranque uno nuevo.
    public void LockAll()
    {
        foreach (var slot in slots)
            slot.SetLocked(true);
    }

    // Click directo: confirma el sabor al toque (anima el glifo al centro,
    // lo bloquea) y avisa para que arranque el minijuego de ese sabor.
    private void HandleSlotClicked(FlavorSlotView slot)
    {
        if (usedFlavors.Contains(slot.Flavor)) return;
        if (HasReachedMax) return;

        usedFlavors.Add(slot.Flavor);
        PlayFlyToCenter(slot);
        slot.SetLocked(true);

        OnFlavorChosen?.Invoke(slot.Flavor);
    }

    // Anima el glifo elegido hacia el centro de la pantalla.
    private void PlayFlyToCenter(FlavorSlotView slot)
    {
        if (flightParent == null || centerTarget == null) return;

        Sprite glyphSprite = slot.Flavor != null ? slot.Flavor.referenceSprite : null;
        if (glyphSprite == null) return;

        Vector3 startPosition = slot.GlyphIcon != null ? slot.GlyphIcon.rectTransform.position : slot.transform.position;

        GameObject clone = new GameObject("Flying Glyph", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        clone.transform.SetParent(flightParent, false);

        RectTransform cloneRect = clone.GetComponent<RectTransform>();
        cloneRect.sizeDelta = flightIconSize;
        cloneRect.position = startPosition;

        Image cloneImage = clone.GetComponent<Image>();
        cloneImage.sprite = glyphSprite;
        cloneImage.preserveAspect = true;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(cloneRect.DOMove(centerTarget.position, flightDuration).SetEase(Ease.OutBack));
        sequence.Join(cloneRect.DOScale(0.6f, flightDuration).SetEase(Ease.OutBack));
        sequence.AppendInterval(holdAtCenter);
        sequence.Append(cloneImage.DOFade(0f, fadeOutDuration));
        sequence.OnComplete(() => Destroy(clone));
    }
}

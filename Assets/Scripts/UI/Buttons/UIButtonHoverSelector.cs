using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private GameObject selectionLeft;
    [SerializeField] private GameObject selectionRight;

    private void Awake()
    {
        if (selectionLeft != null)
            selectionLeft.SetActive(false);
        if (selectionRight != null)
            selectionRight.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectionLeft != null)
            selectionLeft.SetActive(true);
        if (selectionRight != null)
            selectionRight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectionLeft != null)
            selectionLeft.SetActive(false);
        if (selectionRight != null)
            selectionRight.SetActive(false);
    }
}

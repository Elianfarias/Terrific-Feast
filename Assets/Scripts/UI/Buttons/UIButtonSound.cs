using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickSound);
    }


    private void OnDestroy()
    {
        btn.onClick.RemoveAllListeners();
    }

    private void OnClickSound()
    {
        AudioController.Instance.PlayButtonClickSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioController.Instance.PlayButtonHoverSound();
    }
}

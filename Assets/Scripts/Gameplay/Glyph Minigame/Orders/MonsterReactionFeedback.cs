using UnityEngine;
using UnityEngine.Events;

public class MonsterReactionFeedback : MonoBehaviour
{
    [SerializeField] private MonsterCustomer customer;
    [SerializeField] private SpriteRenderer faceRenderer;

    [Header("Sprite por reacción")]
    [SerializeField] private Sprite disgustaSprite;
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite gustaSprite;

    [Header("Feedbacks extra (opcional)")]
    [SerializeField] private UnityEvent onDisgusta;
    [SerializeField] private UnityEvent onNeutral;
    [SerializeField] private UnityEvent onGusta;

    private void OnEnable() => customer.OnReaction += HandleReaction;
    private void OnDisable() => customer.OnReaction -= HandleReaction;

    private void HandleReaction(MonsterCustomer monster, PreferenceTier reaction)
    {
        switch (reaction)
        {
            case PreferenceTier.Disgusta:
                SetFace(disgustaSprite);
                onDisgusta?.Invoke();
                break;
            case PreferenceTier.Gusta:
                SetFace(gustaSprite);
                onGusta?.Invoke();
                break;
            default:
                SetFace(neutralSprite);
                onNeutral?.Invoke();
                break;
        }
    }

    private void SetFace(Sprite sprite)
    {
        if (faceRenderer != null && sprite != null)
            faceRenderer.sprite = sprite;
    }
}

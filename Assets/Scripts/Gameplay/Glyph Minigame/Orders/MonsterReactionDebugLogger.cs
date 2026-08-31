using UnityEngine;

public class MonsterReactionDebugLogger : MonoBehaviour
{
    [SerializeField] private MonsterCustomer customer;

    private void OnEnable() => customer.OnReaction += HandleReaction;
    private void OnDisable() => customer.OnReaction -= HandleReaction;

    private void HandleReaction(MonsterCustomer monster, PreferenceTier reaction)
    {
        Debug.Log($"[Reacción] {monster.name} terminó el trago -> {reaction}");
    }
}

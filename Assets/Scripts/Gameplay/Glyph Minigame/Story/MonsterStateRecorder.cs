using UnityEngine;

public class MonsterStateRecorder : MonoBehaviour
{
    [SerializeField] private MonsterCustomer customer;

    private void OnEnable() => customer.OnReaction += HandleReaction;
    private void OnDisable() => customer.OnReaction -= HandleReaction;

    private void HandleReaction(MonsterCustomer monster, PreferenceTier reaction)
    {
        GameStateService.SetMonsterReaction(monster.MonsterId, reaction);
    }
}

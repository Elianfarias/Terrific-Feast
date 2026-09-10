using UnityEngine;

// Al servir el trago, graba en guardado.json si le gustó (Gusta) o no
// (Neutral/Disgusta) al cliente actual, para que la novela visual lo lea.
public class MonsterStateRecorder : MonoBehaviour
{
    [SerializeField] private MonsterCustomer customer;
    [SerializeField] private GameStateProgress progress;

    private void OnEnable() => customer.OnReaction += HandleReaction;
    private void OnDisable() => customer.OnReaction -= HandleReaction;

    private void HandleReaction(MonsterCustomer monster, PreferenceTier reaction)
    {
        bool liked = reaction == PreferenceTier.Gusta;

        progress.Progreso.miniGameResult = liked;

        switch (monster.MonsterId)
        {
            case "Tartu": progress.Progreso.afectoSapo = liked; break;
            case "Kerita": progress.Progreso.afectoKerita = liked; break;
            case "Fue": progress.Progreso.afectoFire = liked; break;
            case "Naima": progress.Progreso.afectoVamp = liked; break;
            default:
                Debug.LogWarning($"MonsterStateRecorder: monsterId \"{monster.MonsterId}\" no tiene campo de afecto asociado.");
                break;
        }

        progress.guardarProgreso();
    }
}

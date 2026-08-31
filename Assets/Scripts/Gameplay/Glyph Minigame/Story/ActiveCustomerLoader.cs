using UnityEngine;

// Lee activeChar de guardado.json (el progreso que arma la novela visual) y
// se lo pasa al MonsterCustomer del minijuego, para que sepa qué personaje
// es el cliente actual y busque sus preferencias correctas.
public class ActiveCustomerLoader : MonoBehaviour
{
    // Mismo orden fijo que Personaje.cs (ID FIJOS): 0=Tartu, 1=Kerita, 2=Fue, 3=Naima.
    private static readonly string[] CharacterIds = { "Tartu", "Kerita", "Fue", "Naima" };

    [SerializeField] private GameStateProgress progress;
    [SerializeField] private MonsterCustomer customer;

    private void Awake()
    {
        progress.cargarProgreso();

        int activeChar = progress.Progreso.activeChar;
        if (activeChar < 0 || activeChar >= CharacterIds.Length)
        {
            Debug.LogWarning($"ActiveCustomerLoader: activeChar {activeChar} fuera de rango, no se pudo identificar al cliente.");
            return;
        }

        customer.SetMonsterId(CharacterIds[activeChar]);
    }
}

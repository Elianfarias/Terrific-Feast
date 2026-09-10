using UnityEngine;

// Lee activeChar de guardado.json (el progreso que arma la novela visual) y
// se lo pasa al MonsterCustomer del minijuego, para que sepa qué personaje
// es el cliente actual y busque sus preferencias correctas.
public class ActiveCustomerLoader : MonoBehaviour
{
    // Mismo orden fijo que Personaje.cs (ID FIJOS): 0=Tartu, 1=Kerita, 2=Fue, 3=Naima.
    private static readonly string[] CharacterIds = { "Tartu", "Kerita", "Fue", "Naima" };

    // Nodo de Yarn donde arranca el encuentro con cada personaje (para
    // reintentarlo entero si el jugador muere en el minijuego).
    private static readonly string[] EntryNodes = { "tartu", "kerita", "fue", "naima" };

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
        customer.StartWaiting();
    }

    public static string GetEntryNode(int activeChar)
    {
        if (activeChar < 0 || activeChar >= EntryNodes.Length) return "Start";
        return EntryNodes[activeChar];
    }
}

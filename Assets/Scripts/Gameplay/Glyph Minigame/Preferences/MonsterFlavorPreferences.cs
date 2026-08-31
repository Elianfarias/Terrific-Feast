using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterPreferences", menuName = "Magic/Monster Flavor Preferences")]
public class MonsterFlavorPreferences : ScriptableObject
{
    [Header("\"Libélula sola\": le da igual qué glifo elijas, mientras lo dibujes bien")]
    [SerializeField] private bool ignoresFlavor = false;
    [SerializeField] private PreferenceTier universalTier = PreferenceTier.Gusta;

    public List<FlavorPreference> preferences = new List<FlavorPreference>();

    [Header("Pista para el jugador")]
    [TextArea(2, 5)] public string pista;

    // Tier de este personaje para un glifo (Neutral si no está en la lista).
    public PreferenceTier GetTier(DrawPattern glyph)
    {
        if (ignoresFlavor) return universalTier;

        FlavorPreference match = preferences.FirstOrDefault(p => p.glyph == glyph);
        return match != null ? match.tier : PreferenceTier.Neutral;
    }
}

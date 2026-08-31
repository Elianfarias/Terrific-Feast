using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Mapea un monsterId (el que va a llegar del JSON del cliente actual) a sus
// preferencias de sabor, sin necesitar un prefab por personaje.
[CreateAssetMenu(fileName = "MonsterPreferencesRegistry", menuName = "Magic/Monster Preferences Registry")]
public class MonsterPreferencesRegistry : ScriptableObject
{
    [Serializable]
    private class Entry
    {
        public string monsterId;
        public MonsterFlavorPreferences preferences;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();

    public MonsterFlavorPreferences GetPreferencesFor(string monsterId)
    {
        Entry match = entries.FirstOrDefault(e => e.monsterId == monsterId);
        if (match == null)
            Debug.LogWarning($"MonsterPreferencesRegistry: no hay preferencias registradas para \"{monsterId}\".");

        return match != null ? match.preferences : null;
    }
}

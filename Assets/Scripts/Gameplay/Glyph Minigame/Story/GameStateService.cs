using System.IO;
using System.Linq;
using UnityEngine;

public static class GameStateService
{
    private static string FilePath =>
        Path.Combine(Application.dataPath, "..", "SaveData", "game_state.json");

    private static GameStateData cached;

    // Carga el estado desde disco (o lo crea vacío) y lo cachea en memoria.
    public static GameStateData Load()
    {
        if (cached != null) return cached;

        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            cached = JsonUtility.FromJson<GameStateData>(json);
        }

        if (cached == null)
            cached = new GameStateData();

        return cached;
    }

    // Actualiza (o agrega) la reacción de un monstruo y guarda en disco.
    public static void SetMonsterReaction(string monsterId, PreferenceTier reaction)
    {
        GameStateData data = Load();
        MonsterStateEntry entry = data.monsterStates.FirstOrDefault(e => e.monsterId == monsterId);

        if (entry == null)
        {
            entry = new MonsterStateEntry { monsterId = monsterId };
            data.monsterStates.Add(entry);
        }

        entry.reaction = reaction;
        Save();
    }

    public static bool TryGetMonsterReaction(string monsterId, out PreferenceTier reaction)
    {
        MonsterStateEntry entry = Load().monsterStates.FirstOrDefault(e => e.monsterId == monsterId);

        if (entry == null)
        {
            reaction = default;
            return false;
        }

        reaction = entry.reaction;
        return true;
    }

    private static void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        File.WriteAllText(FilePath, JsonUtility.ToJson(cached, true));
    }
}

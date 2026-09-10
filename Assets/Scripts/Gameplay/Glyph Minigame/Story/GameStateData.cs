using System;
using System.Collections.Generic;

[Serializable]
public class MonsterStateEntry
{
    public string monsterId;
    public PreferenceTier reaction;
}

[Serializable]
public class GameStateData
{
    public List<MonsterStateEntry> monsterStates = new List<MonsterStateEntry>();
}

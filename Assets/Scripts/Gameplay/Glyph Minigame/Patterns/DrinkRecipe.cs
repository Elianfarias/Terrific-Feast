using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDrinkRecipe", menuName = "Magic/Drink Recipe")]
public class DrinkRecipe : ScriptableObject
{
    [Header("Identidad")]
    public string drinkName;
    public Sprite icon;

    [Header("Glifo requerido")]
    public DrawPattern glyph;

    [Header("Resultados según precisión (ordenar de mayor a menor minAccuracy)")]
    public List<GlyphOutcome> outcomes = new List<GlyphOutcome>();

    [Header("Si sale muy mal, invoca algo random de esta lista")]
    public List<GameObject> failFallbackPrefabs = new List<GameObject>();

    public GameObject CorrectResultPrefab => outcomes.Count > 0 ? outcomes[0].resultPrefab : null;

    public float RequiredAccuracy => outcomes.Count > 0 ? outcomes[0].minAccuracy : 1f;

    // Devuelve el prefab que corresponde a la precisión lograda.
    public GameObject ResolveOutcome(float accuracy)
    {
        foreach (var outcome in outcomes)
        {
            if (accuracy >= outcome.minAccuracy)
                return outcome.resultPrefab;
        }

        if (failFallbackPrefabs.Count > 0)
            return failFallbackPrefabs[Random.Range(0, failFallbackPrefabs.Count)];

        return null;
    }
}

[System.Serializable]
public class GlyphOutcome
{
    [Range(0f, 1f)] public float minAccuracy;
    public GameObject resultPrefab;
}

using UnityEngine;

[CreateAssetMenu(fileName = "DrinkPreferenceRules", menuName = "Magic/Drink Preference Rules")]
public class DrinkPreferenceRules : ScriptableObject
{
    [Header("Puntos por sabor incluido (solo si se dibujó bien)")]
    public int disgustaScore = -1;
    public int neutralScore = 0;
    public int gustaScore = 1;

    [Header("Precisión mínima para que un glifo cuente en la suma")]
    [Range(0f, 1f)] public float minAccuracyToCount = 0.5f;

    [Header("Umbral de reacción final (puntaje total >= umbral)")]
    public int minScoreForGusta = 1;

    // Puntos que aporta un glifo según el tier de preferencia.
    public int ScoreFor(PreferenceTier tier)
    {
        switch (tier)
        {
            case PreferenceTier.Disgusta: return disgustaScore;
            case PreferenceTier.Gusta: return gustaScore;
            default: return neutralScore;
        }
    }

    // Mapea el puntaje total del trago a la reacción final del monstruo.
    public PreferenceTier ResolveReaction(int totalScore)
    {
        if (totalScore >= minScoreForGusta) return PreferenceTier.Gusta;
        if (totalScore < 0) return PreferenceTier.Disgusta;
        return PreferenceTier.Neutral;
    }
}

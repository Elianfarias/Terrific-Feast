using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCustomer : MonoBehaviour
{
    [SerializeField] private float patienceTime = 15f;
    [SerializeField] private Slider patienceBar;

    [Header("Identidad (para GameStateService)")]
    [Tooltip("Si lo dejás vacío, se usa el nombre del GameObject.")]
    [SerializeField] private string monsterId;

    [Header("Preferencias (flujo nuevo de sabores)")]
    [SerializeField] private MonsterPreferencesRegistry preferencesRegistry;
    [SerializeField] private DrinkPreferenceRules rules;

    private MonsterFlavorPreferences preferences;
    private float timeRemaining;
    private bool waiting;

    public event Action<MonsterCustomer> OnPatienceRanOut;
    public event Action<MonsterCustomer, PreferenceTier> OnReaction;

    public string MonsterId => string.IsNullOrEmpty(monsterId) ? name : monsterId;

    // Busca las preferencias del cliente actual en el registro por su
    // monsterId, en vez de necesitar un prefab por personaje.
    private void Awake()
    {
        preferences = preferencesRegistry.GetPreferencesFor(MonsterId);
    }

    private void Update()
    {
        if (!waiting) return;

        timeRemaining -= Time.deltaTime;
        if (patienceBar != null)
            patienceBar.value = timeRemaining / patienceTime;

        if (timeRemaining <= 0f)
        {
            waiting = false;
            OnPatienceRanOut?.Invoke(this);
        }
    }

    public void Served()
    {
        waiting = false;
    }

    // Suma el puntaje de cada glifo dibujado contra las preferencias de
    // este personaje y devuelve la reacción final del trago.
    public PreferenceTier EvaluateTrago(List<DrawnFlavorResult> results)
    {
        int totalScore = 0;

        foreach (var result in results)
        {
            PreferenceTier tier = result.accuracy >= rules.minAccuracyToCount
                ? preferences.GetTier(result.glyph)
                : PreferenceTier.Disgusta;

            totalScore += rules.ScoreFor(tier);
        }

        return rules.ResolveReaction(totalScore);
    }

    public void ReactTo(PreferenceTier reaction)
    {
        waiting = false;
        OnReaction?.Invoke(this, reaction);
    }
}

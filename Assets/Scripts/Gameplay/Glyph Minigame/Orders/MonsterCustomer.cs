using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCustomer : MonoBehaviour
{
    [SerializeField] private float patienceTime = 15f;
    [SerializeField] private Slider patienceBar;

    [Header("Identidad (coincide con Personaje.cs: 0=Tartu, 1=Kerita, 2=Fue, 3=Naima)")]
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
    public MonsterFlavorPreferences Preferences => preferences;

    // Busca las preferencias del cliente actual en el registro por su
    // monsterId, en vez de necesitar un prefab por personaje.
    private void Awake()
    {
        preferences = preferencesRegistry.GetPreferencesFor(MonsterId);
    }

    // Setea qué personaje es el cliente actual (ej: desde el activeChar del
    // guardado.json) y vuelve a resolver sus preferencias.
    public void SetMonsterId(string id)
    {
        monsterId = id;
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

    // Arranca (o reinicia) la cuenta regresiva de paciencia de este cliente.
    public void StartWaiting()
    {
        timeRemaining = patienceTime;
        waiting = true;
    }

    public void Served()
    {
        waiting = false;
    }

    // Suma el puntaje del líquido principal (Wand Minigame) más el de cada
    // glifo dibujado, y devuelve la reacción final combinando ambos minijuegos.
    public PreferenceTier EvaluateTrago(List<DrawnFlavorResult> results, MazeSignData liquid = null)
    {
        int totalScore = 0;

        if (liquid != null)
            totalScore += rules.ScoreFor(preferences.GetLiquidTier(liquid));

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

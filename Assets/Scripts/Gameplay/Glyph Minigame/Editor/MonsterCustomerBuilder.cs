using UnityEditor;
using UnityEngine;

public static class MonsterCustomerBuilder
{
    private const string RegistryPath = "Assets/Data/Glyphs/Preferences/MonsterPreferencesRegistry.asset";
    private const string RulesPath = "Assets/Data/Glyphs/Preferences/DrinkPreferenceRules.asset";

    // Crea el GameStateProgress + MonsterCustomer genérico (sin visual) y los
    // conecta entre sí, con el registro de preferencias y el FlavorDrawSequencer.
    [MenuItem("Magic/Build Monster Customer")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<MonsterCustomer>() != null)
        {
            Debug.Log("Monster Customer: ya existe en la escena, no se creó nada.");
            return;
        }

        MonsterPreferencesRegistry registry = AssetDatabase.LoadAssetAtPath<MonsterPreferencesRegistry>(RegistryPath);
        DrinkPreferenceRules rules = AssetDatabase.LoadAssetAtPath<DrinkPreferenceRules>(RulesPath);

        if (registry == null || rules == null)
        {
            Debug.LogError("Monster Customer: no se encontraron el registro de preferencias y/o las reglas en las rutas esperadas.");
            return;
        }

        GameObject progressGO = new GameObject("Game State Progress", typeof(GameStateProgress));
        Undo.RegisterCreatedObjectUndo(progressGO, "Create Monster Customer");
        GameStateProgress progress = progressGO.GetComponent<GameStateProgress>();

        GameObject customerGO = new GameObject("Monster Customer", typeof(MonsterCustomer));
        Undo.RegisterCreatedObjectUndo(customerGO, "Create Monster Customer");
        MonsterCustomer customer = customerGO.GetComponent<MonsterCustomer>();

        SerializedObject customerSo = new SerializedObject(customer);
        customerSo.FindProperty("preferencesRegistry").objectReferenceValue = registry;
        customerSo.FindProperty("rules").objectReferenceValue = rules;
        customerSo.ApplyModifiedProperties();

        ActiveCustomerLoader loader = customerGO.AddComponent<ActiveCustomerLoader>();
        SerializedObject loaderSo = new SerializedObject(loader);
        loaderSo.FindProperty("progress").objectReferenceValue = progress;
        loaderSo.FindProperty("customer").objectReferenceValue = customer;
        loaderSo.ApplyModifiedProperties();

        MonsterStateRecorder recorder = customerGO.AddComponent<MonsterStateRecorder>();
        SerializedObject recorderSo = new SerializedObject(recorder);
        recorderSo.FindProperty("customer").objectReferenceValue = customer;
        recorderSo.FindProperty("progress").objectReferenceValue = progress;
        recorderSo.ApplyModifiedProperties();

        FlavorDrawSequencer sequencer = Object.FindFirstObjectByType<FlavorDrawSequencer>();
        if (sequencer != null)
        {
            SerializedObject sequencerSo = new SerializedObject(sequencer);
            sequencerSo.FindProperty("targetCustomer").objectReferenceValue = customer;
            sequencerSo.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("Monster Customer: no se encontró un FlavorDrawSequencer en la escena para conectarlo automáticamente.");
        }

        Debug.Log("Monster Customer: creado y conectado.");
    }
}

using UnityEditor;
using UnityEngine;

public static class PreloadedGlassLoaderBuilder
{
    [MenuItem("Magic/Build Preloaded Glass Loader")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<PreloadedGlassLoader>() != null)
        {
            Debug.Log("Preloaded Glass Loader: ya existe en la escena, no se creó nada.");
            return;
        }

        DrinkGlassDisplay glassDisplay = Object.FindFirstObjectByType<DrinkGlassDisplay>();
        if (glassDisplay == null)
        {
            Debug.LogError("Preloaded Glass Loader: no se encontró un DrinkGlassDisplay en la escena.");
            return;
        }

        GameObject go = new GameObject("Preloaded Glass Loader", typeof(PreloadedGlassLoader));
        Undo.RegisterCreatedObjectUndo(go, "Create Preloaded Glass Loader");

        PreloadedGlassLoader loader = go.GetComponent<PreloadedGlassLoader>();
        SerializedObject so = new SerializedObject(loader);
        so.FindProperty("glassDisplay").objectReferenceValue = glassDisplay;
        so.ApplyModifiedProperties();

        Debug.Log("Preloaded Glass Loader: creado y conectado.");
    }
}

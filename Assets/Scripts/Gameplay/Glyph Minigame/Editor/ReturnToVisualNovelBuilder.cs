using UnityEditor;
using UnityEngine;

public static class ReturnToVisualNovelBuilder
{
    // Crea el puente que vuelve a la novela visual al servir el trago.
    [MenuItem("Magic/Build Return To Visual Novel")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<ReturnToVisualNovel>() != null)
        {
            Debug.Log("Return To Visual Novel: ya existe en la escena, no se creó nada.");
            return;
        }

        FlavorDrawSequencer sequencer = Object.FindFirstObjectByType<FlavorDrawSequencer>();
        if (sequencer == null)
        {
            Debug.LogError("Return To Visual Novel: no se encontró un FlavorDrawSequencer en la escena.");
            return;
        }

        GameObject go = new GameObject("Return To Visual Novel", typeof(ReturnToVisualNovel));
        Undo.RegisterCreatedObjectUndo(go, "Create Return To Visual Novel");

        ReturnToVisualNovel bridge = go.GetComponent<ReturnToVisualNovel>();
        SerializedObject so = new SerializedObject(bridge);
        so.FindProperty("sequencer").objectReferenceValue = sequencer;
        so.ApplyModifiedProperties();

        Debug.Log("Return To Visual Novel: creado y conectado.");
    }
}

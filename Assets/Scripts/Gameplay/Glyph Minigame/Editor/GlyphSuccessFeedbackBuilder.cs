using UnityEditor;
using UnityEngine;

public static class GlyphSuccessFeedbackBuilder
{
    // Agrega CameraShake a la cámara principal y crea el GlyphSuccessFeedback.
    [MenuItem("Magic/Build Glyph Success Feedback")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<GlyphSuccessFeedback>() != null)
        {
            Debug.Log("Glyph Success Feedback: ya existe en la escena, no se creó nada.");
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Glyph Success Feedback: no se encontró una cámara principal (tag MainCamera) en la escena.");
            return;
        }

        GlyphCastController caster = Object.FindFirstObjectByType<GlyphCastController>();
        if (caster == null)
        {
            Debug.LogError("Glyph Success Feedback: no se encontró un GlyphCastController en la escena.");
            return;
        }

        CameraShake shake = mainCamera.GetComponent<CameraShake>();
        if (shake == null)
        {
            Undo.AddComponent<CameraShake>(mainCamera.gameObject);
            shake = mainCamera.GetComponent<CameraShake>();
        }

        GameObject go = new GameObject("Glyph Success Feedback", typeof(GlyphSuccessFeedback));
        Undo.RegisterCreatedObjectUndo(go, "Create Glyph Success Feedback");

        GlyphSuccessFeedback feedback = go.GetComponent<GlyphSuccessFeedback>();
        SerializedObject so = new SerializedObject(feedback);
        so.FindProperty("caster").objectReferenceValue = caster;
        so.FindProperty("cameraShake").objectReferenceValue = shake;
        so.ApplyModifiedProperties();

        Debug.Log("Glyph Success Feedback: creado y conectado. Asigná el Success Sound desde el Inspector.");
    }
}

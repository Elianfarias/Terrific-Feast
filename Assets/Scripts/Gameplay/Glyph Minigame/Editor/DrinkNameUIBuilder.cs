using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class DrinkNameUIBuilder
{
    // Crea el Canvas + Text del nombre de bebida y lo conecta al GlyphCastController.
    [MenuItem("Magic/Build Drink Name Display")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<DrinkNameDisplay>() != null)
        {
            Debug.Log("Drink Name Display: ya existe en la escena, no se creó nada.");
            return;
        }

        GlyphCastController caster = Object.FindFirstObjectByType<GlyphCastController>();
        if (caster == null)
        {
            Debug.LogError("Drink Name Display: no se encontró un GlyphCastController en la escena.");
            return;
        }

        GameObject canvasGO = new GameObject("HUD Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Drink Name Display");
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject textGO = new GameObject("Drink Name Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Drink Name Display");
        textGO.transform.SetParent(canvasGO.transform, false);

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -20f);
        rect.sizeDelta = new Vector2(500f, 60f);

        Text text = textGO.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.alignment = TextAnchor.UpperCenter;
        text.color = Color.white;
        text.text = string.Empty;

        DrinkNameDisplay display = textGO.AddComponent<DrinkNameDisplay>();
        SerializedObject so = new SerializedObject(display);
        so.FindProperty("caster").objectReferenceValue = caster;
        so.FindProperty("label").objectReferenceValue = text;
        so.ApplyModifiedProperties();

        Debug.Log("Drink Name Display: creado y conectado.");
    }
}

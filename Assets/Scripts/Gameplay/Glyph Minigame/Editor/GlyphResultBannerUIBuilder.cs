using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class GlyphResultBannerUIBuilder
{
    // Crea el Canvas + cartel de resultado y lo conecta al GlyphCastController.
    [MenuItem("Magic/Build Glyph Result Banner")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<GlyphResultBanner>() != null)
        {
            Debug.Log("Glyph Result Banner: ya existe en la escena, no se creó nada.");
            return;
        }

        GlyphCastController caster = Object.FindFirstObjectByType<GlyphCastController>();
        if (caster == null)
        {
            Debug.LogError("Glyph Result Banner: no se encontró un GlyphCastController en la escena.");
            return;
        }

        GameObject canvasGO = new GameObject("Glyph Result Canvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Glyph Result Banner");
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject panelGO = new GameObject("Result Banner",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        Undo.RegisterCreatedObjectUndo(panelGO, "Create Glyph Result Banner");
        panelGO.transform.SetParent(canvasGO.transform, false);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0f, -220f);
        panelRect.sizeDelta = new Vector2(420f, 90f);

        Image panelImage = panelGO.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.65f);

        CanvasGroup group = panelGO.GetComponent<CanvasGroup>();
        group.alpha = 0f;

        GameObject textGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Glyph Result Banner");
        textGO.transform.SetParent(panelGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text label = textGO.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 28;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = string.Empty;

        GlyphResultBanner banner = panelGO.AddComponent<GlyphResultBanner>();
        SerializedObject so = new SerializedObject(banner);
        so.FindProperty("caster").objectReferenceValue = caster;
        so.FindProperty("group").objectReferenceValue = group;
        so.FindProperty("label").objectReferenceValue = label;
        so.ApplyModifiedProperties();

        Debug.Log("Glyph Result Banner: creado y conectado.");
    }
}

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class GlyphHintDisplayBuilder
{
    private const string TagSpritePath = "Assets/Art/Sprites/Menu/etiqueta izq.png";

    // Crea el panel de pista (oculto por defecto) y lo conecta al
    // GlyphCastController de la escena.
    [MenuItem("Magic/Build Glyph Hint Display")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<GlyphHintDisplay>() != null)
        {
            Debug.Log("Glyph Hint Display: ya existe en la escena, no se creó nada.");
            return;
        }

        MonsterCustomer customer = Object.FindFirstObjectByType<MonsterCustomer>();
        if (customer == null)
        {
            Debug.LogError("Glyph Hint Display: no se encontró un MonsterCustomer en la escena.");
            return;
        }

        Sprite tagSprite = AssetDatabase.LoadAssetAtPath<Sprite>(TagSpritePath);
        if (tagSprite == null)
        {
            Debug.LogError($"Glyph Hint Display: no se encontró el sprite en \"{TagSpritePath}\".");
            return;
        }

        GameObject canvasGO = new GameObject("Glyph Hint Canvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Glyph Hint Display");
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject panelGO = new GameObject("Hint Panel",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        Undo.RegisterCreatedObjectUndo(panelGO, "Create Glyph Hint Display");
        panelGO.transform.SetParent(canvasGO.transform, false);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0.5f);
        panelRect.anchorMax = new Vector2(0f, 0.5f);
        panelRect.pivot = new Vector2(0f, 0.5f);
        panelRect.anchoredPosition = new Vector2(30f, 0f);
        panelRect.sizeDelta = new Vector2(400f, 340f);

        Image panelImage = panelGO.GetComponent<Image>();
        panelImage.sprite = tagSprite;
        panelImage.type = Image.Type.Simple;
        panelImage.preserveAspect = true;

        CanvasGroup group = panelGO.GetComponent<CanvasGroup>();

        GameObject textGO = new GameObject("Hint Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Glyph Hint Display");
        textGO.transform.SetParent(panelGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.15f);
        textRect.anchorMax = new Vector2(0.85f, 0.75f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text label = textGO.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 22;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.black;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Overflow;

        GlyphHintDisplay display = canvasGO.AddComponent<GlyphHintDisplay>();
        SerializedObject so = new SerializedObject(display);
        so.FindProperty("customer").objectReferenceValue = customer;
        so.FindProperty("panel").objectReferenceValue = panelRect;
        so.FindProperty("group").objectReferenceValue = group;
        so.FindProperty("hintText").objectReferenceValue = label;
        so.ApplyModifiedProperties();

        panelGO.SetActive(false);

        Debug.Log("Glyph Hint Display: creado. Conectá el botón de ayuda a GlyphHintDisplay.ShowHint() desde su OnClick, y completá el campo \"Pista\" en cada Monster Flavor Preferences.");
    }
}

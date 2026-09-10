using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class FlavorSelectionUIBuilder
{
    private const string GlyphsFolder = "Assets/Data/Glyphs";

    // Crea la grilla de sabores, el Center Target y la capa de vuelo, y las
    // conecta a un FlavorSelectionController nuevo.
    [MenuItem("Magic/Build Flavor Selection UI")]
    public static void Build()
    {
        EnsureEventSystem();

        if (Object.FindFirstObjectByType<FlavorSelectionController>() != null)
        {
            Debug.Log("Flavor Selection UI: ya existe en la escena, no se creó nada.");
            return;
        }

        List<DrawPattern> flavors = LoadFlavors();
        if (flavors.Count == 0)
        {
            Debug.LogError($"Flavor Selection UI: no se encontraron DrawPattern en {GlyphsFolder}.");
            return;
        }

        GameObject canvasGO = new GameObject("Flavor Selection Canvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Flavor Selection UI");

        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject grid = new GameObject("Flavor Grid", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        Undo.RegisterCreatedObjectUndo(grid, "Create Flavor Selection UI");
        grid.transform.SetParent(canvasGO.transform, false);

        RectTransform gridRect = grid.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.5f, 1f);
        gridRect.anchorMax = new Vector2(0.5f, 1f);
        gridRect.pivot = new Vector2(0.5f, 1f);
        gridRect.anchoredPosition = new Vector2(0f, -40f);
        gridRect.sizeDelta = new Vector2(200f * flavors.Count, 220f);

        HorizontalLayoutGroup layout = grid.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        var slots = new List<FlavorSlotView>();
        foreach (var flavor in flavors)
            slots.Add(BuildSlot(grid.transform, flavor));

        RectTransform centerTarget = BuildCenterTarget(canvasGO.transform);
        RectTransform flightLayer = BuildFlightLayer(canvasGO.transform);

        FlavorSelectionController controller = canvasGO.AddComponent<FlavorSelectionController>();
        SerializedObject controllerSO = new SerializedObject(controller);

        SerializedProperty slotsProp = controllerSO.FindProperty("slots");
        slotsProp.arraySize = slots.Count;
        for (int i = 0; i < slots.Count; i++)
            slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];

        controllerSO.FindProperty("flightParent").objectReferenceValue = flightLayer;
        controllerSO.FindProperty("centerTarget").objectReferenceValue = centerTarget;
        controllerSO.ApplyModifiedProperties();

        Debug.Log($"Flavor Selection UI: creada con {slots.Count} sabores.");
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;

        GameObject eventSystemGO = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create Flavor Selection UI");

        Debug.Log("Flavor Selection UI: no había EventSystem en la escena, se creó uno.");
    }

    private static List<DrawPattern> LoadFlavors()
    {
        string[] guids = AssetDatabase.FindAssets("t:DrawPattern", new[] { GlyphsFolder });
        return guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<DrawPattern>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(pattern => pattern != null)
            .OrderBy(pattern => pattern.glyphName)
            .ToList();
    }

    private static FlavorSlotView BuildSlot(Transform parent, DrawPattern flavor)
    {
        GameObject slotGO = new GameObject($"Slot - {flavor.glyphName}",
            typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(CanvasGroup));
        Undo.RegisterCreatedObjectUndo(slotGO, "Create Flavor Selection UI");
        slotGO.transform.SetParent(parent, false);
        slotGO.GetComponent<RectTransform>().sizeDelta = new Vector2(180f, 220f);

        VerticalLayoutGroup slotLayout = slotGO.GetComponent<VerticalLayoutGroup>();
        slotLayout.spacing = 6f;
        slotLayout.childAlignment = TextAnchor.UpperCenter;
        slotLayout.childControlWidth = false;
        slotLayout.childControlHeight = false;

        Text nameText = CreateLabel(slotGO.transform, flavor.glyphName);

        GameObject buttonGO = new GameObject("Glyph Button",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        Undo.RegisterCreatedObjectUndo(buttonGO, "Create Flavor Selection UI");
        buttonGO.transform.SetParent(slotGO.transform, false);
        buttonGO.GetComponent<RectTransform>().sizeDelta = new Vector2(140f, 140f);

        Image glyphImage = buttonGO.GetComponent<Image>();
        glyphImage.color = new Color(1f, 1f, 1f, 0.15f);

        Button glyphButton = buttonGO.GetComponent<Button>();
        glyphButton.transition = Selectable.Transition.None;
        glyphButton.navigation = new Navigation { mode = Navigation.Mode.None };

        FlavorSlotView slotView = slotGO.AddComponent<FlavorSlotView>();
        SerializedObject slotSO = new SerializedObject(slotView);
        slotSO.FindProperty("flavor").objectReferenceValue = flavor;
        slotSO.FindProperty("button").objectReferenceValue = glyphButton;
        slotSO.FindProperty("glyphIcon").objectReferenceValue = glyphImage;
        slotSO.FindProperty("nameLabel").objectReferenceValue = nameText;
        slotSO.FindProperty("slotGroup").objectReferenceValue = slotGO.GetComponent<CanvasGroup>();
        slotSO.ApplyModifiedProperties();

        return slotView;
    }

    private static RectTransform BuildCenterTarget(Transform canvasParent)
    {
        GameObject centerGO = new GameObject("Center Target", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(centerGO, "Create Flavor Selection UI");
        centerGO.transform.SetParent(canvasParent, false);

        RectTransform rect = centerGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        return rect;
    }

    private static RectTransform BuildFlightLayer(Transform canvasParent)
    {
        GameObject layerGO = new GameObject("Flight Layer", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(layerGO, "Create Flavor Selection UI");
        layerGO.transform.SetParent(canvasParent, false);
        layerGO.transform.SetAsLastSibling();

        RectTransform rect = layerGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return rect;
    }

    private static Text CreateLabel(Transform parent, string text)
    {
        GameObject textGO = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Flavor Selection UI");
        textGO.transform.SetParent(parent, false);
        textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(180f, 24f);

        Text label = textGO.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 16;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = text;

        return label;
    }
}

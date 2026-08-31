using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public static class FlavorDrawSequencerBuilder
{
    private const string RecipesFolder = "Assets/Data/Recipes";

    // Crea el FlavorDrawSequencer y lo conecta a todo lo que necesita en la
    // escena, más el botón "Servir" y el texto de ayuda.
    [MenuItem("Magic/Build Flavor Draw Sequencer")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<FlavorDrawSequencer>() != null)
        {
            Debug.Log("Flavor Draw Sequencer: ya existe en la escena, no se creó nada.");
            return;
        }

        FlavorSelectionController selection = Object.FindFirstObjectByType<FlavorSelectionController>();
        GlyphCastController caster = Object.FindFirstObjectByType<GlyphCastController>();
        SoulCreatureView creature = Object.FindFirstObjectByType<SoulCreatureView>();
        GlyphResultBanner resultBanner = Object.FindFirstObjectByType<GlyphResultBanner>();

        if (selection == null || caster == null || creature == null || resultBanner == null)
        {
            Debug.LogError("Flavor Draw Sequencer: falta FlavorSelectionController, GlyphCastController, SoulCreatureView y/o GlyphResultBanner en la escena.");
            return;
        }

        List<DrinkRecipe> recipes = LoadRecipes();
        if (recipes.Count == 0)
        {
            Debug.LogError($"Flavor Draw Sequencer: no se encontraron DrinkRecipe en {RecipesFolder}.");
            return;
        }

        MonsterCustomer targetCustomer = Object.FindFirstObjectByType<MonsterCustomer>();
        if (targetCustomer == null)
            Debug.LogWarning("Flavor Draw Sequencer: no hay MonsterCustomer en la escena, se crea sin conectar reacción todavía.");

        SlideDownIntro tablaIntro = Object.FindFirstObjectByType<SlideDownIntro>();
        if (tablaIntro == null)
            Debug.LogWarning("Flavor Draw Sequencer: no hay SlideDownIntro (Tabla) en la escena, la criatura va a aparecer directo al arrancar.");

        GameObject go = new GameObject("Flavor Draw Sequencer", typeof(FlavorDrawSequencer));
        Undo.RegisterCreatedObjectUndo(go, "Create Flavor Draw Sequencer");

        FlavorDrawSequencer sequencer = go.GetComponent<FlavorDrawSequencer>();
        SerializedObject so = new SerializedObject(sequencer);
        so.FindProperty("selection").objectReferenceValue = selection;
        so.FindProperty("caster").objectReferenceValue = caster;
        so.FindProperty("creature").objectReferenceValue = creature;
        so.FindProperty("resultBanner").objectReferenceValue = resultBanner;
        so.FindProperty("tablaIntro").objectReferenceValue = tablaIntro;
        so.FindProperty("targetCustomer").objectReferenceValue = targetCustomer;

        SerializedProperty catalogProp = so.FindProperty("recipeCatalog");
        catalogProp.arraySize = recipes.Count;
        for (int i = 0; i < recipes.Count; i++)
            catalogProp.GetArrayElementAtIndex(i).objectReferenceValue = recipes[i];

        so.ApplyModifiedProperties();

        BuildServeButton(selection.transform, sequencer);
        BuildHintLabel(selection.transform);

        Debug.Log($"Flavor Draw Sequencer: creado con {recipes.Count} recetas.");
    }

    private static void BuildHintLabel(Transform selectionCanvas)
    {
        if (selectionCanvas.Find("Hint Label") != null) return;

        GameObject textGO = new GameObject("Hint Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Hint Label");
        textGO.transform.SetParent(selectionCanvas, false);

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -260f);
        rect.sizeDelta = new Vector2(700f, 40f);

        Text label = textGO.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 18;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = "Seleccioná un glifo y presioná Enter para continuar, o Servir el trago";
    }

    private static void BuildServeButton(Transform selectionCanvas, FlavorDrawSequencer sequencer)
    {
        if (selectionCanvas.Find("Serve Button") != null) return;

        GameObject buttonGO = new GameObject("Serve Button",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(ButtonHoverScale));
        Undo.RegisterCreatedObjectUndo(buttonGO, "Create Serve Button");
        buttonGO.transform.SetParent(selectionCanvas, false);

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -300f);
        rect.sizeDelta = new Vector2(160f, 50f);

        Button button = buttonGO.GetComponent<Button>();
        button.transition = Selectable.Transition.None;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        GameObject textGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Serve Button");
        textGO.transform.SetParent(buttonGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text label = textGO.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 20;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = "Servir";

        UnityEventTools.AddPersistentListener(button.onClick, sequencer.RequestServe);
    }

    private static List<DrinkRecipe> LoadRecipes()
    {
        string[] guids = AssetDatabase.FindAssets("t:DrinkRecipe", new[] { RecipesFolder });
        return guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<DrinkRecipe>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(recipe => recipe != null)
            .ToList();
    }
}

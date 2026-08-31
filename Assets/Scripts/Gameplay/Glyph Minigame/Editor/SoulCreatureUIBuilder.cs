using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SoulCreatureUIBuilder
{
    private const string AliveSpritePath = "Assets/Art/Sprites/butterfly/libelula_viva.png";
    private const string DeadSpritePath = "Assets/Art/Sprites/butterfly/libelula_muerta.png";

    // Crea el Canvas + imagen de la criatura y la conecta a SoulEnergy.
    [MenuItem("Magic/Build Soul Creature Display")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<SoulCreatureView>() != null)
        {
            Debug.Log("Soul Creature Display: ya existe en la escena, no se creó nada.");
            return;
        }

        SoulEnergy soul = Object.FindFirstObjectByType<SoulEnergy>();
        if (soul == null)
        {
            Debug.LogError("Soul Creature Display: no se encontró SoulEnergy en la escena.");
            return;
        }

        Sprite aliveSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AliveSpritePath);
        Sprite deadSprite = AssetDatabase.LoadAssetAtPath<Sprite>(DeadSpritePath);
        if (aliveSprite == null || deadSprite == null)
        {
            Debug.LogError($"Soul Creature Display: no se encontraron los sprites en {AliveSpritePath} / {DeadSpritePath}.");
            return;
        }

        GameObject canvasGO = new GameObject("Soul Creature Canvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Soul Creature Display");
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject imageGO = new GameObject("Creature Image",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        Undo.RegisterCreatedObjectUndo(imageGO, "Create Soul Creature Display");
        imageGO.transform.SetParent(canvasGO.transform, false);

        RectTransform rect = imageGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(120f, 120f);

        Image image = imageGO.GetComponent<Image>();
        image.sprite = aliveSprite;
        image.preserveAspect = true;

        CanvasGroup group = imageGO.GetComponent<CanvasGroup>();
        group.alpha = 0f;

        SoulCreatureView view = imageGO.AddComponent<SoulCreatureView>();
        SerializedObject so = new SerializedObject(view);
        so.FindProperty("soul").objectReferenceValue = soul;
        so.FindProperty("image").objectReferenceValue = image;
        so.FindProperty("creatureGroup").objectReferenceValue = group;
        so.FindProperty("aliveSprite").objectReferenceValue = aliveSprite;
        so.FindProperty("deadSprite").objectReferenceValue = deadSprite;
        so.ApplyModifiedProperties();

        Debug.Log("Soul Creature Display: creada y conectada.");
    }
}

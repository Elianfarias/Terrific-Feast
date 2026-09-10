using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public static class TutorialPanelBuilder
{
    private const string BoardSpritePath = "Assets/Art/Sprites/UI/Transparent center/UI board Medium  parchment.png";
    private const string CloseSpritePath = "Assets/Art/Sprites/UI/Transparent center/boton con x.png";
    private const string NextSpritePath = "Assets/Art/Sprites/UI/Transparent center/next boton.png";

    // Crea el botón "?" y el panel de tutorial (oculto por defecto) y los conecta.
    [MenuItem("Magic/Build Tutorial Panel")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<TutorialPanelController>() != null)
        {
            Debug.Log("Tutorial Panel: ya existe en la escena, no se creó nada.");
            return;
        }

        Sprite boardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BoardSpritePath);
        Sprite closeSprite = AssetDatabase.LoadAssetAtPath<Sprite>(CloseSpritePath);
        Sprite nextSprite = AssetDatabase.LoadAssetAtPath<Sprite>(NextSpritePath);

        if (boardSprite == null || closeSprite == null || nextSprite == null)
        {
            Debug.LogError("Tutorial Panel: no se encontraron los sprites de panel/cerrar/continuar en las rutas esperadas.");
            return;
        }

        GameObject canvasGO = new GameObject("Tutorial Canvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Tutorial Panel");
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject panelRoot = BuildPanel(canvasGO.transform, boardSprite, closeSprite, nextSprite);
        BuildOpenButton(canvasGO.transform, panelRoot.GetComponent<TutorialPanelController>());

        Debug.Log("Tutorial Panel: creado y conectado.");
    }

    private static GameObject BuildOpenButton(Transform parent, TutorialPanelController controller)
    {
        GameObject buttonGO = new GameObject("Open Tutorial Button",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(ButtonHoverScale));
        Undo.RegisterCreatedObjectUndo(buttonGO, "Create Tutorial Panel");
        buttonGO.transform.SetParent(parent, false);

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-30f, -30f);
        rect.sizeDelta = new Vector2(60f, 60f);

        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.color = new Color(0f, 0f, 0f, 0.65f);

        Button button = buttonGO.GetComponent<Button>();
        button.transition = Selectable.Transition.None;

        GameObject labelGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(labelGO, "Create Tutorial Panel");
        labelGO.transform.SetParent(buttonGO.transform, false);

        RectTransform labelRect = labelGO.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text label = labelGO.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 32;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = "?";

        UnityEventTools.AddPersistentListener(button.onClick, controller.Show);

        return buttonGO;
    }

    private static GameObject BuildPanel(Transform parent, Sprite boardSprite, Sprite closeSprite, Sprite nextSprite)
    {
        GameObject panelGO = new GameObject("Tutorial Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(panelGO, "Create Tutorial Panel");
        panelGO.transform.SetParent(parent, false);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image dimBackground = panelGO.GetComponent<Image>();
        dimBackground.color = new Color(0f, 0f, 0f, 0.6f);

        GameObject boxGO = new GameObject("Box", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(boxGO, "Create Tutorial Panel");
        boxGO.transform.SetParent(panelGO.transform, false);

        RectTransform boxRect = boxGO.GetComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0.5f, 0.5f);
        boxRect.anchorMax = new Vector2(0.5f, 0.5f);
        boxRect.pivot = new Vector2(0.5f, 0.5f);
        boxRect.sizeDelta = new Vector2(700f, 490f);

        Image boxImage = boxGO.GetComponent<Image>();
        boxImage.sprite = boardSprite;
        boxImage.type = Image.Type.Simple;
        boxImage.preserveAspect = true;

        Text title = BuildTitle(boxGO.transform);
        Text body = BuildBody(boxGO.transform);

        TutorialPanelController controller = panelGO.AddComponent<TutorialPanelController>();

        GameObject nextButtonGO = BuildNextButton(boxGO.transform, controller, nextSprite);
        BuildCloseButton(boxGO.transform, controller, closeSprite);

        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("panelRoot").objectReferenceValue = panelGO;
        so.FindProperty("titleText").objectReferenceValue = title;
        so.FindProperty("bodyText").objectReferenceValue = body;
        so.FindProperty("nextButtonRoot").objectReferenceValue = nextButtonGO;
        so.ApplyModifiedProperties();

        // El panel arranca oculto desde acá (no desde Awake: panelRoot es
        // este mismo objeto, y auto-desactivarse en su propio Awake generaba
        // que la primera apertura se cancelara sola).
        panelGO.SetActive(false);

        return panelGO;
    }

    private static Text BuildTitle(Transform parent)
    {
        GameObject titleGO = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(titleGO, "Create Tutorial Panel");
        titleGO.transform.SetParent(parent, false);

        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -50f);
        titleRect.sizeDelta = new Vector2(-120f, 50f);

        Text title = titleGO.GetComponent<Text>();
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 30;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.black;

        return title;
    }

    private static Text BuildBody(Transform parent)
    {
        GameObject bodyGO = new GameObject("Body", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(bodyGO, "Create Tutorial Panel");
        bodyGO.transform.SetParent(parent, false);

        RectTransform bodyRect = bodyGO.GetComponent<RectTransform>();
        bodyRect.anchorMin = new Vector2(0f, 0f);
        bodyRect.anchorMax = new Vector2(1f, 1f);
        bodyRect.offsetMin = new Vector2(60f, 100f);
        bodyRect.offsetMax = new Vector2(-60f, -110f);

        Text body = bodyGO.GetComponent<Text>();
        body.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        body.fontSize = 22;
        body.alignment = TextAnchor.UpperLeft;
        body.color = Color.black;

        return body;
    }

    private static GameObject BuildNextButton(Transform parent, TutorialPanelController controller, Sprite sprite)
    {
        GameObject buttonGO = new GameObject("Next Button",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(ButtonHoverScale));
        Undo.RegisterCreatedObjectUndo(buttonGO, "Create Tutorial Panel");
        buttonGO.transform.SetParent(parent, false);

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-40f, 30f);
        rect.sizeDelta = new Vector2(70f, 65f);

        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.sprite = sprite;
        buttonImage.preserveAspect = true;

        Button button = buttonGO.GetComponent<Button>();
        button.transition = Selectable.Transition.None;

        UnityEventTools.AddPersistentListener(button.onClick, controller.Next);

        return buttonGO;
    }

    private static void BuildCloseButton(Transform parent, TutorialPanelController controller, Sprite sprite)
    {
        GameObject buttonGO = new GameObject("Close Button",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(ButtonHoverScale));
        Undo.RegisterCreatedObjectUndo(buttonGO, "Create Tutorial Panel");
        buttonGO.transform.SetParent(parent, false);

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-15f, -15f);
        rect.sizeDelta = new Vector2(50f, 46f);

        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.sprite = sprite;
        buttonImage.preserveAspect = true;

        Button button = buttonGO.GetComponent<Button>();
        button.transition = Selectable.Transition.None;

        UnityEventTools.AddPersistentListener(button.onClick, controller.Hide);
    }
}

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class GameOverPanelBuilder
{
    // Crea (o actualiza) el cartel "Has Muerto" en la novela visual y lo
    // conecta al YarnComands de la escena.
    [MenuItem("Magic/Build Visual Novel Game Over")]
    public static void Build()
    {
        YarnComands yarnComands = Object.FindFirstObjectByType<YarnComands>();
        if (yarnComands == null)
        {
            Debug.LogError("Game Over: no se encontró un YarnComands en la escena.");
            return;
        }

        GameObject canvasGO = GameObject.Find("Game Over Canvas");
        if (canvasGO == null)
        {
            canvasGO = new GameObject("Game Over Canvas",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Game Over");
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
        }

        Transform existingPanel = canvasGO.transform.Find("Death Panel");
        GameObject panel = existingPanel != null ? existingPanel.gameObject : BuildPanel(canvasGO.transform);

        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group == null)
            group = Undo.AddComponent<CanvasGroup>(panel);

        panel.SetActive(false);

        SerializedObject so = new SerializedObject(yarnComands);
        so.FindProperty("gameOverPanel").objectReferenceValue = panel;
        so.FindProperty("gameOverGroup").objectReferenceValue = group;
        so.ApplyModifiedProperties();

        Debug.Log("Game Over: creado/actualizado y conectado a YarnComands.");
    }

    private static GameObject BuildPanel(Transform parent)
    {
        GameObject panelGO = new GameObject("Death Panel",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        Undo.RegisterCreatedObjectUndo(panelGO, "Create Game Over");
        panelGO.transform.SetParent(parent, false);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image background = panelGO.GetComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.92f);

        GameObject textGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Game Over");
        textGO.transform.SetParent(panelGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(1200f, 200f);

        Text label = textGO.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 90;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color(0.65f, 0.1f, 0.1f);
        label.text = "HAS MUERTO";

        return panelGO;
    }
}

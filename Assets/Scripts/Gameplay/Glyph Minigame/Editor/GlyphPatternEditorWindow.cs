using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GlyphPatternEditorWindow : EditorWindow
{
    private DrawPattern targetPattern;
    private GlyphReferenceDisplay sceneReference;

    private Vector2 worldOrigin = Vector2.zero;
    private float worldWidth = 8f;
    private float worldHeight = 8f;
    private float defaultRadius = 0.4f;

    private readonly List<Vector2> normalizedPoints = new List<Vector2>();

    private Vector2 scrollPosition;
    private const float MaxPreviewHeight = 500f;

    [MenuItem("Magic/Glyph Pattern Editor")]
    public static void Open()
    {
        GetWindow<GlyphPatternEditorWindow>("Glyph Pattern Editor");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("1. Patrón destino", EditorStyles.boldLabel);
        targetPattern = (DrawPattern)EditorGUILayout.ObjectField(
            "Draw Pattern", targetPattern, typeof(DrawPattern), false);

        Sprite referenceSprite = targetPattern != null ? targetPattern.referenceSprite : null;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("2. Mapeo a coordenadas de mundo", EditorStyles.boldLabel);
        sceneReference = (GlyphReferenceDisplay)EditorGUILayout.ObjectField(
            "Referencia en escena", sceneReference, typeof(GlyphReferenceDisplay), true);

        GUI.enabled = sceneReference != null;
        if (GUILayout.Button("Autocompletar desde escena"))
            AutoDetectFromScene();
        GUI.enabled = true;

        worldOrigin = EditorGUILayout.Vector2Field("Origen (esquina inferior izq.)", worldOrigin);
        worldWidth = EditorGUILayout.FloatField("Ancho en unidades", worldWidth);
        worldHeight = EditorGUILayout.FloatField("Alto en unidades", worldHeight);
        defaultRadius = EditorGUILayout.FloatField("Radio por defecto", defaultRadius);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Click izquierdo sobre la imagen agrega un nodo EN ORDEN.\n" +
            "Click derecho elimina el último nodo agregado.\n" +
            "La imagen se muestra recortada igual que el Sprite en el juego (mismo bounds).",
            MessageType.Info);

        if (referenceSprite != null)
            DrawImageArea(referenceSprite);
        else
            EditorGUILayout.HelpBox(
                "Asigná un Draw Pattern con Reference Sprite para poder marcar los nodos.",
                MessageType.Warning);

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Deshacer último"))
                UndoLastPoint();

            if (GUILayout.Button("Limpiar todo"))
                normalizedPoints.Clear();

            GUI.enabled = targetPattern != null && normalizedPoints.Count > 0;
            if (GUILayout.Button("Guardar en Draw Pattern"))
                SaveToPattern();
            GUI.enabled = true;

            GUI.enabled = targetPattern != null && targetPattern.nodes.Count > 0;
            if (GUILayout.Button("Cargar desde Draw Pattern"))
                LoadFromPattern();
            GUI.enabled = true;
        }

        EditorGUILayout.LabelField($"Nodos colocados: {normalizedPoints.Count}");

        EditorGUILayout.EndScrollView();
    }

    // Dibuja solo el recorte real del Sprite (su rect en la textura), igual que se ve en el juego.
    private void DrawImageArea(Sprite sprite)
    {
        Rect spriteRect = sprite.rect;
        float aspect = spriteRect.height / spriteRect.width;

        float availableWidth = EditorGUIUtility.currentViewWidth - 20f;
        float height = Mathf.Min(availableWidth * aspect, MaxPreviewHeight);
        float width = height / aspect;
        Rect rect = GUILayoutUtility.GetRect(width, height);

        Texture2D texture = sprite.texture;
        Rect uv = new Rect(
            spriteRect.x / texture.width,
            spriteRect.y / texture.height,
            spriteRect.width / texture.width,
            spriteRect.height / texture.height);

        GUI.DrawTextureWithTexCoords(rect, texture, uv);
        HandleMouseInput(rect);
        DrawPointsOverlay(rect);
    }

    private void HandleMouseInput(Rect rect)
    {
        Event e = Event.current;
        if (!rect.Contains(e.mousePosition)) return;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 normalized = new Vector2(
                (e.mousePosition.x - rect.x) / rect.width,
                (e.mousePosition.y - rect.y) / rect.height);

            normalizedPoints.Add(normalized);
            e.Use();
            Repaint();
        }
        else if (e.type == EventType.MouseDown && e.button == 1)
        {
            UndoLastPoint();
            e.Use();
            Repaint();
        }
    }

    private void DrawPointsOverlay(Rect rect)
    {
        Handles.BeginGUI();

        for (int i = 0; i < normalizedPoints.Count; i++)
        {
            Vector2 screenPos = new Vector2(
                rect.x + normalizedPoints[i].x * rect.width,
                rect.y + normalizedPoints[i].y * rect.height);

            if (i > 0)
            {
                Vector2 prevScreenPos = new Vector2(
                    rect.x + normalizedPoints[i - 1].x * rect.width,
                    rect.y + normalizedPoints[i - 1].y * rect.height);
                Handles.color = Color.yellow;
                Handles.DrawLine(prevScreenPos, screenPos);
            }

            Handles.color = Color.cyan;
            Handles.DrawSolidDisc(screenPos, Vector3.forward, 5f);
            GUI.Label(new Rect(screenPos.x + 6, screenPos.y - 8, 30, 20), i.ToString());
        }

        Handles.EndGUI();
    }

    // Deriva Origen/Ancho/Alto del objeto real de la escena en vez de tipearlos a mano.
    private void AutoDetectFromScene()
    {
        var serialized = new SerializedObject(sceneReference);
        Vector2 targetWorldSize = serialized.FindProperty("targetWorldSize").vector2Value;

        Vector3 center = sceneReference.transform.position;
        worldWidth = targetWorldSize.x;
        worldHeight = targetWorldSize.y;
        worldOrigin = new Vector2(center.x - worldWidth / 2f, center.y - worldHeight / 2f);
    }

    private void UndoLastPoint()
    {
        if (normalizedPoints.Count > 0)
            normalizedPoints.RemoveAt(normalizedPoints.Count - 1);
    }

    // Convierte los puntos normalizados a coordenadas de mundo y los guarda.
    private void SaveToPattern()
    {
        targetPattern.nodes.Clear();

        foreach (var norm in normalizedPoints)
        {
            // Y invertida: la imagen crece hacia abajo, el mundo hacia arriba.
            Vector2 worldPos = new Vector2(
                worldOrigin.x + norm.x * worldWidth,
                worldOrigin.y + (1f - norm.y) * worldHeight);

            targetPattern.nodes.Add(new PatternNode { position = worldPos, radius = defaultRadius });
        }

        EditorUtility.SetDirty(targetPattern);
        AssetDatabase.SaveAssets();
        Debug.Log($"Guardados {normalizedPoints.Count} nodos en {targetPattern.name}");
    }

    private void LoadFromPattern()
    {
        normalizedPoints.Clear();

        foreach (var node in targetPattern.nodes)
        {
            Vector2 norm = new Vector2(
                (node.position.x - worldOrigin.x) / worldWidth,
                1f - (node.position.y - worldOrigin.y) / worldHeight);

            normalizedPoints.Add(norm);
        }
    }
}

using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BackgroundBuilder
{
    private const string BackgroundSpritePath = "Assets/Art/Background/madera.jpeg";
    private const float BackgroundZ = 10f;
    private const int SortingOrder = -100;

    // Crea el fondo fijo, centrado y escalado para cubrir la cámara.
    [MenuItem("Magic/Build Background")]
    public static void Build()
    {
        if (Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Any(t => t.name == "Background"))
        {
            Debug.Log("Background: ya existe en la escena, no se creó nada.");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Background: no se encontró una cámara con tag MainCamera en la escena.");
            return;
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundSpritePath);
        if (sprite == null)
        {
            Debug.LogError($"Background: no se encontró un Sprite en {BackgroundSpritePath}.");
            return;
        }

        GameObject backgroundGO = new GameObject("Background", typeof(SpriteRenderer));
        Undo.RegisterCreatedObjectUndo(backgroundGO, "Create Background");

        SpriteRenderer spriteRenderer = backgroundGO.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = SortingOrder;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        float scale = Mathf.Max(camWidth / spriteWidth, camHeight / spriteHeight);
        backgroundGO.transform.localScale = new Vector3(scale, scale, 1f);

        Vector3 camPos = cam.transform.position;
        backgroundGO.transform.position = new Vector3(camPos.x, camPos.y, BackgroundZ);

        Debug.Log("Background: creado y ajustado a la cámara.");
    }
}

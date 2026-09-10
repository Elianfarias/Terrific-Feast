using System.Linq;
using UnityEditor;
using UnityEngine;

public static class TablaBuilder
{
    private const string TablaSpritePath = "Assets/Art/Background/tablapng.png";
    private const float TablaZ = 9f;
    private const int SortingOrder = -50;

    // Crea la tabla animada (SlideDownIntro), escalada para cubrir la cámara.
    [MenuItem("Magic/Build Tabla")]
    public static void Build()
    {
        if (Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Any(t => t.name == "Tabla"))
        {
            Debug.Log("Tabla: ya existe en la escena, no se creó nada.");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Tabla: no se encontró una cámara con tag MainCamera en la escena.");
            return;
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(TablaSpritePath);
        if (sprite == null)
        {
            Debug.LogError($"Tabla: no se encontró un Sprite en {TablaSpritePath}.");
            return;
        }

        GameObject tablaGO = new GameObject("Tabla", typeof(SpriteRenderer), typeof(SlideDownIntro));
        Undo.RegisterCreatedObjectUndo(tablaGO, "Create Tabla");

        SpriteRenderer spriteRenderer = tablaGO.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = SortingOrder;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        float scale = Mathf.Max(camWidth / spriteWidth, camHeight / spriteHeight);
        tablaGO.transform.localScale = new Vector3(scale, scale, 1f);

        Vector3 camPos = cam.transform.position;
        tablaGO.transform.position = new Vector3(camPos.x, camPos.y, TablaZ);

        SerializedObject introSO = new SerializedObject(tablaGO.GetComponent<SlideDownIntro>());
        introSO.FindProperty("dropDistance").floatValue = camHeight;
        introSO.ApplyModifiedProperties();

        Debug.Log("Tabla: creada, ajustada a la cámara y con entrada animada.");
    }
}

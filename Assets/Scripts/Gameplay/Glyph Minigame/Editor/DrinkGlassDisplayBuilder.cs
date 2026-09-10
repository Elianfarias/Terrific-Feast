using UnityEditor;
using UnityEngine;

public static class DrinkGlassDisplayBuilder
{
    private const string GlassObjectName = "vaso vacio_0";
    private const string DefaultSpritePath = "Assets/Art/Sprites/Glyphs/vaso vacio.png";

    // Conecta el DrinkGlassDisplay al vaso que ya está en la escena.
    [MenuItem("Magic/Wire Drink Glass Display")]
    public static void Build()
    {
        GameObject glassGO = GameObject.Find(GlassObjectName);
        if (glassGO == null)
        {
            Debug.LogError($"Drink Glass Display: no se encontró el objeto \"{GlassObjectName}\" en la escena.");
            return;
        }

        SpriteRenderer spriteRenderer = glassGO.GetComponent<SpriteRenderer>();
        Sprite defaultSprite = AssetDatabase.LoadAssetAtPath<Sprite>(DefaultSpritePath);

        if (defaultSprite == null)
        {
            Debug.LogError("Drink Glass Display: no se encontró el sprite de vaso vacío en la ruta esperada.");
            return;
        }

        DrinkGlassDisplay display = glassGO.GetComponent<DrinkGlassDisplay>();
        if (display == null)
        {
            Undo.AddComponent<DrinkGlassDisplay>(glassGO);
            display = glassGO.GetComponent<DrinkGlassDisplay>();
        }

        SerializedObject so = new SerializedObject(display);
        so.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
        so.FindProperty("defaultSprite").objectReferenceValue = defaultSprite;
        so.ApplyModifiedProperties();

        Debug.Log("Drink Glass Display: conectado a \"" + GlassObjectName + "\".");
    }
}

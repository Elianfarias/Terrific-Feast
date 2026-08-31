using UnityEditor;
using UnityEngine;

public static class DrinkGlassDisplayBuilder
{
    private const string GlassObjectName = "vaso vacio_0";
    private const string EmptySpritePath = "Assets/Art/Sprites/Glyphs/vaso vacio.png";
    private const string FailSpritePath = "Assets/Art/Sprites/Glyphs/vaso negro1.png";

    // Conecta el DrinkGlassDisplay al vaso que ya está en la escena.
    [MenuItem("Magic/Wire Drink Glass Display")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<DrinkGlassDisplay>() != null)
        {
            Debug.Log("Drink Glass Display: ya existe en la escena, no se creó nada.");
            return;
        }

        GameObject glassGO = GameObject.Find(GlassObjectName);
        if (glassGO == null)
        {
            Debug.LogError($"Drink Glass Display: no se encontró el objeto \"{GlassObjectName}\" en la escena.");
            return;
        }

        GlyphCastController caster = Object.FindFirstObjectByType<GlyphCastController>();
        if (caster == null)
        {
            Debug.LogError("Drink Glass Display: no se encontró un GlyphCastController en la escena.");
            return;
        }

        SpriteRenderer spriteRenderer = glassGO.GetComponent<SpriteRenderer>();
        Sprite emptySprite = AssetDatabase.LoadAssetAtPath<Sprite>(EmptySpritePath);
        Sprite failSprite = AssetDatabase.LoadAssetAtPath<Sprite>(FailSpritePath);

        if (emptySprite == null || failSprite == null)
        {
            Debug.LogError("Drink Glass Display: no se encontraron los sprites de vaso vacío/negro en las rutas esperadas.");
            return;
        }

        Undo.AddComponent<DrinkGlassDisplay>(glassGO);
        DrinkGlassDisplay display = glassGO.GetComponent<DrinkGlassDisplay>();

        SerializedObject so = new SerializedObject(display);
        so.FindProperty("caster").objectReferenceValue = caster;
        so.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
        so.FindProperty("emptySprite").objectReferenceValue = emptySprite;
        so.FindProperty("failSprite").objectReferenceValue = failSprite;
        so.ApplyModifiedProperties();

        Debug.Log("Drink Glass Display: conectado a \"" + GlassObjectName + "\".");
    }
}

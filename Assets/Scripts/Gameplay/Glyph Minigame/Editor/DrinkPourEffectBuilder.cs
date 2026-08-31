using UnityEditor;
using UnityEngine;

public static class DrinkPourEffectBuilder
{
    private const string GlassObjectName = "vaso vacio_0";

    // Crea el objeto de efecto de sabor (burbujas, fuego, etc.) encima del vaso.
    [MenuItem("Magic/Build Drink Pour Effect")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<DrinkPourEffect>() != null)
        {
            Debug.Log("Drink Pour Effect: ya existe en la escena, no se creó nada.");
            return;
        }

        GameObject glassGO = GameObject.Find(GlassObjectName);
        if (glassGO == null)
        {
            Debug.LogError($"Drink Pour Effect: no se encontró el objeto \"{GlassObjectName}\" en la escena.");
            return;
        }

        GlyphCastController caster = Object.FindFirstObjectByType<GlyphCastController>();
        if (caster == null)
        {
            Debug.LogError("Drink Pour Effect: no se encontró un GlyphCastController en la escena.");
            return;
        }

        GameObject go = new GameObject("Drink Pour Effect", typeof(SpriteRenderer), typeof(SpriteSheetFlipbook), typeof(DrinkPourEffect));
        Undo.RegisterCreatedObjectUndo(go, "Create Drink Pour Effect");
        go.transform.SetParent(glassGO.transform.parent, false);
        go.transform.position = glassGO.transform.position;

        SpriteRenderer glassRenderer = glassGO.GetComponent<SpriteRenderer>();
        SpriteRenderer effectRenderer = go.GetComponent<SpriteRenderer>();
        effectRenderer.sortingLayerID = glassRenderer.sortingLayerID;
        effectRenderer.sortingOrder = glassRenderer.sortingOrder + 1;
        effectRenderer.enabled = false;

        SpriteSheetFlipbook flipbook = go.GetComponent<SpriteSheetFlipbook>();
        SerializedObject flipbookSo = new SerializedObject(flipbook);
        flipbookSo.FindProperty("spriteRenderer").objectReferenceValue = effectRenderer;
        flipbookSo.ApplyModifiedProperties();

        DrinkPourEffect effect = go.GetComponent<DrinkPourEffect>();
        SerializedObject effectSo = new SerializedObject(effect);
        effectSo.FindProperty("caster").objectReferenceValue = caster;
        effectSo.FindProperty("flipbook").objectReferenceValue = flipbook;
        effectSo.ApplyModifiedProperties();

        Debug.Log("Drink Pour Effect: creado sobre \"" + GlassObjectName + "\". Asigná Pour Effect Frames en cada DrawPattern.");
    }
}

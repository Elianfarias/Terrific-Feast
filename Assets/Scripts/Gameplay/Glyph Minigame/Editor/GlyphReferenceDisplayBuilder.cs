using UnityEditor;
using UnityEngine;

public static class GlyphReferenceDisplayBuilder
{
    // Crea el objeto que muestra el glifo a trazar y lo conecta a todo lo
    // que necesita en la escena.
    [MenuItem("Magic/Build Glyph Reference Display")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<GlyphReferenceDisplay>() != null)
        {
            Debug.Log("Glyph Reference Display: ya existe en la escena, no se creó nada.");
            return;
        }

        GlyphCastController caster = Object.FindFirstObjectByType<GlyphCastController>();
        SoulEnergy soul = Object.FindFirstObjectByType<SoulEnergy>();
        SoulCreatureView creature = Object.FindFirstObjectByType<SoulCreatureView>();

        if (caster == null || soul == null || creature == null)
        {
            Debug.LogError("Glyph Reference Display: falta GlyphCastController, SoulEnergy y/o SoulCreatureView en la escena.");
            return;
        }

        GameObject go = new GameObject("Glyph Reference Display",
            typeof(SpriteRenderer), typeof(GlyphReferenceDisplay));
        Undo.RegisterCreatedObjectUndo(go, "Create Glyph Reference Display");

        go.transform.position = new Vector3(4f, 4f, 0.1f);

        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -10;
        spriteRenderer.enabled = false;

        GlyphReferenceDisplay display = go.GetComponent<GlyphReferenceDisplay>();
        SerializedObject so = new SerializedObject(display);
        so.FindProperty("caster").objectReferenceValue = caster;
        so.FindProperty("soul").objectReferenceValue = soul;
        so.FindProperty("creature").objectReferenceValue = creature;
        so.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
        so.ApplyModifiedProperties();

        Debug.Log("Glyph Reference Display: creado y conectado. Ajustá su posición si hace falta.");
    }
}

using UnityEngine;

// Muestra el vaso con la bebida que viene precargada desde el minijuego
// anterior. Ya no cambia de sprite según los sabores cargados acá: eso se
// resuelve en la otra escena antes de llegar a esta.
[RequireComponent(typeof(SpriteRenderer))]
public class DrinkGlassDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;

    private void Awake()
    {
        if (defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    // Setea el vaso con la bebida precargada desde el minijuego anterior.
    public void SetGlass(Sprite glassSprite)
    {
        if (glassSprite == null) return;
        spriteRenderer.sprite = glassSprite;
    }
}

using UnityEngine;

// Aplica al vaso el sprite ganado en el Wand Minigame (guardado en
// WandMinigameSession al cambiar de escena).
public class PreloadedGlassLoader : MonoBehaviour
{
    [SerializeField] private DrinkGlassDisplay glassDisplay;

    private void Awake()
    {
        if (WandMinigameSession.SelectedGlassSprite != null)
            glassDisplay.SetGlass(WandMinigameSession.SelectedGlassSprite);
    }
}

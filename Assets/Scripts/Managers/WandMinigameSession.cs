using UnityEngine;

// Datos que hay que pasar del Wand Minigame al minijuego de glifos al
// cambiar de escena (no se guardan en disco, solo duran la sesión actual).
public static class WandMinigameSession
{
    public static Sprite SelectedGlassSprite;
    public static MazeSignData SelectedLiquid;
}

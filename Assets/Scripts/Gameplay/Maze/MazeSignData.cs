using UnityEngine;

[CreateAssetMenu(fileName = "NewMazeSign", menuName = "Pociones/Maze Sign Data")]
public class MazeSignData : ScriptableObject
{
    [Header("Identification")]
    public string id;
    public string displayName;

    [Header("Resulting Potion")]
    [Tooltip("Sprite of the filled glass with the drink from this maze")]
    public Sprite fullGlassSprite;
}
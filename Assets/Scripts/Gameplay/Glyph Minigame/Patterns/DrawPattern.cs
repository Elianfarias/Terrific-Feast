using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlyph", menuName = "Magic/Draw Pattern")]
public class DrawPattern : ScriptableObject
{
    public string glyphName;
    public Sprite referenceSprite;
    public Sprite glassSprite;
    public Sprite[] pourEffectFrames;
    public List<PatternNode> nodes = new List<PatternNode>();
}

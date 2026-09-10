public readonly struct DrawnFlavorResult
{
    public readonly DrawPattern glyph;
    public readonly float accuracy;

    public DrawnFlavorResult(DrawPattern glyph, float accuracy)
    {
        this.glyph = glyph;
        this.accuracy = accuracy;
    }
}

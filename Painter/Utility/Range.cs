namespace ExcelDsp.Painter.Utility;

/// <summary>A range of numbers</summary>
/// <param name="Min">Minimum value</param>
/// <param name="Max">Maximum value</param>
internal record struct Range(int Min, int Max)
{
    /// <summary>Difference between <see cref="Min"/> and <see cref="Max"/></summary>
    public readonly int Width => Max - Min;

    /// <summary>Create from unordered endpoints</summary>
    /// <param name="a">Endpoint number</param>
    /// <param name="b">Endpoint number</param>
    /// <returns>New ordered <see cref="Range"/></returns>
    public static Range Create(int a, int b)
    {
        if(a < b)
            return new Range(a, b);
        else
            return new Range(b, a);
    }
}
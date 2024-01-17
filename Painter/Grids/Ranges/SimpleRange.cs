using System.Collections.Generic;

namespace ExcelDsp.Painter.Grids.Ranges;

/// <summary>A simple range of numbers</summary>
/// <param name="min">Minimum value</param>
/// <param name="max">Maximum value</param>
internal class SimpleRange(int min, int max) : Range
{
    private readonly int _min = min;
    private readonly int _max = max;

    /// <inheritdoc />
    public override int Count => _max - _min;

    /// <summary>Create the inverse of this range within a larger full range</summary>
    /// <param name="fullRange">Full range to invert within</param>
    /// <returns>Range from <see cref="_max"/> to <see cref="_min"/></returns>
    public CompoundRange Invert(SimpleRange fullRange)
    {
        SimpleRange longRange1 = new(_max, fullRange._max);
        SimpleRange longRange2 = new(fullRange._min, _min);
        return new CompoundRange(longRange1, longRange2);
    }

    /// <inheritdoc />
    public override IEnumerator<int> GetEnumerator()
    {
        for(int i = _min; i <= _max; i++)
            yield return i;
    }

    /// <summary>Create from unordered endpoints</summary>
    /// <param name="a">Endpoint number</param>
    /// <param name="b">Endpoint number</param>
    /// <returns>New ordered <see cref="Range"/></returns>
    public static SimpleRange Create(int a, int b)
    {
        if(a < b)
            return new SimpleRange(a, b);
        else
            return new SimpleRange(b, a);
    }
}
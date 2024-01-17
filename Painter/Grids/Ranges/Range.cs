using System.Collections;
using System.Collections.Generic;

namespace ExcelDsp.Painter.Grids.Ranges;

/// <summary>Range of enumerable sequential numbers</summary>
internal abstract class Range : IEnumerable<int>
{
    public abstract int Count { get; }

    /// <inheritdoc />
    public abstract IEnumerator<int> GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

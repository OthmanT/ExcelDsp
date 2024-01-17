using System.Collections.Generic;
using System.Linq;

namespace ExcelDsp.Painter.Grids.Ranges;

/// <summary><see cref="Range"/> composed of multiple inner ordered <see cref="Range"/></summary>
/// <param name="ranges"><see cref="Range"/> to enumerate</param>
internal class CompoundRange(params Range[] ranges) : Range
{
    private readonly Range[] _ranges = ranges;

    /// <inheritdoc />
    public override int Count => _ranges.Sum(r => r.Count);

    /// <inheritdoc />
    public override IEnumerator<int> GetEnumerator()
        => _ranges.SelectMany(r => r).GetEnumerator();
}

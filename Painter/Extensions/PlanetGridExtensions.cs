using ExcelDsp.Painter.Grids;
using UnityEngine;

namespace ExcelDsp.Painter.Extensions;

internal static class PlanetGridExtensions
{
    /// <summary>Calculate the number of longitude <see cref="PolarCoordinate.Segment"/> at a given latitude</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="latSegment">Latitude <see cref="PolarCoordinate.Segment"/></param>
    /// <returns>Number of longitude <see cref="PolarCoordinate.Segment"/></returns>
    public static int CalculateLongitudeSegments(this PlanetGrid grid, float latSegment)
    {
        int latIndex = Mathf.FloorToInt(Mathf.Abs(latSegment));
        int segments = PlanetGrid.DetermineLongitudeSegmentCount(latIndex, grid.segment);
        return segments;
    }
}

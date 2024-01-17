using ExcelDsp.Painter.Grids;
using ExcelDsp.Painter.Utility;
using UnityEngine;

namespace ExcelDsp.Painter.Extensions;

internal static class PlanetGridExtensions
{
    /// <summary>Compute snapped grid segments in a rectangle</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="platform"><see cref="PlatformSystem"/></param>
    /// <param name="startPos">Rectangle start position</param>
    /// <param name="endPos">Rectangle end position</param>
    /// <param name="useShortestPath">Whether to use the shortest or longest arc path between the endpoints</param>
    /// <param name="reformPoints">Output points on the normalized grid that require reformation (excludes already reformed points)</param>
    /// <param name="reformIndices">Output reformation indices in the rectangle</param>
    /// <param name="reformCenter">Center point on the normalized grid</param>
    /// <returns>Number of valid <paramref name="reformPoints"/></returns>
    public static int ReformSnapRect(this PlanetGrid grid, PlatformSystem platform, Vector3 startPos, Vector3 endPos, bool useShortestPath, ref Vector3[] reformPoints, ref int[] reformIndices, out Vector3 reformCenter)
    {
        platform.EnsureReformData();
        float latSegmentMax = platform.latitudeCount / 10;

        int indexCount = 0;
        int pointCount = 0;

        GridTile start = GridTile.FromPosition(grid, startPos);
        GridTile end = GridTile.FromPosition(grid, endPos);
        if(!end.Longitude.IsValid || !end.LatitudeRow.IsValid(latSegmentMax))
            end = start;

        bool isFirst = true;
        bool isWide = false;
        Range latRange = Range.Create(start.Latitude.Element, end.Latitude.Element);
        for(int latElement = latRange.Min; latElement <= latRange.Max; latElement++)
        {
            GridRow row = GridRow.FromLatitudeElement(grid, latElement);
            if(!row.IsValid(latSegmentMax))
                continue;

            AddRow(platform, row, start, end, useShortestPath, isFirst, ref isWide, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
            isFirst = false;
        }

        reformCenter = end.CalculatePosition();
        return pointCount;
    }

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

    private static void AddRow(PlatformSystem platform, GridRow row, GridTile start, GridTile end, bool useShortestPath, bool isFirst, ref bool isWide, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        GridTile latStart = GridTile.FromLongitudeAngle(row, start.Longitude.Angle);
        GridTile latEnd = GridTile.FromLongitudeAngle(row, end.Longitude.Angle);

        Range longRange = Range.Create(latStart.Longitude.Element, latEnd.Longitude.Element);
        Range fullRange = PolarCoordinate.GetElementRange(row.LongitudeSegments);

        // Check if more than half of the total width is selected
        if(isFirst)
            isWide = longRange.Width > fullRange.Max;

        // We need to calculate the shortest/longest path to overlay longRange over fullRange
        if(isWide == useShortestPath)
        {
            // Split ranges across the anti-prime-meridian to select the inverse
            Range longRange1 = new(fullRange.Min, longRange.Min);
            Range longRange2 = new(longRange.Max, fullRange.Max);
            AddLongitudeRange(platform, row, longRange1, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
            AddLongitudeRange(platform, row, longRange2, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
        }
        else
        {
            // Use selection as-is
            AddLongitudeRange(platform, row, longRange, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
        }
    }

    private static void AddLongitudeRange(PlatformSystem platform, GridRow row, Range longRange, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        for(int longElement = longRange.Min; longElement <= longRange.Max; longElement++)
        {
            GridTile tile = GridTile.FromLongitudeElement(row, longElement);
            if(tile.Longitude.IsValid)
                AddTile(platform, tile, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
        }
    }

    private static void AddTile(PlatformSystem platform, GridTile tile, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        int reformIndex = platform.GetReformIndexForSegment(tile.Latitude.Segment, tile.Longitude.Segment);
        ResizableArray.AddItem(ref reformIndices, ref indexCount, reformIndex);

        int reformType = platform.GetReformType(reformIndex);
        if(!platform.IsTerrainReformed(reformType))
            ResizableArray.AddItem(ref reformPoints, ref pointCount, tile.CalculatePosition());
    }
}

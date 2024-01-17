using ExcelDsp.Painter.Grids;
using ExcelDsp.Painter.Grids.Ranges;
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

        SimpleRange latRange = SimpleRange.Create(start.Latitude.Element, end.Latitude.Element);
        GridTile startRowEnd = GridTile.FromLongitudeAngle(start.LatitudeRow, end.Longitude.Angle);
        bool invert = CalculatePath(start.LatitudeRow, start, startRowEnd, useShortestPath);

        foreach(int latElement in latRange)
        {
            GridRow row = GridRow.FromLatitudeElement(grid, latElement);
            if(!row.IsValid(latSegmentMax))
                continue;

            AddRow(platform, row, start, end, invert, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
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

    private static bool CalculatePath(GridRow row, GridTile start, GridTile end, bool useShortestPath)
    {
        GridTile latStart = GridTile.FromLongitudeAngle(row, start.Longitude.Angle);
        GridTile latEnd = GridTile.FromLongitudeAngle(row, end.Longitude.Angle);

        SimpleRange longRange = SimpleRange.Create(latStart.Longitude.Element, latEnd.Longitude.Element);
        SimpleRange fullRange = PolarCoordinate.GetElementRange(row.LongitudeSegments);

        int midpoint = fullRange.Count / 2;
        bool isWide = longRange.Count > midpoint;
        bool invert = isWide == useShortestPath;
        return invert;
    }

    private static void AddRow(PlatformSystem platform, GridRow row, GridTile start, GridTile end, bool invert, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        GridTile latStart = GridTile.FromLongitudeAngle(row, start.Longitude.Angle);
        GridTile latEnd = GridTile.FromLongitudeAngle(row, end.Longitude.Angle);

        SimpleRange longRangeSimple = SimpleRange.Create(latStart.Longitude.Element, latEnd.Longitude.Element);
        Range longRange = longRangeSimple;

        if(invert)
        {
            // Split range across the anti-prime-meridian to select the inverse
            SimpleRange fullRange = PolarCoordinate.GetElementRange(row.LongitudeSegments);
            longRange = longRangeSimple.Invert(fullRange);
        }

        foreach(int longElement in longRange)
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

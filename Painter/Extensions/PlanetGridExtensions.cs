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
        if(!end.IsValid(latSegmentMax))
            end = start;

        bool isFirst = true;
        bool isWide = false;
        Range latRange = Range.Create(start.Latitude.Element, end.Latitude.Element);
        for(int latElement = latRange.Min; latElement <= latRange.Max; latElement++)
        {
            AddLatitude(grid, platform, latSegmentMax, start, end, latElement, useShortestPath, isFirst, ref isWide, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
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

    private static void AddLatitude(PlanetGrid grid, PlatformSystem platform, float latSegmentMax, GridTile start, GridTile end, int latElement, bool useShortestPath, bool isFirst, ref bool isWide, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        if(latElement == 0)
            return;

        GridTile latStart = GridTile.FromBandAngle(grid, latElement, start.Longitude.Angle);
        GridTile latEnd = GridTile.FromBandAngle(grid, latElement, end.Longitude.Angle);

        Range longRange = Range.Create(latStart.Longitude.Element, latEnd.Longitude.Element);
        Range fullRange = PolarCoordinate.GetElementRange(latStart.LongitudeSegments);

        // Check if more than half of the total width is selected
        if(isFirst)
            isWide = longRange.Width > fullRange.Max;

        // We need to calculate the shortest/longest path to overlay longRange over fullRange
        if(isWide == useShortestPath)
        {
            // Split ranges across the anti-prime-meridian to select the inverse
            Range longRange1 = new(fullRange.Min, longRange.Min);
            Range longRange2 = new(longRange.Max, fullRange.Max);
            AddLongitudeRange(grid, platform, latSegmentMax, latElement, longRange1, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
            AddLongitudeRange(grid, platform, latSegmentMax, latElement, longRange2, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
        }
        else
        {
            // Use selection as-is
            AddLongitudeRange(grid, platform, latSegmentMax, latElement, longRange, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
        }
    }

    private static void AddLongitudeRange(PlanetGrid grid, PlatformSystem platform, float latSegmentMax, int latElement, Range longRange, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        for(int longElement = longRange.Min; longElement <= longRange.Max; longElement++)
        {
            GridTile tile = GridTile.FromElement(grid, latElement, longElement);
            AddTile(platform, latSegmentMax, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount, tile);
        }
    }

    private static void AddTile(PlatformSystem platform, float latSegmentMax, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount, GridTile tile)
    {
        if(!tile.IsValid(latSegmentMax))
            return;

        int reformIndex = platform.GetReformIndexForSegment(tile.Latitude.Segment, tile.Longitude.Segment);
        ResizableArray.AddItem(ref reformIndices, ref indexCount, reformIndex);

        int reformType = platform.GetReformType(reformIndex);
        if(!platform.IsTerrainReformed(reformType))
            ResizableArray.AddItem(ref reformPoints, ref pointCount, tile.CalculatePosition());
    }
}

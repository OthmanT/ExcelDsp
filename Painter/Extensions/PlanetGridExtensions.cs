using ExcelDsp.Painter.Utility;
using UnityEngine;

namespace ExcelDsp.Painter.Extensions;

internal static class PlanetGridExtensions
{
    private const int IndexIntStep = 1;

    /// <summary>Compute snapped grid segments in a rectangle</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="platform"><see cref="PlatformSystem"/></param>
    /// <param name="startPos">Rectangle start position</param>
    /// <param name="endPos">Rectangle end position</param>
    /// <param name="reformPoints">Output points on the normalized grid that require reformation (excludes already reformed points)</param>
    /// <param name="reformIndices">Output reformation indices in the rectangle</param>
    /// <param name="reformCenter">Center point on the normalized grid</param>
    /// <returns>Number of valid <paramref name="reformPoints"/></returns>
    public static int ReformSnapRect(this PlanetGrid grid, PlatformSystem platform, Vector3 startPos, Vector3 endPos, ref Vector3[] reformPoints, ref int[] reformIndices, out Vector3 reformCenter)
    {
        float latIndexPartMax = platform.latitudeCount / 10;
        int indexCount = 0;
        int pointCount = 0;

        Tile start = Tile.FromPosition(grid, startPos);
        Tile end = Tile.FromPosition(grid, endPos);
        if(!end.IsValid(latIndexPartMax))
            end = start;

        Range latRange = Range.Create(start.LatIndexInt, end.LatIndexInt);
        for(int latIndex = latRange.Min; latIndex <= latRange.Max; latIndex += IndexIntStep)
            AddLatitude(grid, platform, latIndexPartMax, start, end, latIndex, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);

        reformCenter = start.GetPosition();
        return pointCount;
    }

    private static void AddLatitude(PlanetGrid grid, PlatformSystem platform, float latIndexPartMax, Tile start, Tile end, int latIndex, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        if(latIndex == 0)
            return;

        Tile latStart = Tile.FromLatIndexLongAngle(grid, latIndex, start.LongAngle);
        Tile latEnd = Tile.FromLatIndexLongAngle(grid, latIndex, end.LongAngle);

        Range longRange = Range.Create(latStart.LongIndexInt, latEnd.LongIndexInt);
        AddRange(grid, platform, latIndexPartMax, latIndex, longRange, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount);
    }

    private static void AddRange(PlanetGrid grid, PlatformSystem platform, float latIndexPartMax, int latIndex, Range longRange, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount)
    {
        for(int longIndex = longRange.Min; longIndex <= longRange.Max; longIndex += IndexIntStep)
        {
            Tile tile = Tile.FromIndex(grid, latIndex, longIndex);
            AddTile(platform, latIndexPartMax, ref reformPoints, ref reformIndices, ref indexCount, ref pointCount, tile);
        }
    }

    private static void AddTile(PlatformSystem platform, float latIndexPartMax, ref Vector3[] reformPoints, ref int[] reformIndices, ref int indexCount, ref int pointCount, Tile tile)
    {
        if(!tile.IsValid(latIndexPartMax))
            return;

        int reformIndex = platform.GetReformIndexForSegment(tile.LatIndexPart, tile.LongIndexPart);
        ResizableArray.AddItem(ref reformIndices, ref indexCount, reformIndex);

        int reformType = platform.GetReformType(reformIndex);
        if(!platform.IsTerrainReformed(reformType))
            ResizableArray.AddItem(ref reformPoints, ref pointCount, tile.GetPosition());
    }

    public static int GetLongsAtLat(this PlanetGrid grid, float latIndexPart)
    {
        int latIndex = Mathf.FloorToInt(Mathf.Abs(latIndexPart));
        int count = PlanetGrid.DetermineLongitudeSegmentCount(latIndex, grid.segment);
        return count;
    }
}

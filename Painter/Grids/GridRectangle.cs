using ExcelDsp.Painter.Grids.Ranges;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelDsp.Painter.Grids;

/// <summary>Rectangular selection of <see cref="GridTile"/></summary>
internal class GridRectangle
{
    private readonly List<GridTile> _tiles = [];
    private PlanetData? _planet;

    /// <summary>Save the calculated <see cref="GridTile"/> data in the format used by the game</summary>
    /// <param name="indices">Reform indices (used by <see cref="PlatformSystem"/>) for all tiles (including unreformed)</param>
    /// <param name="points">Points relative to the planet's center (only for unreformed tiles)</param>
    /// <param name="pointCount">Number of valid <paramref name="points"/></param>
    /// <param name="centerPoint">An arbitrary <paramref name="points"/> value roughly near the middle</param>
    public void Export(ref int[] indices, ref Vector3[] points, out int pointCount, out Vector3 centerPoint)
    {
        if(_planet is null)
            throw new InvalidOperationException("Must calculate first");

        EnsureSize(ref points, _tiles.Count);
        EnsureSize(ref indices, _tiles.Count);

        PlatformSystem platform = _planet.factory.platformSystem;
        float radius = _planet.radius + 0.2f;
        pointCount = 0;

        if(_tiles.Count == 0)
        {
            centerPoint = Vector3.zero;
            return;
        }

        int midIndex = _tiles.Count / 2;
        GridTile midTile = _tiles[midIndex];
        centerPoint = midTile.CalculatePosition() * radius;

        for(int i = 0; i < _tiles.Count; i++)
        {
            GridTile tile = _tiles[i];

            int reformIndex = platform.GetReformIndexForSegment(tile.Latitude.Segment, tile.Longitude.Segment);
            indices[i] = reformIndex;

            int reformType = platform.GetReformType(reformIndex);
            if(!platform.IsTerrainReformed(reformType))
                points[pointCount++] = tile.CalculatePosition() * radius;
        }
    }

    /// <summary>Calculate the <see cref="GridTile"/>s within a given rectangle</summary>
    /// <param name="planet"><see cref="PlanetData"/></param>
    /// <param name="startPos">Starting corner position (relative to planet center)</param>
    /// <param name="endPos">Ending corner position (relative to planet center)</param>
    /// <param name="useShortestPath">Whether to use the shortest or longest horizontal path between the selected corners</param>
    public void Calculate(PlanetData planet, Vector3 startPos, Vector3 endPos, bool useShortestPath)
    {
        _planet = planet;
        _tiles.Clear();

        PlanetGrid grid = planet.aux.mainGrid;
        PlatformSystem platform = planet.factory.platformSystem;
        platform.EnsureReformData();
        float latSegmentMax = platform.latitudeCount / 10;

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

            AddRow(row, start, end, invert);
        }
    }

    private void AddRow(GridRow row, GridTile start, GridTile end, bool invert)
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
                _tiles.Add(tile);
        }
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

    private static void EnsureSize<T>(ref T[] array, int size)
    {
        if(array.Length < size)
            Array.Resize(ref array, size);
    }
}

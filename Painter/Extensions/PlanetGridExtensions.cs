using System;
using UnityEngine;

namespace ExcelDsp.Painter.Extensions;

internal static class PlanetGridExtensions
{
    private const float Step = 0.2f;

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
        Segment start = Segment.FromPosition(grid, startPos);
        Segment end = Segment.FromPosition(grid, endPos);

        Range latRange = Range.Create(start.LatIndex, end.LatIndex);
        Range longRange = Range.Create(start.LongIndex, end.LongIndex);

        float latMax = platform.latitudeCount / 10;
        Range latValidRange = new(-latMax, latMax);

        int indexCount = 0;
        int pointCount = 0;

        for(float latIndex = latRange.Min; latIndex <= latRange.Max; latIndex += Step)
        {
            for(float longIndex = longRange.Min; longIndex <= longRange.Max; longIndex += Step)
            {
                Segment seg = Segment.FromIndices(grid, latIndex, longIndex);

                bool isValid = latValidRange.ContainsExclusive(seg.LatIndex) && seg.SegmentCount == start.SegmentCount;
                int reformIndex = isValid ? platform.GetReformIndexForSegment(seg.LatIndex, seg.LongIndex) : -1;
                AddItem(ref reformIndices, ref indexCount, reformIndex);

                bool needsReform = isValid && !platform.IsTerrainReformed(platform.GetReformType(reformIndex));
                if(needsReform)
                    AddItem(ref reformPoints, ref pointCount, seg.GetPosition());
            }
        }

        reformCenter = start.GetPosition();
        return pointCount;
    }

    private static void AddItem<T>(ref T[] array, ref int index, T item)
    {
        int count = index + 1;
        if(array.Length < count)
            Array.Resize(ref array, count);

        array[index] = item;
        index = count;
    }

    private struct Segment
    {
        public float LatAngle;
        public float LongAngle;
        public float LatIndex;
        public float LongIndex;
        public int SegmentCount;

        public static Segment FromAngles(PlanetGrid grid, float latAngle, float longAngle)
        {
            Segment seg = new()
            {
                LatAngle = latAngle,
                LongAngle = longAngle,
                LatIndex = latAngle / (Mathf.PI * 2f) * grid.segment
            };

            int latIndexInt = Mathf.FloorToInt(Mathf.Abs(seg.LatIndex));
            seg.SegmentCount = PlanetGrid.DetermineLongitudeSegmentCount(latIndexInt, grid.segment);

            seg.LongIndex = longAngle / (Mathf.PI * 2f) * seg.SegmentCount;
            return seg;
        }

        public static Segment FromIndices(PlanetGrid grid, float latIndex, float longIndex)
        {
            Segment seg = new()
            {
                LatIndex = latIndex,
                LongIndex = longIndex,
                LatAngle = latIndex / grid.segment * (Mathf.PI * 2f),
            };

            int latIndexInt = Mathf.FloorToInt(Mathf.Abs(latIndex));
            seg.SegmentCount = PlanetGrid.DetermineLongitudeSegmentCount(latIndexInt, grid.segment);

            seg.LongAngle = longIndex / seg.SegmentCount * (Mathf.PI * 2f);
            return seg;
        }

        public static Segment FromPosition(PlanetGrid grid, Vector3 position)
        {
            Vector3 normalized = position.normalized;
            float latAngle = Mathf.Asin(normalized.y);
            float longAngle = Mathf.Atan2(normalized.x, 0f - normalized.z);
            return FromAngles(grid, latAngle, longAngle);
        }

        public readonly Vector3 GetPosition()
        {
            float latSin = Mathf.Sin(LatAngle);
            float latCos = Mathf.Cos(LatAngle);
            float longSin = Mathf.Sin(LongAngle);
            float longCos = Mathf.Cos(LongAngle);
            return new Vector3(latCos * longSin, latSin, latCos * (0f - longCos));
        }
    }

    private record struct Range(float Min, float Max)
    {
        public readonly bool ContainsInclusive(float value)
            => value >= Min && value <= Max;

        public readonly bool ContainsExclusive(float value)
            => value > Min && value < Max;

        public static Range Create(float a, float b)
        {
            if(a < b)
                return new Range(a, b);
            else
                return new Range(b, a);
        }
    }
}

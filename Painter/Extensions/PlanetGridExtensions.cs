using System;
using UnityEngine;

namespace ExcelDsp.Painter.Extensions;

internal static class PlanetGridExtensions
{
    private const int Step = 1;
    private const float PartScale = 5f;
    private const float Pi2 = Mathf.PI * 2;

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
        Segment start = Segment.FromPosition(grid, startPos);
        Segment end = Segment.FromPosition(grid, endPos);

        if(!end.IsValid(latIndexPartMax, start.LongsAtLat))
            end = start;

        Range latRange = Range.Create(start.LatIndexInt, end.LatIndexInt);
        Range longRange = Range.Create(start.LongIndexInt, end.LongIndexInt);

        int indexCount = 0;
        int pointCount = 0;

        for(int latIndex = latRange.Min; latIndex <= latRange.Max; latIndex += Step)
        {
            if(latIndex == 0)
                continue;

            for(int longIndex = longRange.Min; longIndex <= longRange.Max; longIndex += Step)
            {
                Segment seg = Segment.FromIndex(grid, latIndex, longIndex);
                if(!seg.IsValid(latIndexPartMax, start.LongsAtLat))
                    continue;

                // TODO: Off-by-one at tropic lines
                int reformIndex = platform.GetReformIndexForSegment(seg.LatIndexPart, seg.LongIndexPart);
                AddItem(ref reformIndices, ref indexCount, reformIndex);

                int reformType = platform.GetReformType(reformIndex);
                if(!platform.IsTerrainReformed(reformType))
                    AddItem(ref reformPoints, ref pointCount, seg.GetPosition());
            }
        }

        reformCenter = start.GetPosition();
        return pointCount;
    }

    public static int GetLongsAtLat(this PlanetGrid grid, float latIndexPart)
    {
        int latIndex = Mathf.FloorToInt(Mathf.Abs(latIndexPart));
        int count = PlanetGrid.DetermineLongitudeSegmentCount(latIndex, grid.segment);
        return count;
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
        public int LatIndexInt;
        public int LongIndexInt;
        public float LatIndexPart;
        public float LongIndexPart;
        public int LongsAtLat;

        public static Segment FromAngle(PlanetGrid grid, float latAngle, float longAngle)
        {
            Segment seg = new()
            {
                LatAngle = latAngle,
                LongAngle = longAngle,
                LatIndexInt = AngleToIndexInt(latAngle, grid.segment)
            };
            seg.LatIndexPart = IndexIntToPart(seg.LatIndexInt);
            seg.LongsAtLat = grid.GetLongsAtLat(seg.LatIndexPart);

            seg.LongIndexInt = AngleToIndexInt(longAngle, seg.LongsAtLat);
            seg.LongIndexPart = IndexIntToPart(seg.LongIndexInt);
            return seg;
        }

        public static Segment FromIndex(PlanetGrid grid, int latIndexInt, int longIndexInt)
        {
            Segment seg = new()
            {
                LatIndexInt = latIndexInt,
                LatIndexPart = IndexIntToPart(latIndexInt),
                LongIndexInt = longIndexInt,
                LongIndexPart = IndexIntToPart(longIndexInt),
            };

            seg.LatAngle = IndexPartToAngle(seg.LatIndexPart, grid.segment);
            seg.LongsAtLat = grid.GetLongsAtLat(seg.LatIndexPart);

            seg.LongAngle = IndexPartToAngle(seg.LongIndexPart, seg.LongsAtLat);
            return seg;
        }

        public static Segment FromPosition(PlanetGrid grid, Vector3 position)
        {
            Vector3 normalized = position.normalized;
            float latAngle = Mathf.Asin(normalized.y);
            float longAngle = Mathf.Atan2(normalized.x, 0f - normalized.z);
            return FromAngle(grid, latAngle, longAngle);
        }

        public readonly Vector3 GetPosition()
        {
            float latSin = Mathf.Sin(LatAngle);
            float latCos = Mathf.Cos(LatAngle);
            float longSin = Mathf.Sin(LongAngle);
            float longCos = Mathf.Cos(LongAngle);
            return new Vector3(latCos * longSin, latSin, latCos * (0f - longCos));
        }

        public readonly bool IsValid(float latIndexPartMax, int longsAtLat)
        {
            return LatIndexPart < latIndexPartMax
                && LatIndexPart > -latIndexPartMax
                && LatIndexInt != 0
                && LongIndexInt != 0
                && LongsAtLat == longsAtLat;
        }

        public override readonly string ToString()
            => $"({LatIndexInt},{LongIndexInt})";

        private static int AngleToIndexInt(float angle, int segments)
        {
            float rawIndexPart = angle / Pi2 * segments;
            return IndexPartToInt(rawIndexPart);
        }

        private static float IndexPartToAngle(float index, int segments)
            => index / segments * Pi2;

        private static int IndexPartToInt(float indexPart)
        {
            float scaled = indexPart * PartScale;
            if(scaled > 0)
                return Mathf.CeilToInt(scaled);
            else
                return Mathf.FloorToInt(scaled);
        }

        private static float IndexIntToPart(int indexInt)
            => indexInt / PartScale;
    }

    private record struct Range(int Min, int Max)
    {
        public static Range Create(int a, int b)
        {
            if(a < b)
                return new Range(a, b);
            else
                return new Range(b, a);
        }
    }
}

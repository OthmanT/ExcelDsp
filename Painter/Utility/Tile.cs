using ExcelDsp.Painter.Extensions;
using UnityEngine;

namespace ExcelDsp.Painter.Utility;

internal struct Tile
{
    private const float IndexPartScale = 5f;
    private const float IndexPartOffset = -0.1f;
    private const float Pi2 = Mathf.PI * 2;

    public float LatAngle;
    public float LongAngle;
    public int LatIndexInt;
    public int LongIndexInt;
    public float LatIndexPart;
    public float LongIndexPart;
    public int LongsAtLat;

    public static Tile FromAngle(PlanetGrid grid, float latAngle, float longAngle)
    {
        Tile seg = new()
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

    public static Tile FromIndex(PlanetGrid grid, int latIndexInt, int longIndexInt)
    {
        Tile seg = new()
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

    public static Tile FromLatIndexLongAngle(PlanetGrid grid, int latIndexInt, float longAngle)
    {
        Tile seg = new()
        {
            LatIndexInt = latIndexInt,
            LatIndexPart = IndexIntToPart(latIndexInt),
            LongAngle = longAngle,
        };

        seg.LatAngle = IndexPartToAngle(seg.LatIndexPart, grid.segment);
        seg.LongsAtLat = grid.GetLongsAtLat(seg.LatIndexPart);

        seg.LongIndexInt = AngleToIndexInt(longAngle, seg.LongsAtLat);
        seg.LongIndexPart = IndexIntToPart(seg.LongIndexInt);
        return seg;
    }

    public static Tile FromPosition(PlanetGrid grid, Vector3 position)
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

    public readonly bool IsValid(float latIndexPartMax)
    {
        return LatIndexPart < latIndexPartMax
            && LatIndexPart > -latIndexPartMax
            && LatIndexInt != 0
            && LongIndexInt != 0;
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
        float scaled = indexPart * IndexPartScale;
        if(scaled > 0)
            return Mathf.CeilToInt(scaled);
        else
            return Mathf.FloorToInt(scaled);
    }

    private static float IndexIntToPart(int indexInt)
    {
        float indexPart = indexInt / IndexPartScale;
        float offset = indexPart > 0 ? IndexPartOffset : -IndexPartOffset;
        return indexPart + offset;
    }
}

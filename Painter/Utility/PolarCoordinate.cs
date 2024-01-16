using UnityEngine;

namespace ExcelDsp.Painter.Utility;

/// <summary>One co-ordinate (of two) for a <see cref="GridTile"/></summary>
internal struct PolarCoordinate
{
    private const int ElementsPerSegment = 5;
    private const float SegmentOffset = -0.1f;
    private const float Pi2 = Mathf.PI * 2;

    /// <summary>Polar angle (latitude or longitude) in radians</summary>
    /// <remarks>Relative to equator or prime-meridian</remarks>
    public float Angle;

    /// <summary>Fractional coordinates used by DSP grid</summary>
    /// <remarks>
    ///     - 1.0 per major grid area
    ///     - 0.2 per element (smallest visible tiles)
    ///     - 0.1 per sub-element
    ///     - Ranges from -N to N, skipping 0
    /// </remarks>
    public float Segment;

    /// <summary>Grid tile index</summary>
    /// <remarks>
    ///     - One per smallest visible tile
    ///     - Zero is the prime-meridian or equator and is not a valid tile
    ///     - Ranges from -N to N, skipping 0
    /// </remarks>
    public int Element;

    /// <summary>Calculate from <see cref="Angle"/></summary>
    /// <param name="angle"><see cref="Angle"/></param>
    /// <param name="segments">Number of <see cref="Segment"/>s on the grid</param>
    /// <returns>New <see cref="PolarCoordinate"/></returns>
    public static PolarCoordinate FromAngle(float angle, int segments)
    {
        PolarCoordinate pc = new()
        {
            Angle = angle,
            Element = AngleToElement(angle, segments)
        };
        pc.Segment = ElementToSegment(pc.Element);
        return pc;
    }

    /// <summary>Calculate from <see cref="Element"/></summary>
    /// <param name="element"><see cref="Element"/></param>
    /// <param name="segments">Number of <see cref="Segment"/>s on the grid</param>
    /// <returns>New <see cref="PolarCoordinate"/></returns>
    public static PolarCoordinate FromElement(int element, int segments)
    {
        PolarCoordinate pc = new()
        {
            Element = element,
            Segment = ElementToSegment(element)
        };
        pc.Angle = SegmentToAngle(pc.Segment, segments);
        return pc;
    }

    /// <summary>Get the range of values for <see cref="Element"/></summary>
    /// <returns></returns>
    public static Range GetElementRange(int segments)
    {
        int segentMid = segments / 2;
        int elementMid = segentMid * ElementsPerSegment;
        return new Range(-elementMid, elementMid);
    }

    private static int AngleToElement(float angle, int segments)
    {
        // Always round to element first, instead of keeping fractional segment
        float segment = angle / Pi2 * segments;
        return SegmentToElement(segment);
    }

    private static float SegmentToAngle(float segment, int segments)
        => segment / segments * Pi2;

    private static int SegmentToElement(float segment)
    {
        float element = segment * ElementsPerSegment;
        if(element > 0)
            return Mathf.CeilToInt(element);
        else
            return Mathf.FloorToInt(element);
    }

    private static float ElementToSegment(int element)
    {
        float segment = element / (float)ElementsPerSegment;
        float offset = segment > 0 ? SegmentOffset : -SegmentOffset;
        return segment + offset;
    }
}

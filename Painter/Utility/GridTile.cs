using ExcelDsp.Painter.Extensions;
using UnityEngine;

namespace ExcelDsp.Painter.Utility;

/// <summary>Single tile on a <see cref="PlanetGrid"/></summary>
internal struct GridTile
{
    /// <summary>Latitude (north/south) coordinate</summary>
    public PolarCoordinate Latitude;

    /// <summary>Longitude (east/west) coordinate)</summary>
    public PolarCoordinate Longitude;

    /// <summary>Number of longitude <see cref="PolarCoordinate.Segment"/> values at the current latitude</summary>
    /// <remarks>Largest at equator, decreasing towards each pole</remarks>
    public int LongitudeSegments;

    /// <summary>Calculate from <see cref="PolarCoordinate.Angle"/>s</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="latAngle">Latitude <see cref="PolarCoordinate.Angle"/></param>
    /// <param name="longAngle">Longitude <see cref="PolarCoordinate.Angle"/></param>
    /// <returns>New <see cref="GridTile"/></returns>
    public static GridTile FromAngle(PlanetGrid grid, float latAngle, float longAngle)
    {
        GridTile tile = new()
        {
            Latitude = PolarCoordinate.FromAngle(latAngle, grid.segment)
        };
        tile.LongitudeSegments = grid.CalculateLongitudeSegments(tile.Latitude.Segment);
        tile.Longitude = PolarCoordinate.FromAngle(longAngle, tile.LongitudeSegments);
        return tile;
    }

    /// <summary>Calculate from <see cref="PolarCoordinate.Element"/>s</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="latElement">Latitude <see cref="PolarCoordinate.Element"/></param>
    /// <param name="longElement">Longitude <see cref="PolarCoordinate.Element"/></param>
    /// <returns>New <see cref="GridTile"/></returns>
    public static GridTile FromElement(PlanetGrid grid, int latElement, int longElement)
    {
        GridTile tile = new()
        {
            Latitude = PolarCoordinate.FromElement(latElement, grid.segment)
        };
        tile.LongitudeSegments = grid.CalculateLongitudeSegments(tile.Latitude.Segment);
        tile.Longitude = PolarCoordinate.FromElement(longElement, tile.LongitudeSegments);
        return tile;
    }

    /// <summary>Calculate from a latitude <see cref="PolarCoordinate.Element"/> (band) and longitude angle</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="latElement">Latitude <see cref="PolarCoordinate.Element"/></param>
    /// <param name="longAngle">Longitude <see cref="PolarCoordinate.Angle"/></param>
    /// <returns>New <see cref="GridTile"/></returns>
    public static GridTile FromBandAngle(PlanetGrid grid, int latElement, float longAngle)
    {
        GridTile tile = new()
        {
            Latitude = PolarCoordinate.FromElement(latElement, grid.segment)
        };
        tile.LongitudeSegments = grid.CalculateLongitudeSegments(tile.Latitude.Segment);
        tile.Longitude = PolarCoordinate.FromAngle(longAngle, tile.LongitudeSegments);
        return tile;
    }

    /// <summary>Calculate from a planetary position</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="position">Position relative to the planet's center</param>
    /// <returns>New <see cref="GridTile"/></returns>
    public static GridTile FromPosition(PlanetGrid grid, Vector3 position)
    {
        Vector3 normalized = position.normalized;
        float latAngle = Mathf.Asin(normalized.y);
        float longAngle = Mathf.Atan2(normalized.x, 0f - normalized.z);
        return FromAngle(grid, latAngle, longAngle);
    }

    /// <summary>Calculate the planetary position</summary>
    /// <returns>Position relative to the planet's center</returns>
    public readonly Vector3 CalculatePosition()
    {
        float latSin = Mathf.Sin(Latitude.Angle);
        float latCos = Mathf.Cos(Latitude.Angle);
        float longSin = Mathf.Sin(Longitude.Angle);
        float longCos = Mathf.Cos(Longitude.Angle);
        return new Vector3(latCos * longSin, latSin, latCos * (0f - longCos));
    }

    /// <summary>Check if the coordinates are valid</summary>
    /// <param name="latSegmentMax">Maximum latitude segment value</param>
    /// <returns>Whether this represents a usable tile on the grid</returns>
    public readonly bool IsValid(float latSegmentMax)
    {
        return Latitude.Segment < latSegmentMax
            && Latitude.Segment > -latSegmentMax
            && Latitude.Element != 0
            && Longitude.Element != 0;
    }

    /// <inheritdoc />
    public override readonly string ToString()
        => $"({Latitude.Element},{Longitude.Element})";
}

using UnityEngine;

namespace ExcelDsp.Painter.Grids;

/// <summary>Single tile on a <see cref="PlanetGrid"/></summary>
internal class GridTile(GridRow row, PolarCoordinate longitude)
{
    /// <summary><see cref="GridRow"/> corresponding to the latitude (north/south) coordinate</summary>
    public readonly GridRow LatitudeRow = row;

    /// <summary>Longitude (east/west) coordinate)</summary>
    public readonly PolarCoordinate Longitude = longitude;

    /// <summary>Latitude (north/south) coordinate</summary>
    public PolarCoordinate Latitude => LatitudeRow.Latitude;

    /// <summary>Calculate from longitude <see cref="PolarCoordinate.Angle"/></summary>
    /// <param name="row"><see cref="GridRow"/></param>
    /// <param name="longAngle">Longitude <see cref="PolarCoordinate.Angle"/></param>
    /// <returns>New <see cref="GridTile"/></returns>
    public static GridTile FromLongitudeAngle(GridRow row, float longAngle)
        => new(row, PolarCoordinate.FromAngle(longAngle, row.LongitudeSegments));

    /// <summary>Calculate from longitude <see cref="PolarCoordinate.Element"/></summary>
    /// <param name="row"><see cref="GridRow"/></param>
    /// <param name="longElement">Longitude <see cref="PolarCoordinate.Element"/></param>
    /// <returns>New <see cref="GridTile"/></returns>
    public static GridTile FromLongitudeElement(GridRow row, int longElement)
        => new(row, PolarCoordinate.FromElement(longElement, row.LongitudeSegments));

    /// <summary>Calculate from a planetary position</summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="position">Position relative to the planet's center</param>
    /// <returns>New <see cref="GridTile"/></returns>
    public static GridTile FromPosition(PlanetGrid grid, Vector3 position)
    {
        Vector3 normalized = position.normalized;
        float latAngle = Mathf.Asin(normalized.y);
        float longAngle = Mathf.Atan2(normalized.x, 0f - normalized.z);

        GridRow row = GridRow.FromLatitudeAngle(grid, latAngle);
        return FromLongitudeAngle(row, longAngle);
    }

    /// <summary>Calculate the planetary position</summary>
    /// <returns>Position relative to the planet's center</returns>
    public Vector3 CalculatePosition()
    {
        float latSin = Mathf.Sin(Latitude.Angle);
        float latCos = Mathf.Cos(Latitude.Angle);
        float longSin = Mathf.Sin(Longitude.Angle);
        float longCos = Mathf.Cos(Longitude.Angle);
        return new Vector3(latCos * longSin, latSin, latCos * (0f - longCos));
    }

    /// <inheritdoc />
    public override string ToString()
        => $"({Latitude.Element},{Longitude.Element})";
}

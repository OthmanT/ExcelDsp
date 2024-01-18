using ExcelDsp.Painter.Extensions;

namespace ExcelDsp.Painter.Grids;

/// <summary>Calculate from <see cref="PolarCoordinate"/></summary>
/// <param name="grid"><see cref="PlanetGrid"/></param>
/// <param name="latitude">Latitude <see cref="PolarCoordinate"/></param>
/// <remarks>Based on <see cref="PlanetGrid.ReformSnapTo"/></remarks>
internal class GridRow(PlanetGrid grid, PolarCoordinate latitude)
{
    /// <summary>Latitude (north/south) coordinate</summary>
    public readonly PolarCoordinate Latitude = latitude;

    /// <summary>Number of longitude <see cref="PolarCoordinate.Segment"/> values at the current latitude</summary>
    /// <remarks>Largest at equator, decreasing towards each pole</remarks>
    public readonly int LongitudeSegments = grid.CalculateLongitudeSegments(latitude.Segment);

    /// <summary>Calculate from latitude <see cref="PolarCoordinate.Angle"/></summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="latAngle">Latitude <see cref="PolarCoordinate.Angle"/></param>
    /// <returns>New <see cref="GridRow"/></returns>
    public static GridRow FromLatitudeAngle(PlanetGrid grid, float latAngle)
        => new(grid, PolarCoordinate.FromAngle(latAngle, grid.segment));

    /// <summary>Calculate from latitude <see cref="PolarCoordinate.Element"/></summary>
    /// <param name="grid"><see cref="PlanetGrid"/></param>
    /// <param name="latElement">Latitude <see cref="PolarCoordinate.Element"/></param>
    /// <returns>New <see cref="GridRow"/></returns>
    public static GridRow FromLatitudeElement(PlanetGrid grid, int latElement)
        => new(grid, PolarCoordinate.FromElement(latElement, grid.segment));

    /// <summary>Check if the latitude is valid</summary>
    /// <param name="latSegmentMax">Maximum latitude segment value</param>
    /// <returns>Whether this represents a usable tile on the grid</returns>
    public bool IsValid(float latSegmentMax)
    {
        return Latitude.IsValid
            && Latitude.Segment < latSegmentMax
            && Latitude.Segment > -latSegmentMax;
    }
}

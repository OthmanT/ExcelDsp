using UnityEngine;

namespace ExcelDsp.Painter.Extensions;

/// <summary>Extensions for <see cref="PlanetAuxData"/></summary>
internal static class PlanetAuxDataExtensions
{
    /// <summary>Transform <see cref="PlanetGridExtensions.ReformSnapRect"/> outputs into planetary surface points</summary>
    /// <param name="aux"><see cref="PlanetAuxData"/></param>
    /// <param name="startPos">Rectangle start position</param>
    /// <param name="endPos">Rectangle end position</param>
    /// <param name="useShortestPath">Whether to use the shortest or longest arc path between the endpoints</param>
    /// <param name="reformPoints">Output points on the planetary surface that require reformation (excludes already reformed points)</param>
    /// <param name="reformIndices">Output reformation indices in the rectangle</param>
    /// <param name="reformCenter">Center point on the planetary surface</param>
    /// <returns>Number of valid <paramref name="reformPoints"/></returns>
    public static int ReformSnapRect(this PlanetAuxData aux, Vector3 startPos, Vector3 endPos, bool useShortestPath, ref Vector3[] reformPoints, ref int[] reformIndices, out Vector3 reformCenter)
    {
        int num = aux.mainGrid.ReformSnapRect(aux.planet.factory.platformSystem, startPos, endPos, useShortestPath, ref reformPoints, ref reformIndices, out reformCenter);
        float num2 = aux.planet.radius + 0.2f;
        for(int i = 0; i < num; i++)
        {
            reformPoints[i].x *= num2;
            reformPoints[i].y *= num2;
            reformPoints[i].z *= num2;
        }

        reformCenter *= num2;
        return num;
    }
}

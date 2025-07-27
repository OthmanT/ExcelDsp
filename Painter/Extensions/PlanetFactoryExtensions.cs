using ExcelDsp.Painter.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelDsp.Painter.Extensions;

/// <summary>Extensions for <see cref="PlanetFactory"/></summary>
internal static class PlanetFactoryExtensions
{
    /// <summary>Compute the sand required for the previous <see cref="PlanetFactory.BeginFlattenTerrain"/> call</summary>
    /// <param name="factory"><see cref="PlanetFactory"/></param>
    /// <returns>Calculated sand (soil pile) cost/gain</returns>
    /// <remarks>Based on <see cref="PlanetFactory.ComputeFlattenTerrainReform"/></remarks>
    public static int EndComputeFlattenTerrain(this PlanetFactory factory)
    {
        PlanetData planet = factory.planet;
        PlanetRawData planetRaw = planet.data;
        int totalSand = 0;

        foreach(KeyValuePair<int, int> change in PlanetFactory_Patch.TmpLevelChanges)
        {
            int vertexIndex = change.Key;
            int newLevel = change.Value;

            int oldLevel = planetRaw.GetModLevel(vertexIndex);
            int levelDiff = newLevel - oldLevel;

            float heightOffset = planetRaw.heightData[vertexIndex] * 0.01f;
            float heightFull = planet.realRadius + 0.2f - heightOffset;

            if(heightFull < 0f)
                heightFull *= 2f;

            float vertexSand = 100f * levelDiff * heightFull * 0.3333333f;
            totalSand += Mathf.FloorToInt(vertexSand);
        }

        return totalSand;
    }
}

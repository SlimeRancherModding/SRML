using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.Utils
{
    public static class RegionUtils
    {
        public static List<Region> GetRegionsFromPosition(Vector3 pos, SceneContext s)
        {

            List<Region> regions = new List<Region>();

            foreach (var region in s.RegionRegistry.regionsTrees)
            {
                if (regions.Count > 0) break;
                region.Value.GetColliding(pos, ref regions);
            }

            return regions;
        }

        public static List<Region> GetRegionsFromPosition(Vector3 pos)
        {
            return GetRegionsFromPosition(pos, SceneContext.Instance);
        }

        public static RegionRegistry.RegionSetId GetLikelySetIdFromPosition(Vector3 pos, SceneContext s)
        {
            return GetRegionsFromPosition(pos, s).FirstOrDefault()?.setId ?? RegionRegistry.RegionSetId.UNSET;
        }

        public static RegionRegistry.RegionSetId GetLikelySetIdFromPosition(Vector3 pos)
        {
            return GetLikelySetIdFromPosition(pos, SceneContext.Instance);
        }
    }
}

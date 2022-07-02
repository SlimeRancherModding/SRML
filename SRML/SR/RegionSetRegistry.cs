using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;

namespace SRML.SR
{
    public static class RegionSetRegistry
    {
        internal static Dictionary<RegionRegistry.RegionSetId, BoundsQuadtree<Region>> customTrees = new Dictionary<RegionRegistry.RegionSetId, BoundsQuadtree<Region>>();

        public static void RegisterRegion(RegionRegistry.RegionSetId region, BoundsQuadtree<Region> bounds) => customTrees[region] = bounds;
    }
}

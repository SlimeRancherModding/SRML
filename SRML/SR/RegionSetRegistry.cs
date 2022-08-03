using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;

namespace SRML.SR
{
    public static class RegionSetRegistry
    {
        internal static Dictionary<RegionRegistry.RegionSetId, BoundsQuadtree<Region>> customTrees = new Dictionary<RegionRegistry.RegionSetId, BoundsQuadtree<Region>>();
        internal static Dictionary<ZoneDirector.Zone, RegionRegistry.RegionSetId> regionForZones = new Dictionary<ZoneDirector.Zone, RegionRegistry.RegionSetId>();

        public static void RegisterRegion(RegionRegistry.RegionSetId region, BoundsQuadtree<Region> bounds) => customTrees[region] = bounds;

        public static void RegisterZoneIntoRegion(ZoneDirector.Zone zone, RegionRegistry.RegionSetId region) => regionForZones[zone] = region;
    }
}

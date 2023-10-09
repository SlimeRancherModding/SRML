using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;

namespace SRML.SR
{
    public static class RegionSetRegistry
    {
        internal static Dictionary<RegionRegistry.RegionSetId, BoundsQuadtree<Region>> customTrees = new Dictionary<RegionRegistry.RegionSetId, BoundsQuadtree<Region>>();
        internal static Dictionary<ZoneDirector.Zone, RegionRegistry.RegionSetId> regionForZones = new Dictionary<ZoneDirector.Zone, RegionRegistry.RegionSetId>();

        /// <summary>
        /// Registers a region into the <see cref="RegionRegistry"/>.
        /// </summary>
        /// <param name="region">The <see cref="RegionRegistry.RegionSetId"/> of the region.</param>
        /// <param name="bounds">The bounds and values of the region.</param>
        public static void RegisterRegion(RegionRegistry.RegionSetId region, BoundsQuadtree<Region> bounds) => customTrees[region] = bounds;

        /// <summary>
        /// Adds a zone into a region.
        /// </summary>
        /// <param name="zone">The <see cref="ZoneDirector.Zone"/> to be added.</param>
        /// <param name="region">The <see cref="RegionRegistry.RegionSetId"/> of the region to add to.</param>
        public static void RegisterZoneIntoRegion(ZoneDirector.Zone zone, RegionRegistry.RegionSetId region) => regionForZones[zone] = region;
    }
}

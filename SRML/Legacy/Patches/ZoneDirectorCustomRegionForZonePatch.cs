using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(ZoneDirector), "GetRegionSetId")]
    internal static class ZoneDirectorCustomRegionForZonePatch
    {
        public static bool Prefix(ZoneDirector.Zone zone, ref RegionRegistry.RegionSetId __result)
        {
            if (RegionSetRegistry.regionForZones.TryGetValue(zone, out RegionRegistry.RegionSetId val))
            {
                __result = val;
                return false;
            }
            return true;
        }
    }
}

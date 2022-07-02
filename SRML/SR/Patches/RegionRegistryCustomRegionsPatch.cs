using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using System.Linq;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(RegionRegistry), "Awake")]
    internal static class RegionRegistryCustomRegionsPatch
    {
        public static void Prefix(RegionRegistry __instance) => __instance.regionsTrees = 
            __instance.regionsTrees.Union(RegionSetRegistry.customTrees).ToDictionary(x => x.Key, y => y.Value);
    }
}

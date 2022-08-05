using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using System.Linq;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(RegionRegistry), "RegisterRegion")]
    internal static class RegionRegistryCustomRegionsPatch
    {
        public static void Prefix(RegionRegistry __instance)
        {
            if (RegionSetRegistry.customTrees.Count() == 0 || __instance.regionsTrees.ContainsKey(RegionSetRegistry.customTrees.First().Key)) 
                return;

            __instance.regionsTrees = __instance.regionsTrees.Union(RegionSetRegistry.customTrees).ToDictionary(x => x.Key, y => y.Value);
        }
    }
}

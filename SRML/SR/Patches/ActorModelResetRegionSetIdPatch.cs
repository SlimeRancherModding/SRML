using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(ActorModel), MethodType.Constructor, 
        new[] { typeof(long), typeof(Identifiable.Id), typeof(RegionRegistry.RegionSetId), typeof(Transform) } )]
    internal static class ActorModelResetRegionSetIdPatch
    {
        public static void Postfix(ActorModel __instance)
        {
            if (!SceneContext.Instance.RegionRegistry.regionsTrees.ContainsKey(__instance.currRegionSetId) &&
                !RegionSetRegistry.customTrees.ContainsKey(__instance.currRegionSetId))
                __instance.currRegionSetId = MonomiPark.SlimeRancher.Regions.RegionRegistry.RegionSetId.HOME;
        }
    }
}

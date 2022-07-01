using HarmonyLib;
using SRML.SR.Utils;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SlimeDiet), "RefreshEatMap")]
    internal class SlimeDietRefreshEatMapAddExtrasPatch
    {
        public static void Postfix(SlimeDiet __instance, SlimeDefinition definition)
        {
            if (SlimeExtensions.extraEatEntries.ContainsKey(definition.IdentifiableId))
                __instance.EatMap.AddRange(SlimeExtensions.extraEatEntries[definition.IdentifiableId]);
        }
    }
}

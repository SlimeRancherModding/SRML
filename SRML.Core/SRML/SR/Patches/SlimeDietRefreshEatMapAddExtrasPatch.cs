using HarmonyLib;
using SRML.SR.Utils;
using System.Collections.Generic;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SlimeDiet), "RefreshEatMap")]
    internal class SlimeDietRefreshEatMapAddExtrasPatch
    {
        public static void Postfix(SlimeDiet __instance, SlimeDefinition definition)
        {
            if (SlimeExtensions.extraEatEntries.TryGetValue(definition.IdentifiableId, out List<SlimeDiet.EatMapEntry> ents))
                __instance.EatMap.AddRange(ents);
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GordoSnare), "AttachBait")]
    internal static class GordoSnarePatch
    {
        public static void Postfix(GordoSnare __instance)
        {
            __instance.RemoveComponent<SlimeEmotions>(__instance.bait);
            __instance.RemoveComponent<SlimeFaceAnimator>(__instance.bait);
            __instance.RemoveComponent<SlimeEat>(__instance.bait);
            __instance.RemoveComponent<SlimeEatAsh>(__instance.bait);
            __instance.RemoveComponent<SlimeEatWater>(__instance.bait);
            __instance.RemoveComponent<SlimeEatTrigger>(__instance.bait);
            __instance.RemoveComponents<SlimeSubbehaviour>(__instance.bait);
            __instance.RemoveComponent<DestroyPlortAfterTime>(__instance.bait);
            __instance.RemoveComponent<PlortInvulnerability>(__instance.bait);
            __instance.RemoveComponent<PlaySoundOnHit>(__instance.bait);
            __instance.RemoveComponent<DestroyOnIgnite>(__instance.bait);
        }
    }
}

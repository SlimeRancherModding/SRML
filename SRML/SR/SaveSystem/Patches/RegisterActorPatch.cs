using Harmony;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(GameModel))]
    [HarmonyPatch("RegisterActor")]
    internal static class RegisterActorPatch
    {
        public static void Postfix(GameModel __instance, long actorId, GameObject gameObj,bool skipNotify)
        {
            ExtendedData.OnRegisterActor(__instance, actorId, gameObj, skipNotify);
        }

        public static void Prefix(GameModel __instance, long actorId, GameObject gameObj, bool skipNotify)
        {
            var potentialTag = PersistentAmmoManager.GetPotentialDataTag(gameObj);
            if (potentialTag != null)
            {
                ExtendedData.extendedActorData[actorId] = potentialTag;
                
            }
        }
    }

    [HarmonyPatch(typeof(GameModel))]
    [HarmonyPatch("DestroyActorModel")]
    internal static class DestroyActorPatch
    {
        public static void Prefix(GameObject gameObj)
        {
            ExtendedData.DestroyActor(gameObj);
        }
    }
}

using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data;
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
                ExtendedData.preparedData[DataIdentifier.GetActorIdentifier(actorId)] = new ExtendedData.PreparedData() 
                { 
                    SourceType = ExtendedData.PreparedData.PreparationSource.AMMO, 
                    Data = potentialTag 
                };
            else
                DronePersistentAmmoManager.OnActorSpawned(gameObj, actorId);
        }
    }

    [HarmonyPatch(typeof(GameModel))]
    [HarmonyPatch("DestroyActorModel")]
    internal static class DestroyActorPatch
    {
        public static void Prefix(GameObject gameObj)
        {
            //ExtendedData.DestroyActor(gameObj);
        }
    }
}

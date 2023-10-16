using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GameModel))]
    [HarmonyPatch("RegisterActor")]
    [HarmonyPriority(1000)]
    internal static class RegisterActorPatch
    {
        public static void Postfix(GameObject gameObj)
        {
            var ident = gameObj.GetComponent<Identifiable>();
            if (ident) SRCallbacks.OnActorSpawnCallback(ident.id, gameObj, ident.model);
        }
    }
}

using Harmony;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GameModel))]
    [HarmonyPatch("CreateActorModel")]
    public class GameModelActorCreatePatch
    {
        public static bool Prefix(out ActorModel __result, GameModel __instance, long actorId, Identifiable.Id ident,
            GameObject gameObj)
        {
            __result = null;
            var g = DataModelRegistry.actorOverrideMapping.FirstOrDefault((x) => x.Key(ident));
            if (g.Value==null)
                return true;

            __result = g.Value(actorId, ident, gameObj);
            return false;
        }
    }
}

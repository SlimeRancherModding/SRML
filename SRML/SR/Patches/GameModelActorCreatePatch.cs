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
            var _override = DataModelRegistry.actorOverrideMapping.FirstOrDefault((x) => x.Key(ident));
            if (_override.Value==null)
                return true;

            __result = _override.Value(actorId, ident, gameObj);
            return false;
        }
    }
}

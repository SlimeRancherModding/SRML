using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
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
        public static bool Prefix(out ActorModel __result, GameModel __instance, long actorId, Identifiable.Id ident, RegionRegistry.RegionSetId regionSetId,
            GameObject gameObj)
        {
            __result = null;
            var _override = DataModelRegistry.actorOverrideMapping.FirstOrDefault((x) => x.Key(ident));
            if (_override.Value==null)
                return true;

            __result = _override.Value(actorId, ident, regionSetId, gameObj);
            return false;
        }
    }
}

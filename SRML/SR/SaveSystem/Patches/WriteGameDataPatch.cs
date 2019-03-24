using System.Collections.Generic;
using Harmony;
using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(GameV09))]
    [HarmonyPatch("WriteGameData")]
    internal static class WriteGameDataPatch
    {
        public static void Prefix(GameV09 __instance, ref List<VanillaActorData> __state)
        {
            __state = new List<VanillaActorData>();

            foreach (var v in __instance.actors)
            {
                if (SaveRegistry.IsCustom((VanillaActorData) v))
                {
                    __state.Add((VanillaActorData)v);
                }
            }

            foreach (var v in __state)
            {
                __instance.actors.Remove(v);
            }
        }

        public static void Postfix(GameV09 __instance, ref List<VanillaActorData> __state)
        {
            LogUtils.Log($"we have {__state.Count} things to add back in gamev09");
            foreach (var v in __state)
            {
                __instance.actors.Add(v);
            }
        }
    }
}

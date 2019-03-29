using System.Collections.Generic;
using Harmony;
using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV06;
using VanillaPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;
namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(GameV09))]
    [HarmonyPatch("WriteGameData")]
    internal static class WriteGameDataPatch
    {
        public static void Prefix(GameV09 __instance, ref RemovalData __state)
        {
            __state = new RemovalData();

            foreach (var v in __instance.actors)
            {
                if (SaveRegistry.IsCustom(v))
                {
                    __state.actors.Add(v);
                }
            }

            foreach (var v in __instance.world.placedGadgets)
            {
                if (SaveRegistry.IsCustom(v.Value))
                {
                    __state.gadgets.Add(v.Key, v.Value);
                }
            }

            foreach (var v in __state.actors)
            {
                __instance.actors.Remove(v);
            }

            foreach (var v in __state.gadgets)
            {
                __instance.world.placedGadgets.Remove(v.Key);
            }

            foreach (var v in __state.landplots)
            {
                __instance.ranch.plots.Remove(v);
            }
        }

        public static void Postfix(GameV09 __instance, ref RemovalData __state)
        {
            foreach (var v in __state.actors)
            {
                __instance.actors.Add(v);
            }

            foreach (var v in __state.gadgets)
            {
                __instance.world.placedGadgets[v.Key] = v.Value;
            }

            foreach (var v in __state.landplots)
            {
                __instance.ranch.plots.Add(v);
            }
        }

        public class RemovalData
        {
            public List<VanillaActorData> actors = new List<VanillaActorData>();
            public Dictionary<string,VanillaGadgetData> gadgets = new Dictionary<string, PlacedGadgetV06>();
            public List<VanillaPlotData> landplots = new List<VanillaPlotData>();
        }
    }
}

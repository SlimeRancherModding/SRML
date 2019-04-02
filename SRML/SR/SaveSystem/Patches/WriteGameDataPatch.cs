using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Utils;
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

            __state.AddAndRemoveWhereCustom(__instance.actors,__state.actors);
            __state.AddAndRemoveWhere(__instance.world.placedGadgets,__state.placedGadgets,(x)=>SaveRegistry.IsCustom(x.Value));
            __state.AddAndRemoveWhereCustom(__instance.ranch.plots,__state.landplots);

            __state.AddAndRemoveWhereCustom(__instance.player.upgrades,__state.upgrades);
            __state.AddAndRemoveWhereCustom(__instance.player.availUpgrades, __state.availUpgrades);
            __state.AddAndRemoveWhere(__instance.player.upgradeLocks, __state.upgradeLocks,
                (x) => SaveRegistry.IsCustom(x.Key));

            __state.AddAndRemoveWhereCustom(__instance.player.blueprints,__state.blueprints);
            __state.AddAndRemoveWhereCustom(__instance.player.availBlueprints,__state.availBlueprints);
            __state.AddAndRemoveWhere(__instance.player.blueprintLocks,__state.blueprintLocks,(x)=>SaveRegistry.IsCustom(x.Key));

            __state.AddAndRemoveWhere(__instance.player.progress,__state.progress,(x)=>SaveRegistry.IsCustom(x.Key));
            __state.AddAndRemoveWhere(__instance.player.delayedProgress,__state.delayedProgress,(x)=>SaveRegistry.IsCustom(x.Key));

            __state.AddAndRemoveWhere(__instance.player.gadgets,__state.gadgets,(x)=>SaveRegistry.IsCustom(x.Key));

            __state.AddAndRemoveWhere(__instance.player.craftMatCounts,__state.craftMatCounts,(x)=>SaveRegistry.IsCustom(x.Key));

            __state.AddAndRemoveWhere(__instance.pedia.unlockedIds,__state.unlockedIds,(x)=>SaveRegistry.IsCustom(Enum.Parse(typeof(PediaDirector.Id),x)));
            __state.AddAndRemoveWhere(__instance.pedia.completedTuts, __state.completedTuts, (x) => SaveRegistry.IsCustom(Enum.Parse(typeof(TutorialDirector.Id), x)));
            __state.AddAndRemoveWhere(__instance.pedia.popupQueue, __state.popupQueue, (x) => SaveRegistry.IsCustom(Enum.Parse(typeof(TutorialDirector.Id), x)));

            foreach (var data in AmmoDataUtils.GetAllAmmoData(__instance))
            {
                var moddedData = AmmoDataUtils.RipOutModdedData(data);
                __state.addBacks.Add(() =>
                {
                    AmmoDataUtils.SpliceAmmoData(data,moddedData);
                });
            }
        }

        public static void Postfix(GameV09 __instance, ref RemovalData __state)
        {
            __state.AddAllBack();
        }





        public class RemovalData
        {
            public List<VanillaActorData> actors = new List<VanillaActorData>();
            public Dictionary<string,VanillaGadgetData> placedGadgets = new Dictionary<string, PlacedGadgetV06>();
            public List<VanillaPlotData> landplots = new List<VanillaPlotData>();

            public List<PlayerState.Upgrade> upgrades = new List<PlayerState.Upgrade>();
            public List<PlayerState.Upgrade> availUpgrades = new List<PlayerState.Upgrade>();
            public Dictionary<PlayerState.Upgrade, PlayerState.UpgradeLockData> upgradeLocks = new Dictionary<PlayerState.Upgrade, PlayerState.UpgradeLockData>();

            public Dictionary<ProgressDirector.ProgressType, int> progress =
                new Dictionary<ProgressDirector.ProgressType, int>();
            public Dictionary<ProgressDirector.ProgressTrackerId, double> delayedProgress = new Dictionary<ProgressDirector.ProgressTrackerId, double>();

            public List<Gadget.Id> blueprints = new List<Gadget.Id>();
            public List<Gadget.Id> availBlueprints = new List<Gadget.Id>();
            public Dictionary<Gadget.Id, GadgetDirector.BlueprintLockData> blueprintLocks = new Dictionary<Gadget.Id, GadgetDirector.BlueprintLockData>();

            public Dictionary<Gadget.Id, int> gadgets = new Dictionary<Gadget.Id, int>();

            public Dictionary<Identifiable.Id, int> craftMatCounts = new Dictionary<Identifiable.Id, int>();

            public List<string> unlockedIds = new List<string>();
            public List<string> completedTuts = new List<string>();
            public List<string> popupQueue = new List<string>();

            public List<Action> addBacks = new List<Action>();

            public void AddAndRemoveWhere<K, V>(Dictionary<K, V> original, Dictionary<K, V> buffer,
                Predicate<KeyValuePair<K, V>> cond)
            {
                foreach (var pair in original.Where((x) => cond(x)))
                {
                    buffer.Add(pair.Key, pair.Value);
                }

                foreach (var pair in buffer)
                {
                    original.Remove(pair.Key);
                }

                addBacks.Add(() =>
                {
                    foreach (var v in buffer)
                    {
                        original.Add(v.Key, v.Value);
                    }
                });
            }

            public void AddAndRemoveWhere<T>(List<T> original, List<T> buffer, Predicate<T> cond)
            {
                buffer.AddRange(original.Where((x)=>cond(x)));
                foreach (var v in buffer)
                {
                    original.Remove(v);
                }

                addBacks.Add(() => original.AddRange(buffer));
            }

            public void AddAndRemoveWhereCustom<T>(List<T> original, List<T> buffer)
            {
                buffer.AddRange(original.Where((x) => SaveRegistry.IsCustom(x)));
                foreach (var v in buffer)
                {
                    original.Remove(v);
                }

                addBacks.Add(() => original.AddRange(buffer));
            }

            public void AddAllBack()
            {
                foreach (Action action in addBacks)
                {
                    action();
                }
            }
        }
    }
}

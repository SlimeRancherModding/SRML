using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Ammo;
using SRML.SR.SaveSystem.Data.Appearances;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Utils;
using SRML.Utils;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV09;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV08;
using VanillaPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;
namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(GameV11))]
    [HarmonyPatch("WriteGameData")]
    internal static class WriteGameDataPatch
    {
        public static void Prefix(GameV11 __instance, ref RemovalData __state)
        {
            __state = new RemovalData();

            __state.AddAndRemoveWhereCustom(__instance.actors,__state.actors);
            __state.AddAndRemoveWhere(__instance.world.placedGadgets,__state.placedGadgets,(x)=>SaveRegistry.IsCustom(x.Value)||ModdedStringRegistry.IsModdedString(x.Key));
            __state.AddAndRemoveWhere(__instance.ranch.plots,__state.landplots,(x)=>SaveRegistry.IsCustom(x)||ModdedStringRegistry.IsModdedString(x.id));
            __state.AddAndRemoveWhere(__instance.world.gordos, new Dictionary<string, GordoV01>(), x => SaveRegistry.IsCustom(x.Value) || ModdedStringRegistry.IsModdedString(x.Key));
            __state.AddAndRemoveWhere(__instance.world.treasurePods, new Dictionary<string, TreasurePodV01>(), x => SaveRegistry.IsCustom(x.Value) || ModdedStringRegistry.IsModdedString(x.Key));
            __state.AddAndRemoveWhere(__instance.world.offers, new Dictionary<ExchangeDirector.OfferType, ExchangeOfferV04>(), x => SaveRegistry.IsCustom(x.Value) || ModdedIDRegistry.IsModdedID(x.Key) || ExchangeOfferRegistry.IsCustom(x.Value));
            __state.AddAndRemoveWhere(__instance.world.econSaturations, new Dictionary<Identifiable.Id, float>(), (x) => ModdedIDRegistry.IsModdedID(x.Key));


            __state.AddAndRemoveWhereCustom(__instance.player.upgrades,__state.upgrades);
            __state.AddAndRemoveWhereCustom(__instance.player.availUpgrades, __state.availUpgrades);
            __state.AddAndRemoveWhere(__instance.player.upgradeLocks, __state.upgradeLocks,
                (x) => ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhereCustom(__instance.player.blueprints,__state.blueprints);
            __state.AddAndRemoveWhereCustom(__instance.player.availBlueprints,__state.availBlueprints);
            __state.AddAndRemoveWhere(__instance.player.blueprintLocks,__state.blueprintLocks,(x)=> ModdedIDRegistry.IsModdedID(x.Key));
            
            __state.AddAndRemoveWhere(__instance.player.progress,__state.progress,(x)=> ModdedIDRegistry.IsModdedID(x.Key));
            __state.AddAndRemoveWhere(__instance.player.delayedProgress,__state.delayedProgress,(x)=> ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhere(__instance.player.gadgets,__state.gadgets,(x)=> ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhere(__instance.player.craftMatCounts,__state.craftMatCounts,(x)=> ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhereCustom(__instance.player.unlockedZoneMaps, __state.unlockedZoneMaps);

            __state.AddAndRemoveWhere(__instance.player.mail, __state.mail, (x) => MailRegistry.GetModForMail(x.messageKey) != null);

            __state.AddAndRemoveWhere(__instance.pedia.unlockedIds,__state.unlockedIds,(x)=> ModdedIDRegistry.IsModdedID(Enum.Parse(typeof(PediaDirector.Id),x)));
            __state.AddAndRemoveWhere(__instance.pedia.completedTuts, __state.completedTuts, (x) => ModdedIDRegistry.IsModdedID(Enum.Parse(typeof(TutorialDirector.Id), x)));
            __state.AddAndRemoveWhere(__instance.pedia.popupQueue, __state.popupQueue, (x) => ModdedIDRegistry.IsModdedID(Enum.Parse(typeof(TutorialDirector.Id), x)));

            

            foreach (var data in AmmoDataUtils.GetAllAmmoData(__instance))
            {
                if (AmmoIdentifier.TryGetIdentifier(data, __instance, out var id)&&AmmoIdentifier.IsModdedIdentifier(id))
                {
                    __state.addBacks.Add(AmmoDataUtils.RemoveAmmoDataWithAddBack(data, __instance));
                }
                else
                {
                    var moddedData = AmmoDataUtils.RipOutModdedData(data);

                    __state.addBacks.Add(() =>
                    {
                        AmmoDataUtils.SpliceAmmoData(data, moddedData);
                    });
                }
            }

            void RemovePartial(object actor,RemovalData data)
            {
                if (CustomChecker.GetCustomLevel(actor) == CustomChecker.CustomLevel.PARTIAL)
                {
                    var partial = PartialData.GetPartialData(actor.GetType(), true);
                    partial.Pull(actor);
                    data.addBacks.Add(() =>
                    {
                        partial.Push(actor);
                    });
                }
            }

            foreach (var actor in __instance.actors)
            {
                RemovePartial(actor,__state);
            }

            foreach (var actor in __instance.ranch.plots)
            {
                RemovePartial(actor, __state);
            }

            foreach (var actor in __instance.world.placedGadgets)
            {
                RemovePartial(actor.Value, __state);
            }

            foreach(var actor in __instance.world.gordos)
            {
                RemovePartial(actor.Value, __state);
            }

            foreach(var actor in __instance.world.treasurePods)
            {
                RemovePartial(actor.Value, __state);
            }

            foreach(var offer in __instance.world.offers)
            {
                RemovePartial(offer.Value, __state);
            }

            var partialAppearance = new PartialAppearancesData();
            partialAppearance.Pull(__instance.appearances);
            __state.addBacks.Add(() => partialAppearance.Push(__instance.appearances));
        }

        public static void Postfix(GameV11 __instance, ref RemovalData __state)
        {
            __state.AddAllBack();
        }





        public class RemovalData
        {
            public List<VanillaActorData> actors = new List<VanillaActorData>();
            public Dictionary<string,VanillaGadgetData> placedGadgets = new Dictionary<string, PlacedGadgetV08>();
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

            public List<ZoneDirector.Zone> unlockedZoneMaps =new List<ZoneDirector.Zone>();

            public List<MailV02> mail = new List<MailV02>();

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
                buffer.AddRange(original.Where((x) => x.GetType().IsEnum? ModdedIDRegistry.IsModdedID(x):SaveRegistry.IsCustom(x)));
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

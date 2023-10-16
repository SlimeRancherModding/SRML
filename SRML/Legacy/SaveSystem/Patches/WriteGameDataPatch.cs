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
    [HarmonyPatch(typeof(GameV12))]
    [HarmonyPatch("WriteGameData")]
    internal static class WriteGameDataPatch
    {
        public static void Prefix(GameV12 __instance, ref RemovalData __state)
        {
            __state = new RemovalData();

            __state.AddAndRemoveWhereCustom(__instance.actors);
            __state.AddAndRemoveWhere(__instance.world.placedGadgets,(x)=>SaveRegistry.IsCustom(x.Value)||ModdedStringRegistry.IsModdedString(x.Key));
            __state.AddAndRemoveWhere(__instance.ranch.plots,(x)=>SaveRegistry.IsCustom(x)||ModdedStringRegistry.IsModdedString(x.id));
            __state.AddAndRemoveWhere(__instance.world.gordos, x => SaveRegistry.IsCustom(x.Value) || ModdedStringRegistry.IsModdedString(x.Key));
            __state.AddAndRemoveWhere(__instance.world.treasurePods, x => SaveRegistry.IsCustom(x.Value) || ModdedStringRegistry.IsModdedString(x.Key));
            __state.AddAndRemoveWhere(__instance.world.offers, x => SaveRegistry.IsCustom(x.Value) || ModdedIDRegistry.IsModdedID(x.Key) || ExchangeOfferRegistry.IsCustom(x.Value));
            __state.AddAndRemoveWhere(__instance.world.econSaturations, (x) => ModdedIDRegistry.IsModdedID(x.Key));
            __state.AddAndRemoveWhere(__instance.world.lastOfferRancherIds, ExchangeOfferRegistry.IsCustom);
            __state.AddAndRemoveWhere(__instance.world.pendingOfferRancherIds, ExchangeOfferRegistry.IsCustom);

            __state.AddAndRemoveWhereCustom(__instance.player.upgrades);
            __state.AddAndRemoveWhereCustom(__instance.player.availUpgrades);
            __state.AddAndRemoveWhere(__instance.player.upgradeLocks,
                (x) => ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhereCustom(__instance.player.blueprints);
            __state.AddAndRemoveWhereCustom(__instance.player.availBlueprints);
            __state.AddAndRemoveWhere(__instance.player.blueprintLocks,(x)=> ModdedIDRegistry.IsModdedID(x.Key));
            
            __state.AddAndRemoveWhere(__instance.player.progress,(x)=> ModdedIDRegistry.IsModdedID(x.Key));
            __state.AddAndRemoveWhere(__instance.player.delayedProgress,(x)=> ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhere(__instance.player.gadgets,(x)=> ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhere(__instance.player.craftMatCounts,(x)=> ModdedIDRegistry.IsModdedID(x.Key));

            __state.AddAndRemoveWhereCustom(__instance.player.unlockedZoneMaps);

            __state.AddAndRemoveWhere(__instance.player.mail, (x) => MailRegistry.GetModForMail(x.messageKey) != null);

            __state.AddAndRemoveWhere(__instance.pedia.unlockedIds,(x)=> ModdedIDRegistry.IsModdedID(Enum.Parse(typeof(PediaDirector.Id),x)));
            __state.AddAndRemoveWhere(__instance.pedia.completedTuts,  (x) => ModdedIDRegistry.IsModdedID(Enum.Parse(typeof(TutorialDirector.Id), x)));
            __state.AddAndRemoveWhere(__instance.pedia.popupQueue, (x) => ModdedIDRegistry.IsModdedID(Enum.Parse(typeof(TutorialDirector.Id), x)));

            

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

            public List<Action> addBacks = new List<Action>();

            public void AddAndRemoveWhere<K, V>(Dictionary<K, V> original,
                Predicate<KeyValuePair<K, V>> cond)
            {
                var buffer = new Dictionary<K, V>();
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

            public void AddAndRemoveWhere<T>(List<T> original, Predicate<T> cond)
            {
                var buffer = new List<T>();
                buffer.AddRange(original.Where((x)=>cond(x)));
                foreach (var v in buffer)
                {
                    original.Remove(v);
                }

                addBacks.Add(() => original.AddRange(buffer));
            }

            public void AddAndRemoveWhereCustom<T>(List<T> original)
            {
                var buffer = new List<T>();
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

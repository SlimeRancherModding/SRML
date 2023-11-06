using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.API.Identifiable.Slime
{
    [HarmonyPatch]
    public class FashionSlotRegistry : EnumRegistry<FashionSlotRegistry, Fashion.Slot>
    {
        protected Dictionary<Fashion.Slot, (SlimeAppearance.SlimeBone, string)> moddedSlots = 
            new Dictionary<Fashion.Slot, (SlimeAppearance.SlimeBone, string)>();

        public delegate void FashionSlotRegistrationEvent(Fashion.Slot slot, SlimeAppearance.SlimeBone bone, string path);
        public FashionSlotRegistrationEvent OnRegisterFashionSlot;

        [HarmonyPatch(typeof(AttachFashions), "GetParentForSlot")]
        [HarmonyPrefix]
        internal static bool RegisterFashionSlotPath(AttachFashions __instance, Fashion.Slot slot, ref Transform __result) =>
            Instance.GetNonSlimeSlotPath(__instance, slot, ref __result);

        [HarmonyPatch(typeof(SlimeAppearanceApplicator), "GetFashionParent")]
        [HarmonyPrefix]
        internal static bool RegisterFashionSlotBone(SlimeAppearanceApplicator __instance, Fashion.Slot fashionSlot, ref Transform __result) =>
            Instance.GetSlimeSlotPath(__instance, fashionSlot, ref __result);

        public virtual bool GetNonSlimeSlotPath(AttachFashions fashions, Fashion.Slot slot, ref Transform parent)
        {
            if (global::Identifiable.IsSlime(global::Identifiable.GetId(fashions.gameObject)))
                return true;
            
            if (!moddedSlots.ContainsKey(slot))
                return true;

            string path = moddedSlots[slot].Item2;
            if (path != null)
            {
                parent = fashions.transform.Find(path);
                return false;
            }

            return true;
        }
        public virtual bool GetSlimeSlotPath(SlimeAppearanceApplicator appearanceApplicator, Fashion.Slot slot, ref Transform parent)
        {
            if (!moddedSlots.ContainsKey(slot))
                return true;

            var slotOb = moddedSlots[slot];
            if (slotOb != default)
            {
                parent = appearanceApplicator._boneLookup[slotOb.Item1].transform;
                return false;
            }

            return true;
        }

        public void RegisterSlot(Fashion.Slot slot, SlimeAppearance.SlimeBone bone, string bonePath)
        {
            moddedSlots[slot] = (bone, bonePath);
            OnRegisterFashionSlot(slot, bone, bonePath);
        }

        public override void Initialize()
        {
        }
    }
}

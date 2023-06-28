using HarmonyLib;
using SRML.API.Identifiable.Slime;
using System.Linq;
using UnityEngine;

namespace SRML.API.Identifiable.Patches
{
    [HarmonyPatch(typeof(AttachFashions), "GetParentForSlot")]
    internal static class AttachFashionsParentPatch
    {
        public static bool Prefix(AttachFashions __instance, Fashion.Slot slot, ref Transform __result)
        {
            if (global::Identifiable.IsSlime(global::Identifiable.GetId(__instance.gameObject))) 
                return true;

            string path = FashionSlotRegistry.Instance.Registered.FirstOrDefault(x => x.Item1 == slot).Item3;
            if (path != null)
            {
                __result = __instance.transform.Find(path);
                return false;
            }

            return true;
        }
    }
}

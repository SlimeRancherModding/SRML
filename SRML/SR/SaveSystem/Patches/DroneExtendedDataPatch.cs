using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using SRML.SR.SaveSystem.Data.Ammo;
using SRML.Console;
using System;
using UnityEngine;
using System.Linq;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(DroneNetwork.StorageMetadata), "Increment")]
    internal static class DroneExtendedDataPatch
    {
        public static void Prefix(DroneNetwork.StorageMetadata __instance)
        {
            DronePersistentAmmoManager.RegisterStorage(__instance);
            DronePersistentAmmoManager.currMetadata = __instance;
        }
    }

    [HarmonyPatch]
    internal static class DroneDynamicPickupSaveExtendedDataPatch
    {
        private static readonly Type targetType = AccessTools.Inner(typeof(DroneProgramSourceDynamic), "<SphereCastPickupCoroutine>d__21");

        public static MethodBase TargetMethod() => AccessTools.Method(targetType, "MoveNext");

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt && v.operand is MethodInfo info && info.Name == "MaybeAddToSlot")
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(targetType, "identifiable"));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(DroneDynamicPickupSaveExtendedDataPatch), "Replacement"));
                }
                else yield return v;
            }
        }

        public static bool Replacement(DroneAmmo ammo, Identifiable.Id id, Identifiable ident)
        {
            return ammo.MaybeAddToSpecificSlot(id, ident, 0);
        }
    }
}

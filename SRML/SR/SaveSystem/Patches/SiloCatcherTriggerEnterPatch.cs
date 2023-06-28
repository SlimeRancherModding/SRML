using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SiloCatcher))]
    [HarmonyPatch("OnTriggerEnter")]
    internal static class SiloCatcherTriggerEnterPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach(var v in instr)
            {
                if(v.opcode == OpCodes.Call&&(v.operand as MethodInfo)?.Name == "Insert")
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SiloCatcherTriggerEnterPatch), "Alternate"));
                }
                else
                {
                    yield return v;
                }
            }
        }

        public static bool Alternate(SiloCatcher catcher, Identifiable.Id id, Collider collider)
        {
            switch (catcher.type)
            {
                case SiloCatcher.Type.SILO_DEFAULT:
                    bool result = catcher.storageSilo.GetRelevantAmmo().MaybeAddToSpecificSlot(id, collider.GetComponent<Identifiable>(), catcher.slotIdx, 1, false);
                    catcher.storageSilo.OnAdded();
                    return result;
                case SiloCatcher.Type.REFINERY:
                    return SRSingleton<SceneContext>.Instance.GadgetDirector.AddToRefinery(id);
                case SiloCatcher.Type.DECORIZER:
                    return catcher.storageDecorizer.Add(id);
                case SiloCatcher.Type.VIKTOR_STORAGE:
                    return catcher.storageGlitch.Add(id);
            }
            throw new ArgumentException(catcher.type.ToString());
        }
    }

    //[HarmonyPatch(typeof(DroneProgramDestinationSiloStorage<>))]
    //[HarmonyPatch("OnAction_Deposit")]
    internal static class DroneIncrementSiloStoragePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach(var v in instr)
            {
                if(v.opcode == OpCodes.Call&&(v.operand as MethodInfo)?.Name == "Increment")
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DroneIncrementSiloStoragePatch), "Alternate"));
                }
                else
                {
                    yield return v;
                }
            }
        }

        public static bool Alternate(DroneNetwork.StorageMetadata metadata, Identifiable.Id id, bool overflow, int count)
        {
            SiloCatcher catcher = metadata.catcher;
            switch (catcher.type)
            {
                case SiloCatcher.Type.SILO_DEFAULT:
                    bool result = catcher.storageSilo.MaybeAddIdentifiable(id, catcher.slotIdx, count, overflow);
                    catcher.storageSilo.OnAdded();
                    return result;
                case SiloCatcher.Type.REFINERY:
                    return SRSingleton<SceneContext>.Instance.GadgetDirector.AddToRefinery(id);
                case SiloCatcher.Type.DECORIZER:
                    return catcher.storageDecorizer.Add(id);
                case SiloCatcher.Type.VIKTOR_STORAGE:
                    return catcher.storageGlitch.Add(id);
            }
            throw new ArgumentException(catcher.type.ToString());
        }
    }
}

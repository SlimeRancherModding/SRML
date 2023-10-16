using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(PlortCollector))]
    [HarmonyPatch("Update")]
    internal static class PlortCollectorUpdatePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach(var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt && (v.operand as MethodInfo).Name == "MaybeAddIdentifiable")
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlortCollectorUpdatePatch), "Placeholder"));
                }
                else yield return v;
            }
        }

        public static bool Placeholder(SiloStorage storage, Identifiable.Id id, PlortCollector.JointReference joint) {
            return storage.MaybeAddIdentifiable(joint.vacuumable.identifiable);
        }
    }
}

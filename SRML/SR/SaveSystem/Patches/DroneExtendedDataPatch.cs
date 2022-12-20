using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace SRML.SR.SaveSystem.Patches
{
    //[HarmonyPatch(typeof(DroneProgramSourceSiloStorage), "OnAction")]
    internal static class DroneStorageSourcePatch
    {
        /*public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt && (v.operand as MethodInfo)?.Name == "MaybeAddToSlot")
                {
                    yield return new CodeInstruction()
                }
                else
                {
                    yield return v;
                }
            }
        }*/
    }
}

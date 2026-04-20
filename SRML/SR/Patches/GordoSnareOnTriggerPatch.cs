using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GordoSnare), "OnTriggerEnter")]
    [HarmonyPriority(Priority.First)]
    internal static class GordoSnareOnTriggerPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> instructions = new List<CodeInstruction>(instr);
            int index = -1;
            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].opcode == OpCodes.Ldloc_0 && instructions[i - 1].opcode == OpCodes.Brfalse_S)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1) instructions[index + 2].operand = AccessTools.Method(typeof(GordoSnareOnTriggerPatch), "IsSnareable");
            return instructions;
        }

        public static bool IsSnareable(Identifiable.Id identifiable) => Identifiable.IsFood(identifiable) // Vanilla behaviour
            || SnareRegistry.snareables.Contains(identifiable) // Simple non food baits
            || SnareRegistry.baitFuncs.Any(x => x(identifiable)); // More complex baits/batch id handling
    }
}

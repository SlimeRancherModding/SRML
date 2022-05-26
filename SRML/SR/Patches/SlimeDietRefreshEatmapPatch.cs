using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SlimeDiet), "RefreshEatMap")]
    internal static class SlimeDietRefreshEatmapPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> instructions = new List<CodeInstruction>(instr);
            int insertIndex = -1;
            object jumpTo = null;
            object localVar = null;
            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].opcode == OpCodes.Stloc_S && instructions[i - 1].opcode == OpCodes.Ldc_I4_S) localVar = instructions[i].operand;
                if (instructions[i].opcode == OpCodes.Brtrue_S && instructions[i - 1].opcode == OpCodes.Call)
                {
                    insertIndex = i + 4;
                    jumpTo = instructions[i].operand;
                    break;
                }
            }

            if (insertIndex != -1)
            {
                instructions.InsertRange(insertIndex, new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SlimeDefinition), "IdentifiableId")),
                    new CodeInstruction(OpCodes.Ldloc_S, localVar),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SlimeDietRefreshEatmapPatch), "Contains")),
                    new CodeInstruction(OpCodes.Brtrue_S, jumpTo)
                });
            }
            return instructions;
        }

        public static bool Contains(Identifiable.Id slime, Identifiable.Id transformsInto) => SlimeRegistry.preventLargoTransforms.Contains(new KeyValuePair<Identifiable.Id, Identifiable.Id>(slime, transformsInto));
    }
}

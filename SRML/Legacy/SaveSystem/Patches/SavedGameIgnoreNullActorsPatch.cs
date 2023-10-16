using HarmonyLib;
using MonomiPark.SlimeRancher;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SavedGame), "PushActorData")]
    internal static class SavedGameIgnoreNullActorsPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> instr = new List<CodeInstruction>(instructions);
            int index1 = instr.IndexOf(instr.FirstOrDefault(x => x.opcode == OpCodes.Stloc_0));
            int index2 = instr.IndexOf(instr.LastOrDefault(x => x.opcode == OpCodes.Ret));
            Label label = il.DefineLabel();

            if (index1 != -1 && index2 != -1)
            {
                instr[index2].labels.Add(label);
                instr.InsertRange(index1 + 1, new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Beq_S, label)
                });
            }

            return instr;
        }
    }
}

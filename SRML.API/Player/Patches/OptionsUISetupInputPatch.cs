using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(OptionsUI))]
    [HarmonyPatch("SetupInput")]
    internal static class OptionsUISetupInputPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> allInstructions = new List<CodeInstruction>(instr);
            CodeInstruction latestBinding = instr.Last(x => x.opcode == OpCodes.Call && ((MethodInfo)x.operand).Name == "CreateKeyBindingLine");
            int index = instr.IndexOfItem(latestBinding) + 1;

            List<CodeInstruction> newInstructions = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(OptionsUISetupInputPatch), "Alternate"))
            };
            allInstructions.InsertRange(index, newInstructions);
            return allInstructions;
        }

        public static void Alternate(OptionsUI ui)
        {
            foreach (var v in SRInput.Actions.Actions.Where(x => PlayerActionRegistry.Instance.registeredBindings.Contains(x)))
                ui.CreateKeyBindingLine("key." + v.Name.ToLowerInvariant(), v);
        }
    }
}

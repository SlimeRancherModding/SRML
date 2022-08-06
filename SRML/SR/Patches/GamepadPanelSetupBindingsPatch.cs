using HarmonyLib;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GamepadPanel))]
    [HarmonyPatch("SetupBindings")]
    internal static class GamepadPanelSetupBindingsPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> allInstructions = new List<CodeInstruction>(instr);
            CodeInstruction latestBinding = instr.Last(x => x.opcode == OpCodes.Call && ((MethodInfo)x.operand).Name == "CreateGamepadBindingLine");
            int index = instr.IndexOfItem(latestBinding) + 1;

            List<CodeInstruction> newInstructions = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GamepadPanelSetupBindingsPatch), "Alternate"))
            };
            allInstructions.InsertRange(index, newInstructions);
            return allInstructions;
        }

        public static void Alternate(GamepadPanel ui)
        {
            foreach (var v in SRInput.Actions.Actions.Where(x => BindingRegistry.moddedBindings.Contains(x)))
                ui.CreateGamepadBindingLine("key." + v.Name.ToLowerInvariant(), v);
        }
    }
}

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
    [HarmonyPatch(typeof(OptionsUI))]
    [HarmonyPatch("SetupInput")]
    internal static class OptionsUISetupInputPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            CodeInstruction latestBinding=null;
            List<CodeInstruction> allInstructions = new List<CodeInstruction>();
            foreach(var v in instr)
            {
                allInstructions.Add(v);
                if (v.opcode == OpCodes.Call && (v.operand as MethodInfo).Name == "CreateKeyBindingLine") latestBinding = v;
            }

            using(var codes = allInstructions.GetEnumerator())
            {
                while (codes.MoveNext())
                {
                    var cur = codes.Current;
                    if (cur == latestBinding)
                    {
                        yield return cur;
                        codes.MoveNext();
                        yield return codes.Current;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(OptionsUISetupInputPatch), "Alternate"));
                    }
                    else yield return cur;
                    

                }
            }
        }
        public static void Alternate(OptionsUI ui)
        {
            foreach(var v in SRInput.Actions.Actions.Where(x=>BindingRegistry.IsModdedAction(x)&&!BindingRegistry.ephemeralActions.Contains(x)))
            {
                ui.CreateKeyBindingLine("key." + v.Name.ToLowerInvariant(), v);
                
            }
        }
    }
}

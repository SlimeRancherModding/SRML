using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Harmony;
using SRML.Utils;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(AutoSaveDirector))]
    internal static class LoadSavePatch
    {
        public static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(AutoSaveDirector), "LoadSave");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt&&v.operand is MethodInfo info&&info.Name == "Load")
                {
                    yield return v;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(LoadSavePatch), "LoadModSave"));
                }   
                else
                {
                    yield return v;
                }
            }
        }

        public static void LoadModSave(AutoSaveDirector director, string gameName, string saveName)
        {
            SaveHandler.LoadModdedSave(director,saveName);
        }
    }
}

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
        private static Type targetType = typeof(AutoSaveDirector).GetNestedTypes(BindingFlags.NonPublic)
            .First((x) => x.Name == "<LoadSave_Coroutine>c__Iterator1");

        public static MethodInfo TargetMethod()
        {
            return targetType.GetMethod("MoveNext");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt&&v.operand is MethodInfo info&&info.Name == "Load")
                {
                    yield return v;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    yield return new CodeInstruction(OpCodes.Ldfld, targetType.GetField("$this", flags));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, targetType.GetField("gameName", flags));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, targetType.GetField("saveName", flags));
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

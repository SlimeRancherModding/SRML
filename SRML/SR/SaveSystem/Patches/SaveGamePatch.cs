using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using SRML.Utils;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(AutoSaveDirector))]
    [HarmonyPatch("SaveGame")]
    internal class SaveGamePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            using (var code = instr.GetEnumerator())
            {
                bool passedone = false;
                while (code.MoveNext())
                {
                    var cur = code.Current;

                    if (cur.opcode == OpCodes.Callvirt && cur.operand is MethodInfo info && info.Name == "GetName")
                    {
                        if (!passedone)
                        {
                            passedone = true;
                            yield return cur;
                            continue;
                        }
                        yield return cur;
                        code.MoveNext();
                        yield return code.Current;

                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldloc_0);

                        yield return new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(SaveGamePatch), "OnSaveGame"));

                    }
                    else
                    {
                        yield return cur;
                    }
                }
            }
        }

        public static void OnSaveGame(AutoSaveDirector director, string nextfilename)
        {
            SaveHandler.SaveModdedSave(director, director.GetNextFileName(nextfilename,0, "{0}_{1}"));
        }


    }
}

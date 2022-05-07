using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(CorporatePartnerUI))]
    [HarmonyPatch("BuyLevel")]
    internal static class CorporatePartnerBuyPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            using (IEnumerator<CodeInstruction> iter = instructions.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    CodeInstruction current = iter.Current;
                    if (current.opcode == OpCodes.Ldarg_2)
                    {
                        if (iter.MoveNext())
                        {
                            CodeInstruction next = iter.Current;
                            if (next.opcode == OpCodes.Ldc_I4_5)
                            {
                                yield return new CodeInstruction(OpCodes.Ldarg_2);
                                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SRCallbacks), nameof(SRCallbacks.OnLevelBoughtCallback)));
                            }
                            yield return current;
                            yield return next;
                        }
                        else
                            yield return current;
                    }
                    else
                        yield return current;
                }
            }
        }
    }
}

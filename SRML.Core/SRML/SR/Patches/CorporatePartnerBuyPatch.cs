using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(CorporatePartnerUI), "BuyLevel")]
    internal static class CorporatePartnerBuyPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);
            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldarg_2 && code[i + 1].opcode == OpCodes.Ldc_I4_5)
                {
                    code.InsertRange(i, new List<CodeInstruction>()
                    {
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SRCallbacks), "OnCorporateLevelBoughtCallback"))
                    });
                    break;
                }
            }
            return code;
        }
    }
}

using HarmonyLib;
using SRML.SR.UI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(CorporatePartnerUI), "RebuildUI")]
    internal static class CorporatePartnerRebuildDynamicPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> instructions = new List<CodeInstruction>(instr);

            int index1 = -1;
            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].opcode == OpCodes.Ldc_I4_0 && instructions[i + 1].opcode == OpCodes.Stloc_S)
                {
                    index1 = i;
                    break;
                }
            }
            int index2 = instructions.IndexOf(instructions.First(x => x.opcode == OpCodes.Blt_S));

            if (index1 != -1 && index2 != -2)
            {
                instructions.RemoveRange(index1, index2 - index1 + 1);
                instructions.InsertRange(index1, new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(CorporatePartnerRebuildDynamicPatch), "RefreshRewardsFancily"))
                });
            }
            
            return instructions;
        }

        public static void RefreshRewardsFancily(CorporatePartnerUI instance, int rank)
        {
            for (int i = 0; i < EnhancedCorporatePartnerHandler.content.childCount; i++)
                EnhancedCorporatePartnerHandler.content.GetChild(i).gameObject.Destroy();
            for (int j = 0; j < instance.ranks[rank - 1].rewardIcons.Length; j++)
                EnhancedCorporatePartnerHandler.CreateRewardEntry(instance, rank, j);
            if (!CorporatePartnerRegistry.rewardsForLevel.ContainsKey(rank)) return;
            foreach (CorporatePartnerRegistry.RewardEntry entry in CorporatePartnerRegistry.rewardsForLevel[rank])
                EnhancedCorporatePartnerHandler.CreateRewardEntry(instance, entry);
        }
    }
}

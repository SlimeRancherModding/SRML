using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SRML.SR.SaveSystem.Data;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SlimeEat), "EatAndTransform")]
    internal static class EatAndTransformCopyPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldloc_1 && code[i - 1].opcode == OpCodes.Pop)
                {
                    insertionIndex = i;
                    break;
                }
            }

            MethodInfo getGet = AccessTools.Property(typeof(Component), nameof(Component.gameObject)).GetGetMethod();
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();
            newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
            newInstructions.Add(new CodeInstruction(OpCodes.Call, getGet));
            newInstructions.Add(new CodeInstruction(OpCodes.Ldloc_1));
            newInstructions.Add(new CodeInstruction(OpCodes.Call, typeof(EatAndTransformCopyPatch).GetMethod("CopyComponentsToTransformed", BindingFlags.Public | BindingFlags.Static)));
            if (insertionIndex != -1) code.InsertRange(insertionIndex, newInstructions);
            return code;
        }

        public static void CopyComponentsToTransformed(GameObject original, GameObject transformed)
        {
            if (original.GetComponent<ExtendedData.TransformableParticipant>() == null) return;
            foreach (ExtendedData.TransformableParticipant part in original.GetComponents<ExtendedData.TransformableParticipant>())
            {
                if (part.CopyCondition())
                {
                    if (!transformed.HasComponent(part.GetType())) transformed.AddComponent(part.GetType());
                    var data = new CompoundDataPiece("");
                    part.WriteData(data);
                    ((ExtendedData.TransformableParticipant)transformed.GetComponent(part.GetType())).ReadData(data);
                }
            }
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static SRML.SR.PediaRegistry;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PediaUI))]
    [HarmonyPatch("SelectEntry")]
    internal static class PediaUISelectEntryPatch
    {
        public static GameObject GetPanel(PediaUI ui)
        {
            return ui.genericDescPanel.parent.parent.parent.gameObject;
        }
        public static bool CheckIfHasCustomRenderer(PediaUI ui, PediaDirector.Id id)
        {
            var panel = GetPanel(ui);
            if (PediaRegistry.activeRenderer != null)
            {
                if (PediaRegistry.activeRenderer is IReusablePediaRenderer r) r.CurrentID = id;
                PediaRegistry.activeRenderer.OnListingDeselected(panel);
                PediaRegistry.activeRenderer = null;
            }

            var renderer = PediaRegistry.GetRenderer(id);
            if (renderer == null) return false;
            if (renderer is IReusablePediaRenderer R) R.CurrentID = id;
            renderer.OnListingSelected(panel);
            PediaRegistry.activeRenderer = renderer;
            return true;
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            var list = instr.ToList();
            Label label = default;
            foreach(var v in list)
            {
                if(v.opcode == OpCodes.Br)
                {
                    label = (Label)v.operand;
                }
            }
            using (var codes = list.GetEnumerator())
            {
                
                while (codes.MoveNext())
                {
                    var current = codes.Current;
                    if (current.opcode == OpCodes.Ldsfld && (current.operand as FieldInfo).Name == "TUTORIALS_ENTRIES")
                    { 
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PediaUISelectEntryPatch), "CheckIfHasCustomRenderer"));
                        yield return new CodeInstruction(OpCodes.Brtrue, label);
                        yield return current;
                    }
                    else yield return current;
                }
            }
        }

        public static void Prefix(PediaDirector.Id id)
        {
            Console.Console.Log($"selecting entry {id}");
        }
    }
}

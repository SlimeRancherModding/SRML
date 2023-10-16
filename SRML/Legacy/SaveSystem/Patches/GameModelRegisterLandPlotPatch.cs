using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(GameModel))]
    [HarmonyPatch("RegisterLandPlot")]
    public static class GameModelRegisterLandPlotPatch
    {
        public static  IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach(var v in instr)
            {
                if (v.opcode == OpCodes.Newobj)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameModelRegisterLandPlotPatch), "Replacement"));
                }
                else yield return v;
            }
        }
        public static LandPlotModel Replacement(GameObject obj)
        {
            var landplot = obj.GetComponentInChildren<LandPlot>(true);
            if (landplot)
            {
                var potentialNew = DataModelRegistry.landPlotOverrides.FirstOrDefault(x => x.Key(landplot.typeId));
                if (potentialNew.Value != null)
                {
                    return potentialNew.Value();
                }
            }
            return new LandPlotModel();
        }
    }
}

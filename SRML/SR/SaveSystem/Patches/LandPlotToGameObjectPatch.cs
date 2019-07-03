using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SavedGame))]
    [HarmonyPatch("LandPlotToGameObject")]
    internal static class LandPlotToGameObjectPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            bool flag = false;
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt && v.operand is MethodInfo info && info.Name == "Remove")
                { 

                    flag = true;
                    yield return v;
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloca_S,0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(LandPlotToGameObjectPatch), "FixLandPlotModel"));
                }
                else
                {
                    if (flag)
                    {
                        flag = false;
                        continue;
                    }
                    yield return v;
                }
            }
        }

        public static void FixLandPlotModel(LandPlotV08 data, ref LandPlotModel model)
        {
            return;
            var temp = model;
            model = new TestModel();
            model.SetGameObject(temp.gameObj);
            if (!SceneContext.Instance.GameModel.expectingPush)
            {
                model.Init();
                model.NotifyParticipants();
            }
            SceneContext.Instance.GameModel.landPlots[data.id] = model;
        }

        public class TestModel : LandPlotModel
        {

        }
    }
}

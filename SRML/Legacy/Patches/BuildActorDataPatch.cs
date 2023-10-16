using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SavedGame))]
    [HarmonyPatch("BuildActorData")]
    internal static class BuildActorDataPatch
    {

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        { 
            foreach(var v in instr)
            {
                if (v.opcode == OpCodes.Ret)
                { 
                    yield return new CodeInstruction(OpCodes.Ldarg,4);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildActorDataPatch), "Alternate"));
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return v;
                }
                else yield return v;
            }
        }
        public static void Alternate(ActorDataV09 data,ActorModel model)
        {
            foreach (var v in DataModelRegistry.actorSavers.Where(x => x.Key(model)))
            {
                v.Value(model, data);
            }
        }
    }
}

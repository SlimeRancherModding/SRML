using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Harmony;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SavedGame))]
    internal static class BuildActorDataPatch
    {
        public static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(SavedGame), "BuildActorDataV07");
        }

        public static VanillaActorData ActorCreator(GameModel gameModel, int typeId, long actorId, ActorModel actorModel)
        {
            //Debug.Log($"We got a hit! {typeId} {actorId} {actorModel.GetType().Name}");
            var mod = SaveRegistry.ModForModelType(actorModel.GetType());
            if (mod != null)
            {  
                var info = SaveRegistry.GetSaveInfo(mod).CustomActorDataRegistry;
                var newmodel = info.GetDataForID(info.GetIDForModel(actorModel.GetType()));
                newmodel.PullCustomModel(actorModel);
                return (VanillaActorData)newmodel;
            }
            return new VanillaActorData();
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            using (var code = instr.GetEnumerator())
            {

                code.MoveNext();
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldarg_3);
                yield return new CodeInstruction(OpCodes.Ldarg_S,(byte)4);
                yield return new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(BuildActorDataPatch),"ActorCreator"));
                while (code.MoveNext())
                {
                    yield return code.Current;
                }
            }

        }

       
    }
}

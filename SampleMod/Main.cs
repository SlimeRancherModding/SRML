using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Text;
using SRML;
using Harmony;
using SRML.Utils;

namespace SampleMod
{
    public class Main : ModEntryPoint
    {
        public override void PreLoad(HarmonyInstance instance)
        {
            Debug.Log("We did it! "+TestDependency.Main.depended_on_value);
            instance.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log(SRModInfo.GetMyInfo().Author);
        }

        public override void PostLoad()
        {
            Debug.Log("We did it! Again! ");

        }
    }

    [HarmonyPatch(typeof(SceneContext))]
    [HarmonyPatch("Start")]
    public static class StartGamePatch
    {
        public static void Postfix()
        {
            if (Levels.isMainMenu()) return;
            var playerModel = SceneContext.Instance.GameModel.GetPlayerModel();
            SRBehaviour.InstantiateActor(
                GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.MOSAIC_BOOM_LARGO), playerModel.position,
                playerModel.rotation);

        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Text;
using SRML;
using Harmony;
namespace SampleMod
{
    public class Main : ModEntryPoint
    {
        public override void PreLoad(HarmonyInstance instance)
        {
            Debug.Log("We did it! "+TestDependency.Main.depended_on_value);
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void PostLoad()
        {
            Debug.Log("We did it! Again! "+FileSystem.GetMyPath());
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
            //SRBehaviour.InstantiateActor(
            //    GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.MOSAIC_BOOM_LARGO), playerModel.position,
            //    playerModel.rotation);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<GameContext>.Instance.LookupDirector.GetGordo(Identifiable.Id.BOOM_GORDO));
            Debug.Log(SRML.Utils.GameObjectUtils.PrintObjectTree((gameObject)));

        }
    }
}

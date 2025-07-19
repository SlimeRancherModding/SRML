using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SceneContext))]
    [HarmonyPatch("Start")]
    internal static class SceneContextStartPatch
    {
        public static void Postfix(SceneContext __instance)
        {
            SRCallbacks.OnSceneLoaded(__instance);
        }

        public static void Prefix(SceneContext __instance)
        {
            if (Levels.isMainMenu())
                return;

            try
            {
                SRCallbacks.PreSceneLoad(__instance);
            }
            catch (Exception e)
            {
                Console.Console.Instance.Log($"Error pre-save load! {e}");

                AutoSaveDirector autoSaveDirector = GameContext.Instance.AutoSaveDirector;
                LoadErrorUI.OpenLoadErrorUI(autoSaveDirector.loadFileErrorPrefab, "e.srml_load", false, "e.ok_button", () => autoSaveDirector.loadingUI.OnLoadingError());
            }
        }
    }
}
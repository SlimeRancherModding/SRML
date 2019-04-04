using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using SRML.Editor;
using SRML.Utils;
using TMPro;
using UnityEngine;

namespace SRML
{
    internal static class Main
    {
        private static bool isPreInitialized;
        public static void PreLoad()
        {
            if (isPreInitialized) return;
            isPreInitialized = true;
            Debug.Log("SRML has successfully invaded the game!");
            HarmonyPatcher.PatchAll();
            Debug.Log(EnumPatcher.GetFirstFreeValue(typeof(Identifiable.Id)));
            

            try
            {
                SRModLoader.LoadMods();
                
            }
            catch (Exception e)
            {
                ErrorGUI.CreateError($"{e.GetType().Name}: {e.Message}");
                return;
            }

            try
            {
                SRModLoader.PreLoadMods();
            }
            catch (Exception e)
            {
                ErrorGUI.CreateError($"{e.Message}");
            }
            ReplacerCache.ClearCache();

            HarmonyPatcher.Instance.Patch(typeof(GameContext).GetMethod("Start"),
                new HarmonyMethod(typeof(Main).GetMethod("PostLoad", BindingFlags.NonPublic | BindingFlags.Static)));

        }
        
        private static bool isPostInitialized;

        static void PostLoad()
        {
            if (isPostInitialized) return;
            isPostInitialized = true;
            PrefabUtils.ProcessReplacements();
            try
            {
                SRModLoader.PostLoadMods();
            }
            catch (Exception e)
            {
                ErrorGUI.CreateError($"{e.GetType().Name}: {e.Message}");
                return;
            }
        }

    }
}

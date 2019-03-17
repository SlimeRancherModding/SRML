using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using SRML.Utils;
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
            HarmonyPatcher.Instance.Patch(typeof(GameContext).GetMethod("Start"),
                new HarmonyMethod(typeof(Main).GetMethod("PostLoad",BindingFlags.NonPublic|BindingFlags.Static)));
            try
            {
                SRModLoader.LoadMods();
                SRModLoader.PreLoadMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


        }
        
        private static bool isPostInitialized;

        static void PostLoad()
        {
            if (isPostInitialized) return;
            isPostInitialized = true;
            SRModLoader.PostLoadMods();

        }

    }
}

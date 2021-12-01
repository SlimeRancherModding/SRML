﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRML.Config;
using SRML.Console;
using SRML.Editor;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using SRML.Utils;
using SRML.Utils.Prefab.Patches;
using UnityEngine;

namespace SRML
{
    internal static class Main
    {

        private static bool isPreInitialized;

        /// <summary>
        /// Called before GameContext.Awake()
        /// </summary>
        internal static void PreLoad() 
        {
            if (isPreInitialized) return;
            isPreInitialized = true;
            Debug.Log("SRML has successfully invaded the game!");

            foreach(var v in Assembly.GetExecutingAssembly().GetTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(v.TypeHandle);
            }
            HarmonyPatcher.PatchAll();

            try
            {
                
                SRModLoader.InitializeMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ErrorGUI.CreateError($"{e.GetType().Name}: {e.Message}");
                return;
            }
            FileLogger.Init();
            Console.Console.Init();
            HarmonyOverrideHandler.PatchAll();
            try
            {
                SRModLoader.PreLoadMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ErrorGUI.CreateError($"{e.GetType().Name}: {e.Message}");
                return;
            }
            ReplacerCache.ClearCache();



            HarmonyPatcher.Instance.Patch(typeof(GameContext).GetMethod("Start"),
                prefix: new HarmonyMethod(typeof(Main).GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static)));

        }

        private static bool isInitialized;

        /// <summary>
        /// Called before GameContext.Start()
        /// </summary>
        static void Load()
        {
            if (isInitialized) return;
            isInitialized = true;

            BaseObjects.Populate();
            SRCallbacks.OnLoad();
            PrefabUtils.ProcessReplacements();
            KeyBindManager.ReadBinds();
            GameContext.Instance.gameObject.AddComponent<ModManager>();
            GameContext.Instance.gameObject.AddComponent<KeyBindManager.ProcessAllBindings>();
            try
            {
                SRModLoader.LoadMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ErrorGUI.CreateError($"{e.GetType().Name}: {e.Message}");
                return;
            }
            PostLoad();
        }
        
        private static bool isPostInitialized;

        /// <summary>
        /// Called after Load
        /// </summary>
        static void PostLoad()
        {
            if (isPostInitialized) return;
            isPostInitialized = true;   
            try
            {
                SRModLoader.PostLoadMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ErrorGUI.CreateError($"{e.GetType().Name}: {e.Message}");
                return;
            }

        }

        internal static void ReLoad()
        {
            try
            {
                SRModLoader.ReLoadMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal static void UnLoad()
        {
            try
            {
                SRModLoader.UnLoadMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal static void Update()
        {
            try
            {
                SRModLoader.UpdateMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal static void FixedUpdate()
        {
            try
            {
                SRModLoader.UpdateModsFixed();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal static void LateUpdate()
        {
            try
            {
                SRModLoader.UpdateModsLate();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    internal class ModManager : MonoBehaviour
    {
        void Update() => Main.Update();

        void FixedUpdate() => Main.FixedUpdate();

        void LateUpdate() => Main.LateUpdate();

        void OnApplicationQuit() => Main.UnLoad();
    }
}

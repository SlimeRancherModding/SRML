using System;
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
using SRML.SR.UI;
using SRML.SR.Utils;
using SRML.SR.Utils.BaseObjects;
using SRML.Utils;
using TMPro;
using UnityEngine;

namespace SRML
{
    internal static class Main
    {
        public const string VERSION_STRING = "BETA-0.2.2-2023020501";

        private static bool isPreInitialized;
        internal static Transform prefabParent;
        internal static FileStorageProvider StorageProvider = new FileStorageProvider();
        internal static ConfigFile config;
        internal static AssetBundle uiBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(ModMenuUIHandler), "srml"));

        /// <summary>
        /// Called before GameContext.Awake()
        /// </summary>
        internal static void PreLoad() 
        {
            if (isPreInitialized) return;
            isPreInitialized = true;
            Debug.Log("SRML has successfully invaded the game!");

            SystemContext.IsModded = true;
            SentrySdk sentrySdk = UnityEngine.Object.FindObjectOfType<SentrySdk>();
            if (sentrySdk != null)
            {
                sentrySdk.Dsn = string.Empty;
                FieldInfo field = sentrySdk.GetType().GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
                if (field != null) field.SetValue(null, null);
                sentrySdk.StopAllCoroutines();
                Application.logMessageReceived -= sentrySdk.OnLogMessageReceived;
                UnityEngine.Object.Destroy(sentrySdk, 1f);
                Debug.Log("Disabling Sentry SDK");
            }

            StorageProvider.Initialize();
            prefabParent = new GameObject("PrefabParent").transform;
            prefabParent.gameObject.SetActive(false);
            GameObject.DontDestroyOnLoad(prefabParent.gameObject);
            foreach (var v in Assembly.GetExecutingAssembly().GetTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(v.TypeHandle);
            }
            HarmonyPatcher.PatchAll();

            Type sm = typeof(GameContext).Assembly.GetType("SteamManager", false, true);
            if (sm != null)
            {
                HarmonyPatcher.Instance.Patch(sm.GetMethod("AddAchievement"),
                    prefix: new HarmonyMethod(typeof(AchievementRegistry).GetMethod("ModdedAchievementPatch", BindingFlags.NonPublic | BindingFlags.Static)));
            }
            config = ConfigFile.GenerateConfig(typeof(SRMLConfig));
            config.TryLoadFromFile();

            ErrorGUI.extendedUI = uiBundle.LoadAsset<GameObject>("SRMLErrorUI");
            ErrorGUI.extendedError = uiBundle.LoadAsset<GameObject>("ErrorModInfo");

            foreach (TMP_Text text in ErrorGUI.extendedUI.GetComponentsInChildren<TMP_Text>())
                text.alignment = TextAlignmentOptions.Midline;
            foreach (TMP_Text text in ErrorGUI.extendedError.GetComponentsInChildren<TMP_Text>(true))
                text.alignment = TextAlignmentOptions.TopLeft;

            try
            {
                SRModLoader.InitializeMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ErrorGUI.errors.Add(new ModLoadException("<unknown>", SRModLoader.LoadingStep.INITIALIZATION, e));
                return;
            }
            FileLogger.Init();
            Console.Console.Init();
            Console.Console.Instance.Log($"SRML v {VERSION_STRING}");
            HarmonyOverrideHandler.PatchAll();
            SRModLoader.PreLoadMods();
            IdentifiableRegistry.CategorizeAllIds();
            GadgetRegistry.CategorizeAllIds();
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
            KeyBindManager.ReadBinds();
            SlimeRegistry.Initialize(GameContext.Instance.SlimeDefinitions);
            GameContext.Instance.gameObject.AddComponent<ModManager>();
            GameContext.Instance.gameObject.AddComponent<KeyBindManager.ProcessAllBindings>();
            
            SRModLoader.LoadMods();
            GameContext.Instance.SlimeDefinitions.RefreshEatmaps();

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
            SRModLoader.PostLoadMods();
        }

        internal static void Reload()
        {
            try
            {
                SRModLoader.ReloadMods();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal static void Unload()
        {
            try
            {
                SRModLoader.UnloadMods();
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

        void OnApplicationQuit() => Main.Unload();
    }
}

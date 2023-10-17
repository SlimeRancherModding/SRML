using HarmonyLib;
using SRML.Core.ModLoader;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.Mod;
using SRML.Core.ModLoader.BuiltIn.ModLoader;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Reflection;
using SRML.Core.API;
using Sentry;
using SRML.Core.ModLoader.Attributes;
using SRML.Utils.Enum;

namespace SRML.Core
{
    internal static class Main
    {
        public static Harmony HarmonyInstance;
        public static AssetBundle uiBundle;
        public static Transform prefabParent;

        internal static FileStorageProvider StorageProvider = new FileStorageProvider();

        public const string VERSION_STRING = "BETA-0.3.0";
        public const string MODS_PATH = @"SRML\NewMods";

        public static bool initialized = false;

        public static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;

            Debug.Log("SRML has successfully invaded the game!");
            HarmonyInstance = new Harmony("net.veesus.srml");
            HarmonyInstance.PatchAll();

            FileLogger.Init();
            Console.Console.Init();
            Console.Console.Instance.Log($"SRML v {VERSION_STRING}");

            prefabParent = new GameObject("PrefabParent").transform;
            prefabParent.gameObject.DontDestroyOnLoad();
            prefabParent.gameObject.SetActive(false);
            prefabParent.gameObject.hideFlags = HideFlags.HideAndDontSave;

            uiBundle = AssetBundle.LoadFromStream(typeof(Main).Assembly.GetManifestResourceStream(typeof(ErrorGUI), "srmlassets"));
            ErrorGUI.extendedUI = GameObject.Instantiate(uiBundle.LoadAsset<GameObject>("SRMLErrorUI"));
            ErrorGUI.extendedError = GameObject.Instantiate(uiBundle.LoadAsset<GameObject>("ErrorModInfo"));
            ErrorGUI.extendedUI.Prefabitize();
            ErrorGUI.extendedError.Prefabitize();

            // TODO: find a way to prevent this issue in assetbundles so I don't have to do this garbage
            foreach (TMP_Text text in ErrorGUI.extendedUI.GetComponentsInChildren<TMP_Text>())
                text.alignment = TextAlignmentOptions.Midline;
            foreach (TMP_Text text in ErrorGUI.extendedError.GetComponentsInChildren<TMP_Text>(true))
                text.alignment = TextAlignmentOptions.TopLeft;

            CoreTranslator translator = new CoreTranslator();
            CoreAPI api = new CoreAPI();

            CoreLoader loader = new CoreLoader();
            loader.PreProcessMods += EnumHolderResolver.RegisterAllEnums;
            
            loader.LoadFromDefaultPath();
            
            loader.RegisterModType(typeof(BasicMod), typeof(BasicLoadEntryPoint));
            loader.RegisterModType(typeof(CoreMod), typeof(CoreModEntryPoint));
#pragma warning disable CS0612
            loader.modTypeForEntryType.Add(typeof(IModEntryPoint), typeof(LegacyMod));
            loader.registeredModTypes.Add(typeof(LegacyMod));
#pragma warning restore CS0612
            loader.RegisterModLoader(typeof(BasicModLoader));
            loader.RegisterModLoader(typeof(CoreModLoader));
            // This DOES work, but then it breaks everything because the API is currently still built upon the old modloader.
            loader.RegisterModLoader(typeof(LegacyModLoader));

            var identical = loader.modStack.GroupBy(x => x.Item2.Id).FirstOrDefault(x => x.Count() > 1);
            if (identical != default)
                throw new Exception($"Attempting to load mod with duplicate id: {identical.First().Item2.Id}");

            loader.LoadModStack();
        }
    }
}

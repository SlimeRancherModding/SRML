﻿using HarmonyLib;
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

namespace SRML.Core
{
    internal static class Main
    {
        public static Harmony HarmonyInstance;
        public static CoreLoader loader;
        public static AssetBundle uiBundle;

        internal static FileStorageProvider StorageProvider = new FileStorageProvider();

        public const string VERSION_STRING = "BETA-0.3.0";

        public static void Initialize()
        {
            Debug.Log("SRML has successfully invaded the game!");
            HarmonyInstance = new Harmony("net.veesus.srml");
            HarmonyInstance.PatchAll();

            FileLogger.Init();
            Console.Console.Init();
            Console.Console.Instance.Log($"SRML v {VERSION_STRING}");

            uiBundle = AssetBundle.LoadFromStream(typeof(Main).Assembly.GetManifestResourceStream(typeof(ErrorGUI), "srml"));
            ErrorGUI.extendedUI = uiBundle.LoadAsset<GameObject>("SRMLErrorUI");
            ErrorGUI.extendedError = uiBundle.LoadAsset<GameObject>("ErrorModInfo");

            // TODO: find a way to prevent this issue in assetbundles so I don't have to do this garbage
            foreach (TMP_Text text in ErrorGUI.extendedUI.GetComponentsInChildren<TMP_Text>())
                text.alignment = TextAlignmentOptions.Midline;
            foreach (TMP_Text text in ErrorGUI.extendedError.GetComponentsInChildren<TMP_Text>(true))
                text.alignment = TextAlignmentOptions.TopLeft;

            loader = new CoreLoader();
            loader.RegisterModType(typeof(BasicMod), typeof(BasicLoadEntryPoint));
            loader.RegisterModLoader(typeof(BasicModLoader));

            var identical = loader.modStack.GroupBy(x => x.Item2.Id).FirstOrDefault(x => x.Count() > 1);
            if (identical != default)
                throw new Exception($"Attempting to load mod with duplicate id: {identical.First().Item2.Id}");

            loader.LoadModStack();
            // This DOES work, but then it breaks everything because the API is currently still built upon the old modloader.
            //loader.RegisterModLoader(typeof(LegacyModLoader));
        }
    }
}

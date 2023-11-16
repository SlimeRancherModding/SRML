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
using SRML.Config;
using SRML.SR;
using UnityEngine.UI;
using SRML.SR.UI.Utils;

namespace SRML.Core
{
    internal static class Main
    {
        public static Harmony HarmonyInstance;
        public static AssetBundle uiBundle;
        public static Transform prefabParent;

        internal static FileStorageProvider StorageProvider = new FileStorageProvider();
        internal static ConfigFile Config = ConfigFile.GenerateConfig(typeof(SRMLConfig));

        public const string VERSION_STRING = "BETA-0.3.0";
        public const string MODS_PATH = @"SRML\NewMods";

        public static bool initialized = false;

        public static void Initialize()
        {
            if (initialized)
                return;

            try
            {
                initialized = true;

                Debug.Log("SRML has successfully invaded the game!");

                SentrySdk sentrySdk = UnityEngine.Object.FindObjectOfType<SentrySdk>();
                if (sentrySdk != null)
                {
                    sentrySdk.Dsn = string.Empty;
                    FieldInfo field = sentrySdk.GetType().GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
                    if (field != null) field.SetValue(null, null);
                    sentrySdk.StopAllCoroutines();
                    Application.logMessageReceived -= sentrySdk.OnLogMessageReceived;
                    UnityEngine.Object.Destroy(sentrySdk, 1f);
                    Debug.Log("Disabled Sentry SDK");
                }

                HarmonyInstance = new Harmony("net.veesus.srml");
                HarmonyInstance.PatchAll();

                CoreTranslator translator = new CoreTranslator();

                FileLogger.Init();
                Console.Console.Init();
                Console.Console.Instance.Log($"SRML v {VERSION_STRING}");

                Config.TryLoadFromFile();

                prefabParent = new GameObject("PrefabParent").transform;
                prefabParent.gameObject.DontDestroyOnLoad();
                prefabParent.gameObject.SetActive(false);
                prefabParent.gameObject.hideFlags = HideFlags.HideAndDontSave;

                uiBundle = AssetBundle.LoadFromStream(typeof(Main).Assembly.GetManifestResourceStream(typeof(ErrorGUI), "srmlassets"));
                ErrorGUI.extendedUI = uiBundle.LoadAsset<GameObject>("SRMLErrorUI").CreatePrefabCopy();
                ErrorGUI.extendedError = uiBundle.LoadAsset<GameObject>("ErrorModInfo").CreatePrefabCopy();

                // TODO: find a way to prevent this issue in assetbundles so I don't have to do this garbage
                TMP_FontAsset ass = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(x => x.name == "OpenSans SDF");
                foreach (TMP_Text text in ErrorGUI.extendedUI.GetComponentsInChildren<TMP_Text>())
                {
                    text.alignment = TextAlignmentOptions.Midline;
                    text.font = ass;
                }
                foreach (TMP_Text text in ErrorGUI.extendedError.GetComponentsInChildren<TMP_Text>(true))
                {
                    text.alignment = TextAlignmentOptions.TopLeft;
                    text.font = ass;
                }

                CoreAPI api = new CoreAPI();

                CoreLoader loader = new CoreLoader();
                loader.PreProcessMods += EnumHolderResolver.RegisterAllEnums;
                loader.ProcessMods += x => ConfigManager.PopulateConfigs(x);

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
                //loader.RegisterModLoader(typeof(LegacyModLoader));

                var identical = loader.modStack.GroupBy(x => x.Item2.ID).FirstOrDefault(x => x.Count() > 1);
                if (identical != default)
                    throw new Exception($"Attempting to load mod with duplicate id: {identical.First().Item2.ID}");

                loader.LoadModStack();
            }
            catch (Exception e)
            {
                ErrorGUI.AddError("SRML", ErrorGUI.ErrorType.LoadSRML, e);
                ErrorGUI.CreateExtendedErrorOnMenu();
            }
        }
    }
}

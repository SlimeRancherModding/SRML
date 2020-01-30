using HarmonyLib;
using MonomiPark.SlimeRancher;
using SRML.SR.SaveSystem.Data.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(SavedProfile))]
    [HarmonyPatch("LoadProfile")]
    internal static class SavedProfileLoadProfilePatch
    {
        static void Postfix(SavedProfile __instance)
        {
            var enumTranslatorPath = Path.Combine(Application.persistentDataPath,"SRML","DATA.bin");
            if (!File.Exists(enumTranslatorPath))
            {
                Debug.LogError("Could not find DATA.bin, aborting achievements loading...");
                return;
            }
            var translator = new EnumTranslator();
            using (var stream = new FileStream(enumTranslatorPath, FileMode.Open))
            {
                translator.Read(new BinaryReader(stream));
            }
            translator.FixMissingEnumValues();
            foreach(var mod in SRModLoader.GetMods())
            {
                var path = Path.Combine(FileSystem.GetConfigPath(mod), "slimerancher.prf");
                if (!File.Exists(path)) continue;
                var partial = new PartialProfileAchieveData();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    partial.Read(new BinaryReader(stream));
                }
                translator.FixEnumValues(EnumTranslator.TranslationMode.FROMTRANSLATED, partial);
                partial.Push(__instance.currentProfile.achievements);
            }
        }
    }
    [HarmonyPatch(typeof(SavedProfile))]
    [HarmonyPatch("SaveProfile")]
    internal static class SavedProfileSaveProfilePatch
    {
        public static void Prefix(SavedProfile __instance)
        {
            var translator = SaveRegistry.GenerateEnumTranslator();
            bool doWriteTranslator = false;
            foreach(var mod in SRModLoader.GetMods())
            {
                SRMod.ForceModContext(mod);
                if (CustomChecker.GetCustomLevel(__instance.currentProfile.achievements) == CustomChecker.CustomLevel.PARTIAL)
                {
                    doWriteTranslator = true;
                    var path = Path.Combine(FileSystem.GetConfigPath(mod), "slimerancher.prf");
                    var partial = new PartialProfileAchieveData();
                    partial.Pull(__instance.currentProfile.achievements);
                    translator.FixEnumValues(EnumTranslator.TranslationMode.TOTRANSLATED, partial);
                    using(var stream = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        partial.Write(new BinaryWriter(stream));
                    }
                }

                SRMod.ClearModContext();
            }

            if (doWriteTranslator)
            {
                var enumTranslatorPath = Path.Combine(Application.persistentDataPath, "SRML", "DATA.bin");
                using (var stream = new FileStream(enumTranslatorPath, FileMode.OpenOrCreate))
                {
                    translator.Write(new BinaryWriter(stream));
                }
            }
        }
    }
}

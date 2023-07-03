using System.Collections.Generic;
using HarmonyLib;
using SRML.Core;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(ResourceBundle))]
    [HarmonyPatch("LoadFromText")]
    internal static class ResourceBundlePatch
    {
        [HarmonyPriority(Priority.Last)]
        static void Postfix(string path, Dictionary<string, string> __result, string text)
        {
            CoreTranslator translations = CoreTranslator.Instance;
            MessageDirector.Lang lang = translations.currLang;

            if (path == "ui")
            {
                if (translations.srmlErrorMessages.ContainsKey(lang))
                {
                    __result["t.srml_error"] = translations.srmlErrorMessages[lang].Key;
                    __result["t.srml_error.abort"] = translations.srmlErrorMessages[lang].Value;
                }
                else
                {
                    __result["t.srml_error"] = translations.srmlErrorMessages[MessageDirector.Lang.EN].Key;
                    __result["t.srml_error.abort"] = translations.srmlErrorMessages[MessageDirector.Lang.EN].Value;
                }
            }

            translations.cachedBundles[path] = __result;

            if (!translations.patchesForLang.ContainsKey(lang))
            {
                if (!translations.patchesForLang.ContainsKey(MessageDirector.Lang.EN))
                    return;

                lang = MessageDirector.Lang.EN;
            }

            if (!translations.patchesForLang[lang].TryGetValue(path, out var dict)) 
                return;

            foreach (var entry in dict)
                __result[entry.Key] = entry.Value;
        }
    }
}

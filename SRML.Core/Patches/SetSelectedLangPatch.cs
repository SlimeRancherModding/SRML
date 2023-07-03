using HarmonyLib;

namespace SRML.Core.Patches
{
    [HarmonyPatch(typeof(MessageDirector), "GetBundle")]
    internal static class SetSelectedLangPatch
    {
        public static void Postfix(MessageDirector __instance, string path)
        {
            if (path == "global")
                CoreTranslator.Instance.ClearCache(__instance.GetCultureLang());
        }
    }
}

using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(BaseUI), "Awake")]
    internal static class BugReportUIDestroyPatch
    {
        public static void Prefix(BaseUI __instance)
        {
            if (__instance is BugReportUI)
            {
                GameObject.Destroy(__instance.gameObject);
                GameContext.Instance.UITemplates.CreateErrorDialog("Bug reports are disabled when SRML is installed!").AddComponent<UIInputLocker>();
            }
        }
    }
}

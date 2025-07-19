using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(BaseUI), "Awake")]
    internal static class BaseUINullCheckPatch
    {
        public static bool Prefix() => GameContext.Instance?.MessageDirector != null;
    }
}

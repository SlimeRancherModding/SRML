using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(SlimeAppearanceDirector), "UnlockAppearance")]
    internal static class UnlockAppearancePatch
    {
        internal static bool Prefix(SlimeDefinition slimeDefinition, SlimeAppearance appearance)
        {
            if (slimeDefinition == null || appearance == null)
                return false;
            return true;
        }
    }
    [HarmonyPatch(typeof(SlimeAppearanceDirector), "UpdateChosenSlimeAppearance")]
    internal static class UpdateChosenSlimeAppearancePatch
    {
        internal static bool Prefix(SlimeDefinition definition, SlimeAppearance newChosenAppearance)
        {
            if (definition == null || newChosenAppearance == null)
                return false;
            return true;
        }
    }
}
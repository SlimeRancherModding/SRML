using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.API.Gadget.Patches
{
    [HarmonyPatch(typeof(GadgetsModel), "InitInitialBlueprints")]
    internal static class InitInitialBlueprintsPatch
    {
        public static void Prefix(GadgetsModel __instance)
        {
            __instance.availBlueprints.UnionWith(BlueprintRegistry.Instance.defaultAvail);
            __instance.availBlueprints.UnionWith(BlueprintRegistry.Instance.defaultUnlocked);
            __instance.blueprints.UnionWith(BlueprintRegistry.Instance.defaultUnlocked);
        }
    }
}

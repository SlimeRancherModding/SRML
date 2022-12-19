using HarmonyLib;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(ScenePrefabInstantiator), "InstantiateGadget")]
    internal static class SkipMissingGadgetsPatch
    {
        public static bool Prefix(ScenePrefabInstantiator __instance, Gadget.Id id) =>
            __instance.lookupDir.HasGadgetDefinition(id);
    }
}

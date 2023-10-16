using HarmonyLib;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(ScenePrefabInstantiator), "InstantiateActor")]
    internal static class SkipMissingActorsPatch
    {
        public static bool Prefix(ScenePrefabInstantiator __instance, Identifiable.Id id) =>
            __instance.lookupDir.GetPrefab(id) != null;
    }
}

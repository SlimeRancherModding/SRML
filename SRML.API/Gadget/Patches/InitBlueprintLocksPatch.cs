using HarmonyLib;

namespace SRML.API.Gadget.Patches
{
    [HarmonyPatch(typeof(GadgetDirector), "InitBlueprintLocks")]
    internal static class InitBlueprintLocksPatch
    {
        public static void Postfix(GadgetDirector __instance)
        {
            foreach (var bpLock in BlueprintRegistry.Instance.blueprintLocks)
                __instance.blueprintLocks[bpLock.Key] = bpLock.Value.Invoke(__instance);
        }
    }
}

using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GameContext), "Awake")]
    internal static class GameContextInitializeDefinitionsPatch
    {
        public static void Postfix(GameContext __instance) => SlimeRegistry.Initialize(__instance.SlimeDefinitions);
    }
}

using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PlayerState))]
    [HarmonyPatch("OnEnteredZone")]
    internal static class EnterZonePatch
    {
        public static void Postfix(ZoneDirector.Zone zone, PlayerState __instance) => SRCallbacks.OnZoneEnteredCallback(zone, __instance);
    }
}

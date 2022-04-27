using HarmonyLib;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(PlayerState), "OnEnteredZone")]
    internal static class EnterZonePatch
    {
        public static void Postfix(ZoneDirector.Zone zone, PlayerState __instance) => SRCallbacks.OnZoneEnterCallback(zone, __instance);
    }
}

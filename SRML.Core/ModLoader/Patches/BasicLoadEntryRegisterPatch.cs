using HarmonyLib;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;

namespace SRML.Core.ModLoader.Patches
{
    [HarmonyPatch(typeof(GameContext), "Start")]
    internal static class BasicLoadEntryRegisterPatch
    {
        public static void Prefix() => BasicLoadEntryPoint.GameContextLoad();
        public static void Postfix() => BasicLoadEntryPoint.GameContextPostLoad();
    }
}

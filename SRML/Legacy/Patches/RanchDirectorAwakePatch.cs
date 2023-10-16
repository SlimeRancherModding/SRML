using HarmonyLib;
using System.Linq;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(RanchDirector), "Awake")]
    internal static class RanchDirectorAwakePatch
    {
        public static void Postfix(RanchDirector __instance)
        {
            foreach (RanchDirector.PaletteEntry entry in ChromaRegistry.customPalettes)
                __instance.RegisterPalette(entry);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(RanchModel))]
    [HarmonyPatch("SelectPalette")]
    internal static class RanchPaletteSelectPatch
    {
        public static void Prefix(RanchModel __instance, RanchDirector.PaletteType type, ref RanchDirector.Palette pal)
        {
            if (!Enum.IsDefined(typeof(RanchDirector.Palette), pal))
                pal = ChromaPackRegistry.GetDefaultPaletteForType(type);
        }
    }
}

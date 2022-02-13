using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(RanchDirector))]
    [HarmonyPatch("Awake")]
    internal static class RanchAwakePatch
    {
        public static void Prefix(RanchDirector __instance)
        {
            List<RanchDirector.PaletteEntry> list = __instance.palettes.ToList();
            foreach (RanchDirector.PaletteEntry paletteEntry in ChromaPackRegistry.moddedPaletteEntries)
            {
                if (!list.Contains(paletteEntry))
                    list.Add(paletteEntry);
            }
            __instance.palettes = list.ToArray();
        }
    }
}

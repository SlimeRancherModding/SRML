using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(FileStorageProvider))]
    [HarmonyPatch("DeleteGameData")]
    internal static class DeleteGameDataPatch
    {
        public static void Postfix(FileStorageProvider __instance, string name)
        {
            string fullMod = SaveHandler.GetModdedPath(__instance, name);
            if (File.Exists(fullMod))
            {
                File.Delete(fullMod);
            }
        }
    }
}

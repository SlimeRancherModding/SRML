using HarmonyLib;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(AutoSaveDirector), "LoadSave")]
    internal static class LoadSaveMethodPatch
    {
        public static void Prefix()
        {
            ExtendedData.handlersInSave.Clear();
            ExtendedData.gadgetsInSave.Clear();
            ExtendedData.landplotsInSave.Clear();
        }
    }
}

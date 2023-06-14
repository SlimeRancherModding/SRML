using HarmonyLib;
using System.Linq;

namespace SRML.API.Gadget.Patches
{
    [HarmonyPatch(typeof(GadgetDirector), "IsRefineryResource")]
    internal static class GadgetDirectorIsRefineryResourcePatch
    {
        public static void Postfix(global::Identifiable.Id id, ref bool __result) =>
            __result = __result || RefineryRegistry.Instance.Registered.Contains(id);
    }
}

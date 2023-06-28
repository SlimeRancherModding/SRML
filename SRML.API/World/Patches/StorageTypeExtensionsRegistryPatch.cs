using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.World.Patches
{
    [HarmonyPatch(typeof(StorageTypeExtensions), "GetContents")]
    internal static class StorageTypeExtensionsRegistryPatch
    {
        public static void Postfix(SiloStorage.StorageType type, HashSet<global::Identifiable.Id> __result) =>
            __result.UnionWith(SiloAmmoRegistry.Instance.Registered.Where(x => x.Item1 == type).Select(x => x.Item2));
    }
}

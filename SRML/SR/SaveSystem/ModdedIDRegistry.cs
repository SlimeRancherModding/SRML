using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.Persist;
using SRML;
using SRML.SR.SaveSystem;

static internal class ModdedIDRegistry
{
    internal static Dictionary<Type,IIDRegistry> moddedIdRegistries = new Dictionary<Type, IIDRegistry>();

    internal static void RegisterIDRegistry(IIDRegistry registry)
    {
        moddedIdRegistries[registry.RegistryType] = registry;
    }

    public static bool HasModdedID(object data)
    {
        return (data is ActorDataV07 actor && IsModdedID((Identifiable.Id)actor.typeId))||
               (data is PlacedGadgetV06 gadget && IsModdedID(gadget.gadgetId));
    }

    public static bool IsModdedID(object data)
    {
        return moddedIdRegistries.Any((x) => x.Key == data.GetType() && x.Value.IsModdedID(data));
    }

    internal static SRMod ModForID(object data)
    {
        if (!IsModdedID(data)) return null;

        return moddedIdRegistries.FirstOrDefault((x) => x.Key == data.GetType()).Value?.GetModForID(data);
    }
}
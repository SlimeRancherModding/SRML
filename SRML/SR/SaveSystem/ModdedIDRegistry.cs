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

    public static bool IsModdedID(object id)
    {
        if (!id.GetType().IsEnum) throw new Exception(id.GetType() + " is not an enum!");
        return moddedIdRegistries.Any((x) => x.Key == id.GetType() && x.Value.IsModdedID(id));
    }

    public static bool IsModdedID<T>(T id)
    {
        return IsModdedID((object)id);
    }

    internal static SRMod ModForID(object data)
    {
        
        if (!IsModdedID(data)) return null;
        return moddedIdRegistries.FirstOrDefault((x) => x.Key == data.GetType()).Value?.GetModForID(data);
    }
}
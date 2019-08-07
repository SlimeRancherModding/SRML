using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public  static class PlantSlotRegistry
    {
        public delegate void GardenPatchDelegate(GardenCatcher catcher);
        internal static List<GardenPatchDelegate> patchers = new List<GardenPatchDelegate>();

        internal static void Patch(GardenCatcher catcher) => patchers.ForEach(x => x(catcher));
        
        public static void RegisterGardenCatcherPatcher(GardenPatchDelegate del)
        {
            if (!patchers.Contains(del)) patchers.Add(del);
        }

        public static void RegisterPlantSlot(GardenCatcher.PlantSlot plantSlot)
        {
            RegisterGardenCatcherPatcher(x => x.plantable = x.plantable.Where(y=>y.id!=plantSlot.id).ToArray().AddToArray(plantSlot));
        }

        public static void RegisterPlantSlotFor(Predicate<GardenCatcher> condition, GardenCatcher.PlantSlot slot)
        {
            RegisterGardenCatcherPatcher(x => {if (condition(x)) x.plantable = x.plantable.Where(y => y.id != slot.id).ToArray().AddToArray(slot); });
        }
    }
}

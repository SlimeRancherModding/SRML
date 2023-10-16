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
        
        /// <summary>
        /// Registers an event to be run when a <see cref="GardenCatcher"/> awakes.
        /// </summary>
        /// <param name="del">The event to be ran.</param>
        public static void RegisterGardenCatcherPatcher(GardenPatchDelegate del)
        {
            if (!patchers.Contains(del))
                patchers.Add(del);
        }

        /// <summary>
        /// Registers a link between a plant plot and an <see cref="Identifiable.Id"/>.
        /// </summary>
        /// <param name="plantSlot">The entry of the link.</param>
        public static void RegisterPlantSlot(GardenCatcher.PlantSlot plantSlot) =>
            RegisterGardenCatcherPatcher(x => x.plantable = x.plantable.Where(y => y.id != plantSlot.id).ToArray().AddToArray(plantSlot));

        /// <summary>
        /// Registers a link between a plant plot and an <see cref="Identifiable.Id"/>, which only is used if a condition is met.
        /// </summary>
        /// <param name="condition">The condition that the link is established.</param>
        /// <param name="slot">The entry of the link.</param>
        public static void RegisterPlantSlotFor(Predicate<GardenCatcher> condition, GardenCatcher.PlantSlot slot) =>
            RegisterGardenCatcherPatcher(x => {if (condition(x)) x.plantable = x.plantable.Where(y => y.id != slot.id).ToArray().AddToArray(slot); });
    }
}

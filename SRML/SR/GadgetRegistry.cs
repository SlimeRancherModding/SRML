using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class GadgetRegistry
    {
        internal static Dictionary<Gadget.Id, SRMod> moddedGadgets = new Dictionary<Gadget.Id, SRMod>();

        public static Gadget.Id CreateGadgetId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register gadgets outside of the PreLoad step");
            var id = (Gadget.Id) value;
            if (moddedGadgets.ContainsKey(id))
                throw new Exception(
                    $"Gadget {value} is already registered to {moddedGadgets[id].ModInfo.Id}");
            var sr = SRMod.GetCurrentMod();
            if (sr != null) moddedGadgets[id] = sr;
            EnumPatcher.AddEnumValue(typeof(Gadget.Id), id, name);
            return id;
        }

        public static bool IsModdedGadget(Gadget.Id id)
        {
            return moddedGadgets.ContainsKey(id);
        }
    }
}

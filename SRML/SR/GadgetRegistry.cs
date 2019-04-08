using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class GadgetRegistry
    {
        internal static IDRegistry<Gadget.Id> moddedGadgets = new IDRegistry<Gadget.Id>();

        static GadgetRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedGadgets);
        }

        public static Gadget.Id CreateGadgetId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register gadgets outside of the PreLoad step");
            return moddedGadgets.RegisterValueWithEnum((Gadget.Id) value, name);
        }

        public static bool IsModdedGadget(Gadget.Id id)
        {
            return moddedGadgets.ContainsKey(id);
        }
    }
}

using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class LandPlotRegistry
    {
        internal static IDRegistry<LandPlot.Id> landplots = new IDRegistry<LandPlot.Id>();
        static LandPlotRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(landplots);
        }

        public static LandPlot.Id CreateLandPlotId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register landplots outside of the PreLoad step");
            return landplots.RegisterValueWithEnum((LandPlot.Id)value, name);
        }

        public static bool IsModdedLandPlot(LandPlot.Id id)
        {
            return landplots.ContainsKey(id);
        }
    }
}

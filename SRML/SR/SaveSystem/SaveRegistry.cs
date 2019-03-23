using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.SR.SaveSystem.Registry;

namespace SRML.SR.SaveSystem
{
    public static class SaveRegistry
    {
        internal static Dictionary<SRMod,ModSaveInfo> modToSaveInfo = new Dictionary<SRMod, ModSaveInfo>();

        internal static ModSaveInfo GetSaveInfo(SRMod mod)
        {
            if (!modToSaveInfo.ContainsKey(mod)) modToSaveInfo.Add(mod,new ModSaveInfo());
            return modToSaveInfo[mod];
        }

        internal static ModSaveInfo GetSaveInfo()
        {
            return GetSaveInfo(SRMod.GetCurrentMod());
        }





    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
namespace SRML
{
    public abstract class ModEntryPoint 
    {
        public virtual void PreLoad(HarmonyInstance instance)
        {

        }

        public virtual void PostLoad()
        {

        }

        public virtual SRModInfo GetModInfo() { return new SRModInfo("NULL","NULL",SRModInfo.ModVersion.DEFAULT);}

    }
}

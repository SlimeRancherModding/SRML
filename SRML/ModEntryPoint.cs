using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
namespace SRML
{
    public abstract class ModEntryPoint 
    {
        public HarmonyInstance HarmonyInstance { get; internal set; }

        public virtual void PreLoad()
        {

        }

        public virtual void PostLoad()
        {

        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigReloadAttribute : Attribute
    {
        public ReloadMode Mode;
        public ConfigReloadAttribute(ReloadMode mode)
        {
            Mode = mode;
        }
    }
}

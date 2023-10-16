using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigFileAttribute : Attribute
    {
        internal string FileName;
        internal string DefaultSection;
        public ConfigFileAttribute(string name, string defaultsection = "CONFIG")
        {
            FileName = name;
            DefaultSection = defaultsection;
        }
    }
}

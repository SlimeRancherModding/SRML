using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigNameAttribute : Attribute
    {
        public string Name;
        public ConfigNameAttribute(string name)
        {
            Name = name;
        }
    }
}

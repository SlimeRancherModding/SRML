using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigSectionAttribute : Attribute
    {
        internal string SectionName;
        public ConfigSectionAttribute()
        {
        }

        public ConfigSectionAttribute(string sectionname)
        {
            this.SectionName = sectionname;
        }
    }
}

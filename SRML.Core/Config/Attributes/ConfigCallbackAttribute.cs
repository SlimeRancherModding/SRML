using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigCallbackAttribute : Attribute
    {
        public string methodName;

        public ConfigCallbackAttribute(string methodName)
        {
            this.methodName = methodName;
        }
    }
}

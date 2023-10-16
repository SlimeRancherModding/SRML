using SRML.Config.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigParserAttribute : Attribute
    {
        public IStringParser Parser;
        public ConfigParserAttribute(IStringParser parser)
        {
            Parser = parser;
        }
    }
}

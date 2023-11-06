using SRML.Core.API.BuiltIn.Attributes;
using System;

namespace SRML.API.Gadget.Attributes
{
    public class GadgetCategorizationAttribute : EnumCategorizerAttribute
    {
        public GadgetCategorizationAttribute(Rule rule) : base(rule)
        {
        }

        public override Type RuleType => typeof(Rule);


        [Flags]
        public enum Rule
        {
            NONE = 0,
            MISC = 1,
            EXTRACTOR = 2,
            TELEPORTER = 4,
            WARP_DEPOT = 8,
            ECHO_NET = 16,
            LAMP = 32,
            FASHION_POD = 64,
            SNARE = 128,
            DECO = 256,
            DRONE = 512
        }
    }
}

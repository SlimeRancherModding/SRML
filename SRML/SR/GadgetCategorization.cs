using System;

namespace SRML.SR
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GadgetCategorization : Attribute
    {
        public Rule rules;

        public GadgetCategorization(Rule rules) => this.rules = rules;

        [Flags]
        public enum Rule
        {
            MISC = 0,
            EXTRACTOR = 1,
            TELEPORTER = 2,
            WARP_DEPOT = 4,
            ECHO_NET = 8,
            LAMP = 16,
            FASHION_POD = 32,
            SNARE = 64,
            DECO = 128,
            DRONE = 256
        }
    }
}

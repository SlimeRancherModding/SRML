using System;
using System.Collections.Generic;

namespace SRML.SR
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GadgetCategorization : Attribute
    {
        public static List<Gadget.Id> doNotAutoCategorize = new List<Gadget.Id>();

        public GadgetCategorization(Rule rules) => this.rules = rules;

        public Rule rules;

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

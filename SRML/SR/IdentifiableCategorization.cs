using System;
using System.Collections.Generic;

namespace SRML.SR
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IdentifiableCategorization : Attribute
    {
        public static List<Identifiable.Id> doNotAutoCategorize = new List<Identifiable.Id>();

        public IdentifiableCategorization(Rule rules) => this.rules = rules;
        public Rule rules;

        [Flags]
        public enum Rule
        {
            NONE = 0,
            VEGGIE = 1,
            FRUIT = 2,
            TOFU = 4,
            SLIME = 8,
            LARGO = 16,
            GORDO = 32,
            PLORT = 64,
            MEAT = 128,
            CHICK = 256,
            LIQUID = 512,
            CRAFT = 1024,
            FASHION = 2048,
            ECHO = 4096,
            ECHO_NOTE = 8192,
            ORNAMENT = 16384,
            TOY = 32768
        }
    }
}

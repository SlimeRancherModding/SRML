using SRML.API.Identifiable.Processors;
using SRML.Core.API.BuiltIn.Attributes;
using System;

namespace SRML.API.Identifiable.Attributes
{
    public class IdentifiableCategorizationAttribute : EnumCategorizerAttribute
    {
        public IdentifiableCategorizationAttribute(Rule rule) : base(rule) { }

        public override Type RuleType => typeof(Rule);
        public override Type ProcessorType => typeof(IdentifiableProcessor);

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
            TOY = 32768,
            ELDER = 65536
        }
    }
}

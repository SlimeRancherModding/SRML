using System;

namespace SRML.Core.API.BuiltIn.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class EnumCategorizerAttribute : Attribute
    {
        public abstract Type RuleType { get; }
        public abstract Type ProcessorType { get; }

        internal readonly Enum rule;

        protected EnumCategorizerAttribute(Enum rule)
        {
            if (rule.GetType() != RuleType)
                throw new ArgumentException("Invalid rule type");

            this.rule = rule;
        }
    }
}

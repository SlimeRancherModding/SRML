using System;

namespace SRML.Core.SRML.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumForwardedFromAttribute : Attribute
    {
        public readonly Type ForwardedFrom;

        public EnumForwardedFromAttribute(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Must be an enum type");

            ForwardedFrom = type;
        }
    }
}

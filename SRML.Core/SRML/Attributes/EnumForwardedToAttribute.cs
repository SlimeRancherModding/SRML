using System;

namespace SRML.Core.SRML.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumForwardedToAttribute : Attribute
    {
        public readonly Type ForwardedTo;

        public EnumForwardedToAttribute(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Must be an enum type");

            ForwardedTo = type;
        }
    }
}

using System;

namespace SRML.Core.API.BuiltIn
{
    public class EnumObject<TEnum> where TEnum : Enum
    {
        public object Value;
        public string Name;

        public EnumObject(string name)
        {
            Value = null;
            Name = name;
        }

        public EnumObject(string name, object value)
        {
            Value = value;
            Name = name;
        }
    }
}

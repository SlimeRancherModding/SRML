using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Core.API.BuiltIn.Processors
{
    public abstract class EnumProcessor<TEnum> : Processor<TEnum> where TEnum : Enum
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public TEnum EnumValue { get; private set; }

        public override void Register()
        {
            if (Value == null)
                Value = EnumPatcher.AddEnumValue<TEnum>(Name);
            else
                EnumPatcher.AddEnumValue<TEnum>(Value, Name);

            EnumValue = (TEnum)Value;
        }

        public override bool IsIdenticalTo(Processor<TEnum> other)
        {
            EnumProcessor<TEnum> otherEnum = (EnumProcessor<TEnum>)other;
            return otherEnum.Name == Name || otherEnum.Value == Value;
        }

        protected virtual void OnCategorize(TEnum toCategorize)
        {
        }
        public virtual void Categorize(TEnum value)
        {
            OnCategorize(value);
        }
        public virtual void Decategorize(TEnum value)
        {
        }

        public EnumProcessor(string name) : base()
        {
            Name = name;
        }
        public EnumProcessor(string name, object value) : base()
        {
            Name = name;
            Value = value;
        }
    }
}

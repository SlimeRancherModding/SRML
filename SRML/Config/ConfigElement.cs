using SRML.Config.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config
{
    public class ConfigElement<T> : ConfigElement
    {
        public override Type ElementType => typeof(T);

        public ConfigElement(ConfigElementOptions options)
        {
            this.Options = options;
            if (Options.DefaultValue != null) SetValue(Options.DefaultValue);
        }

        public ConfigElement() : this(GenerateDefaultOptions(typeof(T)))
        {

        }
    }
    public abstract class ConfigElement
    {
        public abstract Type ElementType { get; }
        public ConfigElementOptions Options { get; protected set; }
        protected object Value;
        public T GetValue<T>()
        {
            return (T)Value;
        }

        public void SetValue<T>(T value)
        {
            Value = value;
        }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object value)
        {
            Value = value;
        }

        public static ConfigElementOptions GenerateDefaultOptions(Type type)
        {
            return new ConfigElementOptions()
            {
                Parser = ParserRegistry.GetParser(type),
                DefaultValue = type.IsValueType ? Activator.CreateInstance(type) : null
            };
        }
    }
    
    public class ConfigElementOptions
    {
        public IStringParser Parser { get; internal set; }
        public string Comment { get; internal set; } = "";
        public object DefaultValue { get; internal set; }


    }
}

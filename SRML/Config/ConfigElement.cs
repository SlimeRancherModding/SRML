using SRML.Config.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SRML.Config
{

    public class FieldBackedConfigElement : ConfigElement
    {

        protected FieldInfo field;
        public override Type ElementType => field.FieldType;

        protected override object Value
        {
            get { return field.GetValue(null); }
            set { field.SetValue(null, value); }
        }

        public FieldBackedConfigElement(FieldInfo field) : base(GenerateDefaultOptions(field.FieldType,field.Name)) 
        {
            this.field = field;
            Options.DefaultValue = Value;

        }
    }
    public class ConfigElement<T> : ConfigElement
    {
        public override Type ElementType => typeof(T);

        protected override object Value { get; set; }



        public ConfigElement(string name) : base(GenerateDefaultOptions(typeof(T),name))
        {
            if (Options.DefaultValue != null) SetValue(Options.DefaultValue);
        }
    }
    public abstract class ConfigElement
    {
        public abstract Type ElementType { get; }
        public ConfigElementOptions Options { get; protected set; }
        protected abstract object Value { get; set; }
        public T GetValue<T>()
        {
            return (T)Value;
        }

        public ConfigElement(ConfigElementOptions options)
        {
            this.Options = options;
            
        }

        public void SetValue<T>(T value)
        {
            Value = value;
        }

        public static ConfigElementOptions GenerateDefaultOptions(Type type,string name)
        {
            return new ConfigElementOptions()
            {
                Parser = ParserRegistry.GetParser(type),
                DefaultValue = type.IsValueType ? Activator.CreateInstance(type) : null,
                Name = name
            };
        }
    }

    public class ConfigElementComparer : IEqualityComparer<ConfigElement>
    {
        public bool Equals(ConfigElement x, ConfigElement y)
        {
            return x.Options.Name == y.Options.Name;
        }

        public int GetHashCode(ConfigElement obj)
        {
            throw new NotImplementedException();
        }
    }

    public class ConfigElementOptions
    {
        public IStringParser Parser { get; internal set; }
        public string Comment { get; internal set; }
        public object DefaultValue { get; internal set; }

        public string Name { get; internal set; }
    }
}

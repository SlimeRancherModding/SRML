using MonomiPark.SlimeRancher.Persist;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Partial
{
    public static class PartialDataUtils
    {
        public static PartialDictionary<string, T> CreatePartialDictionaryWithStringKey<T>(SerializerPair<T> pair)
        {
            return new PartialDictionary<string, T>(x => ModdedStringRegistry.IsValidModdedString(x.Key), SerializerPair.STRING, pair, checkValueValid: x => ModdedStringRegistry.IsValidModdedString(x.Key));
        }

        public static PartialDictionary<K, T> CreatePartialDictionaryWithEnumKey<K, T>(SerializerPair<T> pair) => new PartialDictionary<K, T>(x => ModdedIDRegistry.IsModdedID(x.Key), SerializerPair.GetEnumSerializerPair<K>(), pair, checkValueValid: x => ModdedIDRegistry.IsModdedID(x.Key));
        public static PartialDictionary<string, T> CreatePartialDictionaryWithStringKey<T>() where T : PersistedDataSet, new()
        {
            return CreatePartialDictionaryWithStringKey(CreateSerializerForPersistedDataSet<T>());
        }

        public static SerializerPair<T> CreateSerializerForPersistedDataSet<T>() where T : PersistedDataSet, new() => new SerializerPair<T>((x, y) => y.Write(x.BaseStream), (x) => { var v = new T(); v.Load(x.BaseStream); return v; });

        public static PartialCollection<string> CreatePartialModdedStringCollection() => new PartialCollection<string>(ModdedStringRegistry.IsValidModdedString, SerializerPair.STRING, ModdedStringRegistry.IsValidModdedString);

        

        public static object GetEnumNullValue(Type enumType)
        {
            
            return Enum.IsDefined(enumType, "NONE") ? Enum.Parse(enumType, "NONE", true) : Enum.ToObject(enumType,EnumTranslator.SmallestValue(enumType)); 
        }

        
        public static Predicate<object> CreateForbiddenValueTester(Type type)
        {
            if (type.IsGenericType && typeof(KeyValuePair<,>) == type.GetGenericTypeDefinition())
            {
                var args = type.GetGenericArguments();
                var key = CreateForbiddenValueTester(args[0]);
                var value = CreateForbiddenValueTester(args[1]);
                var keyProp = type.GetProperty("Key");
                var valueProp = type.GetProperty("Value");
                return (x) => key(keyProp.GetValue(x, null)) && value(valueProp.GetValue(x, null));
            }
            else if (type.IsEnum)
            {
                var none = GetEnumNullValue(type);
                if (none == null) return x => true;
                return x => none == x;
            }
            else if (typeof(string).IsAssignableFrom(type))
            {
                return x => ModdedStringRegistry.IsValidModdedString((string)x);
            }
            else return x => true;

        }
    }
}

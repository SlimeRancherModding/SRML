using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using SRML.SR;
using SRML.Utils;
using SRML.Core.ModLoader;
using SRML.Core.API.BuiltIn;
using SRML.Core.API;
using SRML.Console;

namespace SRML
{
    /// <summary>
    /// Allows adding values to any Enum
    /// </summary>
    public static class EnumPatcher
    {
        public delegate object AlternateEnumRegister(object value, string name);
        internal static readonly Dictionary<Type, IEnumRegistry> registriesForType = new Dictionary<Type, IEnumRegistry>();
        internal static readonly List<IAttributeCategorizeableEnum> categorizableRegistries = new List<IAttributeCategorizeableEnum>();

        [Obsolete]
        public static void RegisterAlternate<TEnum>(AlternateEnumRegister del) where TEnum : Enum => RegisterAlternate(typeof(TEnum), del);

        [Obsolete]
        public static void RegisterAlternate(Type type, AlternateEnumRegister del) =>
            throw new NotSupportedException("Using AlternateEnumRegister is no longer supported.");

        public static void RegisterEnumRegistry(Type enumType, IEnumRegistry registry)
        {
            registriesForType.Add(enumType, registry);
            if (registry is IAttributeCategorizeableEnum categorizable)
                categorizableRegistries.Add(categorizable);
        }

        private static FieldInfo cache;
        
        private static Dictionary<Type, EnumPatch> patches = new Dictionary<Type, EnumPatch>();

        static EnumPatcher()
        {
            var t = AccessTools.TypeByName("System.RuntimeType");
            cache = AccessTools.Field(t, "GenericCache");
        }

        /// <summary>
        /// Add a new enum value to the given <paramref name="T"/> with the first free value
        /// </summary>
        /// <param name="T">Type of enum to add the value to</param>
        /// <param name="name">Name of the new enum value</param>
        /// <returns>The new enum value</returns>
        public static TEnum AddEnumValue<TEnum>(string name) where TEnum : Enum => (TEnum)AddEnumValue(typeof(TEnum), name);

        /// <summary>
        /// Add a new enum value to the given <paramref name="enumType"/> with the first free value
        /// </summary>
        /// <param name="enumType">Type of enum to add the value to</param>
        /// <param name="name">Name of the new enum value</param>
        /// <returns>The new enum value</returns>
        public static object AddEnumValue(Type enumType, string name)
        {
            var newVal = GetFirstFreeValue(enumType);
            AddEnumValue(enumType, newVal, name);
            return newVal;
        }

        /// <summary>
        /// Add a new value to the given <paramref name="T"/> 
        /// </summary>
        /// <param name="T">Enum to add the new value to</param>
        /// <param name="value">Value to add to the enum</param>
        /// <param name="name">The name of the new value</param>
        public static void AddEnumValue<T>(object value, string name) => AddEnumValue(typeof(T), value, name);

        /// <summary>
        /// Add a new value to the given <paramref name="enumType"/> 
        /// </summary>
        /// <param name="enumType">Enum to add the new value to</param>
        /// <param name="value">Value to add to the enum</param>
        /// <param name="name">The name of the new value</param>
        public static void AddEnumValue(Type enumType, object value, string name)
        {
            if (enumType == null) 
                throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum)
                throw new Exception($"{enumType} is not a valid Enum!");
            if (AlreadyHasName(enumType, name) || EnumUtils.HasEnumValue(enumType, name)) 
                throw new Exception($"The enum ({enumType.FullName}) already has a value with the name \"{name}\"");

            value = (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
            if (!patches.TryGetValue(enumType, out var patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            ClearEnumCache(enumType);

            patch.AddValue((ulong)value, name);

            if (registriesForType.TryGetValue(enumType, out IEnumRegistry enumReg))
            {
                Enum newVal = (Enum)Enum.ToObject(enumType, (ulong)value);

                if (enumReg is ICategorizableEnum categorizable)
                    categorizable.Categorize(newVal);

                enumReg.Process(newVal);
            }
        }

        [Obsolete]
        public static void AddEnumValueWithAlternatives<TEnum>(object value, string name) where TEnum : Enum => AddEnumValue(typeof(TEnum), value, name);

        [Obsolete]
        public static void AddEnumValueWithAlternatives(Type enumType, object value, string name) => AddEnumValue(enumType, value, name);

        internal static bool TryAsNumber(this object value, Type type, out object result)
        {
            if (type.IsSubclassOf(typeof(IConvertible)))
                throw new ArgumentException("The type must inherit the IConvertible interface", "type");
            result = null;
            if (type.IsInstanceOfType(value))
            {
                result = value;
                return true;
            }
            if (value is IConvertible)
            {
                if (type.IsEnum)
                {
                    result = Enum.ToObject(type, value);
                    return true;
                }
                var format = NumberFormatInfo.CurrentInfo;
                result = (value as IConvertible).ToType(type, format);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <param name="T"></param>
        /// <returns>The first undefined enum value</returns>
        public static TEnum GetFirstFreeValue<TEnum>() => (TEnum)GetFirstFreeValue(typeof(TEnum));

        private static readonly Converter<Enum, int> converter = new Converter<Enum, int>(x => Convert.ToInt32(x));
        private static readonly Converter<Enum, uint> uConverter = new Converter<Enum, uint>(x => Convert.ToUInt32(x));

        private static object GetFirstFreeValueRegular(Type enumType)
        {
            var vals = Array.ConvertAll(Enum.GetValues(enumType).OfType<Enum>().ToArray(), converter);
            Array.Sort(vals);
            var length = vals.Length;
            int index = vals.IndexOfItem(vals.FirstOrDefault(x => x > -1));

            for (int i = index == -1 ? 0 : index; i <= int.MaxValue; i++)
            {
                if (i == length || vals[i] != i)
                    return i;
            }

            if (index > 0)
            {
                for (int i = index; i >= int.MinValue; i--)
                {
                    if (i == length || vals[i] != i)
                        return i;
                }
            }

            throw new Exception("No unused values in enum " + enumType.FullName);
        }

        private static object GetFirstFreeValueFlags(Type enumType)
        {
            var vals = Array.ConvertAll(Enum.GetValues(enumType).OfType<Enum>().ToArray(), uConverter);
            Array.Sort(vals);
            var length = vals.Length;
            var magic = vals[0] == 0U ? 1U : 0U; // I genuinely couldn't think of a better name for this
                                                 // I couldn't tell you what this number actually is, but it makes the math work
                                                 // therefore, magic

            for (uint i = magic; i <= uint.MaxValue; i++)
            {
                uint pow = (uint)Math.Pow(2, i - magic);
                if (i == length || vals[i] != pow)
                    return pow;
            }

            throw new Exception("No unused values in enum " + enumType.FullName);
        }

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns>The first undefined enum value</returns>
        public static object GetFirstFreeValue(Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentException("enumType");
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new Exception($"{enumType} is not a valid Enum!");

            if (enumType.GetCustomAttributes<FlagsAttribute>().Any())
                return GetFirstFreeValueFlags(enumType);
            else
                return GetFirstFreeValueRegular(enumType);
        }

        public static void ClearEnumCache(Type enumType)
        {
            cache.SetValue(enumType, null);
        }
        
        internal static bool TryGetRawPatch(Type enumType, out EnumPatch patch)
        {
            return patches.TryGetValue(enumType, out patch);
        }

        internal static bool AlreadyHasName(Type enumType, string name)
        {
            if (TryGetRawPatch(enumType, out EnumPatch patch))
                return patch.HasName(name);
            return false;
        }

        public class EnumPatch 
        {
            private Dictionary<ulong, List<string>> values = new Dictionary<ulong, List<string>>();

            public void AddValue(ulong enumValue, string name)
            {
                if (values.ContainsKey(enumValue))
                    values[enumValue].Add(name);
                else
                    values.Add(enumValue, new List<string> { name });
            }

            public List<KeyValuePair<ulong, string>> GetPairs()
            {
                List<KeyValuePair<ulong, string>> pairs = new List<KeyValuePair<ulong, string>>();
                foreach (KeyValuePair<ulong, List<string>> pair in values)
                {
                    foreach (string value in pair.Value)
                        pairs.Add(new KeyValuePair<ulong, string>(pair.Key, value));
                }
                return pairs;
            }

            public bool HasName(string name)
            {
                foreach (string enumName in this.values.Values.SelectMany(l => l))
                {
                    if (name.Equals(enumName))
                        return true;
                }
                return false;
            }
        }
    }
}
    
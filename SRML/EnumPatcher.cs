﻿using System;
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

namespace SRML
{
    /// <summary>
    /// Allows adding values to any Enum
    /// </summary>
    public static class EnumPatcher
    {
        public delegate object AlternateEnumRegister(object value, string name);
        private static readonly Dictionary<Type, AlternateEnumRegister> BANNED_ENUMS = new Dictionary<Type, AlternateEnumRegister>()
        {
            { typeof(Identifiable.Id),(x,y)=>IdentifiableRegistry.CreateIdentifiableId(x,y)},
            { typeof(Gadget.Id),(x,y)=>(object)GadgetRegistry.CreateGadgetId(x,y)},
            { typeof(PlayerState.Upgrade),(x,y)=>(object)PersonalUpgradeRegistry.CreatePersonalUpgrade(x,y)},
            { typeof(PediaDirector.Id),(x,y)=>(object)PediaRegistry.CreatePediaId(x,y) },
            { typeof(LandPlot.Id),(x,y)=>(object)LandPlotRegistry.CreateLandPlotId(x,y) },
            { typeof(RanchDirector.Palette),(x,y)=>ChromaPackRegistry.CreatePalette(x,y)},
            { typeof(RanchDirector.PaletteType),(x,y)=>ChromaPackRegistry.CreatePaletteType(x,y)}
        };

        public static void RegisterAlternate<TEnum>(AlternateEnumRegister del) where TEnum : Enum => RegisterAlternate(typeof(TEnum), del);

        public static void RegisterAlternate(Type type, AlternateEnumRegister del)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!type.IsEnum) throw new Exception($"The given type isn't an enum ({type.FullName} isn't an Enum)");
            BANNED_ENUMS.Add(type, del);
        }

        private static FieldInfo cache;
        
        private static Dictionary<Type, EnumPatch> patches = new Dictionary<Type, EnumPatch>();

        static EnumPatcher()
        {
            var t = AccessTools.TypeByName("System.RuntimeType");
            cache = AccessTools.Field(t, "GenericCache");
        }

        /// <summary>
        /// Add a new enum value to the given <typeparamref name="TEnum"/> with the first free value
        /// </summary>
        /// <typeparam name="TEnum">Type of enum to add the value to</typeparam>
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
        /// Add a new value to the given <typeparamref name="TEnum"/> 
        /// </summary>
        /// <typeparam name="TEnum">Enum to add the new value to</typeparam>
        /// <param name="value">Value to add to the enum</param>
        /// <param name="name">The name of the new value</param>
        public static void AddEnumValue<TEnum>(object value, string name) where TEnum : Enum => AddEnumValue(typeof(TEnum), value, name);

        /// <summary>
        /// Add a new value to the given <paramref name="enumType"/> 
        /// </summary>
        /// <param name="enumType">Enum to add the new value to</param>
        /// <param name="value">Value to add to the enum</param>
        /// <param name="name">The name of the new value</param>
        public static void AddEnumValue(Type enumType, object value, string name)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new Exception($"{enumType} is not a valid Enum!");
            if (SRModLoader.GetModForAssembly(Assembly.GetCallingAssembly())!=null && BANNED_ENUMS.ContainsKey(enumType)) throw new Exception($"Patching {enumType} through EnumPatcher is not supported!");
            if (AlreadyHasName(enumType, name) || EnumUtils.HasEnumValue(enumType, name)) throw new Exception($"The enum ({enumType.FullName}) already has a value with the name \"{name}\"");

            value = (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
            if (!patches.TryGetValue(enumType, out var patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            ClearEnumCache(enumType);

            patch.AddValue((ulong)value, name);
        }
        
        public static void AddEnumValueWithAlternatives<TEnum>(object value, string name) where TEnum : Enum => AddEnumValueWithAlternatives(typeof(TEnum), value, name);

        public static void AddEnumValueWithAlternatives(Type enumType, object value, string name)
        {
            if (BANNED_ENUMS.TryGetValue(enumType, out var alternate)) alternate(value, name);
            else AddEnumValue(enumType, value, name);
        }

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns>The first undefined enum value</returns>
        public static TEnum GetFirstFreeValue<TEnum>() where TEnum : Enum => (TEnum)GetFirstFreeValue(typeof(TEnum));

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns>The first undefined enum value</returns>
        public static object GetFirstFreeValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new Exception($"The given type isn't an enum ({enumType.FullName} isn't an Enum)");

            var enumValues = EnumUtils.GetAll(enumType);
            if (enumValues == null || enumValues.Length == 0)
                return Enum.ToObject(enumType, 0);

            var allValues = enumValues.Cast<int>();
            var min = allValues.Min();
            var max = allValues.Max();
            for (int i = (min + 1); i < max; i++)
                if (!allValues.Contains(i))
                    return Enum.ToObject(enumType, i);

            return Enum.ToObject(enumType, max + 1);
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
    
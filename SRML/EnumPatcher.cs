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

namespace SRML
{
    /// <summary>
    /// Allows adding values to any Enum
    /// </summary>
    public static class EnumPatcher
    {
        internal delegate object AlternateEnumRegister(object value, string name);
        private static readonly Dictionary<Type, AlternateEnumRegister> BANNED_ENUMS = new Dictionary<Type, AlternateEnumRegister>()
        {
            { typeof(Identifiable.Id),(x,y)=>IdentifiableRegistry.CreateIdentifiableId(x,y)},
            { typeof(Gadget.Id),(x,y)=>(object)GadgetRegistry.CreateGadgetId(x,y)},
            { typeof(PlayerState.Upgrade),(x,y)=>(object)PersonalUpgradeRegistry.CreatePersonalUpgrade(x,y)},
            {typeof(PediaDirector.Id),(x,y)=>(object)PediaRegistry.CreatePediaId(x,y) },
            {typeof(LandPlot.Id),(x,y)=>(object)LandPlotRegistry.CreateLandPlotId(x,y) }
        };

        internal static void RegisterAlternate(Type type, AlternateEnumRegister del)
        {
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
        /// Add a new value to the given <paramref name="enumType"/> 
        /// </summary>
        /// <param name="enumType">Enum to add the new value to</param>
        /// <param name="value">Value to add to the enum</param>
        /// <param name="name">The name of the new value</param>
        public static void AddEnumValue(Type enumType, object value, string name)
        {
            if (SRModLoader.GetModForAssembly(Assembly.GetCallingAssembly())!=null && BANNED_ENUMS.ContainsKey(enumType)) throw new Exception($"Patching {enumType} through EnumPatcher is not supported!");
            if (!enumType.IsEnum) throw new Exception($"{enumType} is not a valid Enum!");

            value = (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
            if (!patches.TryGetValue(enumType, out var patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            ClearEnumCache(enumType);

            patch.AddValue((ulong)value, name);
        }
        
        internal static void AddEnumValueWithAlternatives(Type enumType, object value, string name)
        {
            if (BANNED_ENUMS.TryGetValue(enumType, out var alternate)) alternate(value, name);
            else AddEnumValue(enumType,value,name);
        }
        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns>The first undefined enum value</returns>
        public static object GetFirstFreeValue(Type enumType)
        {
            var allValues = Enum.GetValues(enumType);
            for (int i = 0; i < allValues.Length - 1; i++)
            {
                if ((int)allValues.GetValue(i + 1) - (int)allValues.GetValue(i)>1)
                {
                    return Enum.ToObject(enumType, (int) allValues.GetValue(i) + 1);
                }
            }

            return Enum.ToObject(enumType,(int)allValues.GetValue(allValues.Length - 1) + 1);
        }

        public static void ClearEnumCache(Type enumType)
        {
            cache.SetValue(enumType, null);
        }
        
        internal static bool TryGetRawPatch(Type enumType, out EnumPatch patch)
        {
            return patches.TryGetValue(enumType, out patch);
        }

        public class EnumPatch 
        {
            private Dictionary<ulong, String> values = new Dictionary<ulong, String>();
            public void AddValue(ulong enumValue, string name)
            {
                if (values.ContainsKey(enumValue)) return;
                values.Add(enumValue, name);
            }

            public void GetArrays(out string[] names, out ulong[] values)
            {
                names = this.values.Values.ToArray();
                values = this.values.Keys.ToArray();
            }
        }
    }


}
    
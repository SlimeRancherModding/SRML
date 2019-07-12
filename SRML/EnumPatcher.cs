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

        private static PropertyInfo cache;
        private static FieldInfo global_cache_monitor;
        private static FieldInfo global_cache;
        
        private static Dictionary<Type, EnumPatch> patches = new Dictionary<Type, EnumPatch>();

        static EnumPatcher()
        {
            var t = AccessTools.TypeByName("System.MonoEnumInfo");
            cache = AccessTools.Property(t, "Cache");
            global_cache_monitor = AccessTools.Field(t,"global_cache_monitor");
            global_cache = AccessTools.Field(t, "global_cache");
        }

        public static object AddEnumValue(Type enumType, string name)
        {
            var newVal = GetFirstFreeValue(enumType);
            AddEnumValue(enumType, newVal, name);
            return newVal;
        }

        public static void AddEnumValue(Type enumType, object value, string name)
        {
            if (SRModLoader.GetModForAssembly(Assembly.GetCallingAssembly())!=null && BANNED_ENUMS.ContainsKey(enumType)) throw new Exception($"Patching {enumType} through EnumPatcher is not supported!");
            if (!enumType.IsEnum) throw new Exception($"{enumType} is not a valid Enum!");

            value = Enum.ToObject(enumType, value);
            if (!patches.TryGetValue(enumType, out var patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            ClearEnumCache(enumType);

            patch.AddValue(value, name);
        }

        internal static void AddEnumValueWithAlternatives(Type enumType, object value, string name)
        {
            if (BANNED_ENUMS.TryGetValue(enumType, out var alternate)) alternate(value, name);
            else AddEnumValue(enumType,value,name);
        }

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
            var cache = (Hashtable)EnumPatcher.cache.GetValue(null, null);
            var global_cache_monitor = EnumPatcher.global_cache_monitor.GetValue(null);
            var global_cache = (Hashtable)EnumPatcher.global_cache.GetValue(null);
            if (cache.Contains(enumType))
            {
                cache.Remove(enumType);
            }

            lock (global_cache_monitor)
            {
                if (global_cache.Contains(enumType))
                {
                    global_cache.Remove(enumType);
                }
            }
        }
        
        internal static bool TryGetRawPatch(Type enumType, out EnumPatch patch)
        {
            return patches.TryGetValue(enumType, out patch);
        }

        public class EnumPatch 
        {
            private Dictionary<object, String> values = new Dictionary<object, String>();
            public void AddValue(object enumValue, string name)
            {
                if (values.ContainsKey(enumValue)) return;
                values.Add(enumValue, name);
            }

            public void GetArrays(out string[] names, out object[] values)
            {
                names = this.values.Values.ToArray();
                values = this.values.Keys.ToArray();
            }
        }
    }


}
    
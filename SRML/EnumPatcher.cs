using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using Harmony;

namespace SRML
{
    public static class EnumPatcher
    {

        private static readonly HashSet<Type> BANNED_ENUMS = new HashSet<Type>()
        {
            typeof(Identifiable.Id),
            typeof(Gadget.Id)
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

        public static void AddEnumValue(Type enumType, object value, string name)
        {
            if (SRModLoader.GetModForAssembly(Assembly.GetCallingAssembly())!=null && BANNED_ENUMS.Contains(enumType)) throw new Exception($"Patching {enumType} through EnumPatcher is not supported!");
            if (!enumType.IsEnum) throw new Exception($"{enumType} is not a valid Enum!");
            EnumPatch patch;
            if (!patches.TryGetValue(enumType, out patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            ClearEnumCache(enumType);

            patch.AddValue(value, name);
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

            public void GetArrays(out string[] names, out int[] values)
            {
                names = this.values.Values.ToArray();
                values = this.values.Keys.Select((x) => (int)x).ToArray();
            }
        }
    }


}
    
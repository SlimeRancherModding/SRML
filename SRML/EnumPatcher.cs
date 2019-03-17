using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
namespace SRML
{
    public static class EnumPatcher
    {
        private static Dictionary<Type, IEnumPatcher> patches = new Dictionary<Type, IEnumPatcher>();

        public static void AddEnumValue(Type enumType, object value, string name)
        {
            IEnumPatcher patch;
            if (!patches.TryGetValue(enumType, out patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            if (!(patch is EnumPatch))
            {
                throw new NotImplementedException();
            }

            ClearEnumCache(enumType);

            (patch as EnumPatch).AddValue(value, name);
        }

        public static void ClearEnumCache(Type enumType)
        {
            var cache = (Hashtable)Type.GetType("System.MonoEnumInfo")
                .GetProperty("Cache", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
            var global_cache_monitor = Type.GetType("System.MonoEnumInfo")
                .GetField("global_cache_monitor", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var global_cache = (Hashtable)Type.GetType("System.MonoEnumInfo")
                .GetField("global_cache", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
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
        
        public static bool TryGetRawPatch(Type enumType, out IEnumPatcher patch)
        {
            return patches.TryGetValue(enumType, out patch);
        }

        public class EnumPatch : IEnumPatcher
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

    public interface IEnumPatcher
    {
        void GetArrays(out string[] names, out int[] values);
    }
}
    
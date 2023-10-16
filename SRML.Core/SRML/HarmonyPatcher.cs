using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SRML.Utils;

namespace SRML
{
    /// <summary>
    /// Utility class to help manage the main SRML harmony instance
    /// </summary>
    public static class HarmonyPatcher
    {
        private static Harmony _instance;

        internal static Dictionary<Assembly, Harmony> harmonyForAssembly = new Dictionary<Assembly, Harmony>();

        internal static Harmony Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Harmony("net.veesus.srml");

                return _instance;
            }
        }

        internal static void PatchAll()
        {
            GetInstance().PatchAll(Assembly.GetExecutingAssembly());
        }

        public static Harmony SetInstance(string name) => harmonyForAssembly[ReflectionUtils.GetRelevantAssembly()] = new Harmony(name);

        internal static Harmony SetInstanceForType(Type t, string name) => harmonyForAssembly[t.Assembly] = new Harmony(name);

        public static Harmony GetInstance() => harmonyForAssembly[ReflectionUtils.GetRelevantAssembly()];
    }
}

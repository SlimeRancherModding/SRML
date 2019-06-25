using System.Reflection;
using HarmonyLib;

namespace SRML
{
    public static class HarmonyPatcher
    {
        private static Harmony _instance;

        internal static Harmony Instance
        {
            get
            {
                if (_instance == null)
                {
                    InitializeInstance();
                }

                return _instance;
            }
        }

        static void InitializeInstance()
        {
            _instance = new Harmony("net.veesus.srml");
        }

        internal static void PatchAll()
        {
            Instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static Harmony SetInstance(string name)
        {
            var currentMod = SRMod.GetCurrentMod();
            currentMod.CreateHarmonyInstance(name);
            return currentMod.HarmonyInstance;
        }

        public static Harmony GetInstance()
        {
            return SRMod.GetCurrentMod().HarmonyInstance;
        }
    }
}

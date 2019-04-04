using System.Reflection;
using Harmony;

namespace SRML
{
    public static class HarmonyPatcher
    {
        private static HarmonyInstance _instance;

        internal static HarmonyInstance Instance
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
            _instance = HarmonyInstance.Create("net.veesus.srml");
        }

        internal static void PatchAll()
        {
            Instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static HarmonyInstance SetInstance(string name)
        {
            var currentMod = SRMod.GetCurrentMod();
            currentMod.CreateHarmonyInstance(name);
            return currentMod.HarmonyInstance;
        }

        public static HarmonyInstance GetInstance()
        {
            return SRMod.GetCurrentMod().HarmonyInstance;
        }
    }
}

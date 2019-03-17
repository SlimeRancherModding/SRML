using System.Reflection;
using Harmony;

namespace SRML
{
    internal static class HarmonyPatcher
    {
        private static HarmonyInstance _instance;

        public static HarmonyInstance Instance
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
            _instance = HarmonyInstance.Create("net.veesus.veesustools");
        }

        public static void PatchAll()
        {
            Instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}

using HarmonyLib;
using UnityEngine;

namespace SRML.Core
{
    internal static class Main
    {
        internal static Harmony HarmonyInstance;

        public static void Initialize()
        {
            Debug.Log("SRML has successfully invaded the game!");
            HarmonyInstance = new Harmony("net.veesus.srml");
        }
    }
}

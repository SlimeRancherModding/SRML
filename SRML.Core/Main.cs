using HarmonyLib;
using SRML.Core.ModLoader;
using SRML.Core.ModLoader.DataTypes;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.Core
{
    internal static class Main
    {
        internal static Harmony HarmonyInstance;
        internal static CoreLoader loader;

        public static void Initialize()
        {
            Debug.Log("SRML has successfully invaded the game!");
            HarmonyInstance = new Harmony("net.veesus.srml");

            loader = new CoreLoader();
        }
    }
}

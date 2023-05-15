using HarmonyLib;
using SRML.Core.ModLoader;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.Core.ModLoader.BuiltIn.Mod;
using SRML.Core.ModLoader.BuiltIn.ModLoader;
using UnityEngine;

namespace SRML.Core
{
    internal static class Main
    {
        public static Harmony HarmonyInstance;
        public static CoreLoader loader;

        public static void Initialize()
        {
            Debug.Log("SRML has successfully invaded the game!");
            HarmonyInstance = new Harmony("net.veesus.srml");

            loader = new CoreLoader();
            loader.RegisterModType(typeof(BasicMod), typeof(BasicLoadEntryPoint));
            loader.RegisterModLoader(typeof(BasicModLoader));
        }
    }
}

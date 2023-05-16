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

        public const string VERSION_STRING = "BETA-0.3.0";

        public static void Initialize()
        {
            Debug.Log("SRML has successfully invaded the game!");
            HarmonyInstance = new Harmony("net.veesus.srml");

            AccessTools.Method(typeof(FileLogger), "Init").Invoke(null, null);
            AccessTools.Method(typeof(Console.Console), "Init").Invoke(null, null);
            Console.Console.Instance.Log($"SRML v {VERSION_STRING}");

            loader = new CoreLoader();
            loader.RegisterModType(typeof(BasicMod), typeof(BasicLoadEntryPoint));
            loader.RegisterModLoader(typeof(BasicModLoader));
            // This DOES work, but then it breaks everything because the API is currently still built upon the old modloader.
            //loader.RegisterModLoader(typeof(LegacyModLoader));
        }
    }
}

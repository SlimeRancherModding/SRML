using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SRML.Utils;
using UnityEngine;

namespace SRML
{
    public static class FileSystem
    {
        public const string DataPath = "SlimeRancher_Data";
        public static string ModPath = "SRML/Mods";
        public static string LibPath = "SRML/Libs";

        /// <summary>
        /// Checks if a path exists and creates it if it doesn't
        /// </summary>
        /// <param name="path">Path to be checked</param>
        /// <returns>returns the original path argument</returns>
        public static string CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary>
        /// When called from a mod, gets the base path of that mod
        /// </summary>
        /// <returns>The base path of the current executing mod</returns>
        public static string GetMyPath()
        {
            var assembly = ReflectionUtils.GetRelevantAssembly();
            return SRModLoader.GetModForAssembly(assembly)?.Path ?? Path.GetDirectoryName(assembly.Location);
        }

        /// <summary>
        /// Gets a mods config path 
        /// </summary>
        /// <param name="mod">The mod whose config path is needed</param>
        /// <returns>The config path</returns>
        internal static string GetConfigPath(SRMod mod)
        {
            return CheckDirectory(Path.Combine(Path.Combine(Main.StorageProvider.SavePath(), "SRML/Config"), mod?.ModInfo.Id ?? "SRML"));
        }

        /// <summary>
        /// Gets the current mods config path
        /// </summary>
        /// <returns>The config path</returns>
        public static string GetMyConfigPath()
        {
            return GetConfigPath(SRMod.GetCurrentMod());    
        }
    }
}

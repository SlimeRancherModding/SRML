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
        public const String DataPath = "SlimeRancher_Data";
        public static String ModPath = "SRML/Mods";
        public static String LibPath = "SRML/Libs";
        public static string CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static String GetMyPath()
        {
            var assembly = ReflectionUtils.GetRelevantAssembly();
            return SRModLoader.GetModForAssembly(assembly)?.Path ?? Path.GetDirectoryName(assembly.Location);
        }

        public static String GetMyConfigPath()
        {
            var assembly = ReflectionUtils.GetRelevantAssembly();
            return CheckDirectory(Path.Combine(Path.Combine(Application.persistentDataPath, "SRML/Config"), SRModLoader.GetModForAssembly(assembly)?.ModInfo.Id ?? "SRML"));
        }
    }
}

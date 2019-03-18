using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRML
{
    public static class FileSystem
    {
        public const String DataPath = "SlimeRancher_Data";
        public static String ModPath = Path.Combine(DataPath, "Mods");
        public static void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static Assembly ourAssembly = Assembly.GetExecutingAssembly();
        public static String GetMyPath()
        {
            StackTrace trace = new StackTrace();

            var frames = trace.GetFrames();
            try // TODO: Clean this up, choose a better solution (check for non mod or srml dlls)
            {
                foreach (var frame in frames)
                {
                    var theirAssembly = frame.GetMethod().DeclaringType.Assembly;
                    if (theirAssembly != ourAssembly) return SRModLoader.GetModForAssembly(theirAssembly).Path;
                }
            }
            catch { }

            return Path.GetDirectoryName(ourAssembly.Location);
        }
    }
}

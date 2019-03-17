using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}

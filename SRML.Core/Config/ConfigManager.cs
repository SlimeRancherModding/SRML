using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRML.Config
{
    internal static class ConfigManager
    {
        public static Dictionary<IMod, List<ConfigFile>> modConfigs = new Dictionary<IMod, List<ConfigFile>>();

        public static void PopulateConfigs(IMod mod)
        {
            CoreLoader.Instance.ForceModContext(mod);

            foreach (var file in GetConfigs(mod.Entry.GetType().Module))
            {
                if (!modConfigs.ContainsKey(mod))
                    modConfigs[mod] = new List<ConfigFile>();

                modConfigs[mod].Add(file);
                file.TryLoadFromFile();
            }

            CoreLoader.Instance.ClearModContext();
        }

        public static IEnumerable<ConfigFile> GetConfigs(Module module)
        {
            foreach(var v in module.GetTypes())
            {
                var file = ConfigFile.GenerateConfig(v);
                if (file != null) yield return file;
            }
        }

        public static ConfigFile[] GetConfigs(this IMod mod)
        {
            if (modConfigs.ContainsKey(mod))
                return modConfigs[mod].ToArray();

            return new ConfigFile[0];
        }
    }
}

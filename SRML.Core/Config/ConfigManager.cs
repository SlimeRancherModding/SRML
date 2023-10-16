using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRML.Config
{
    internal static class ConfigManager
    {
        public static void PopulateConfigs(SRMod mod)
        {
            // TODO: Upgrade to new system
            //SRMod.ForceModContext(mod);

            foreach (var file in GetConfigs(mod.EntryType.Module))
            {
                mod.Configs.Add(file);
                file.TryLoadFromFile();
            }

            //SRMod.ClearModContext();
        }

        public static IEnumerable<ConfigFile> GetConfigs(Module module)
        {
            foreach(var v in module.GetTypes())
            {
                var file = ConfigFile.GenerateConfig(v);
                if (file != null) yield return file;
            }
        }
    }
}

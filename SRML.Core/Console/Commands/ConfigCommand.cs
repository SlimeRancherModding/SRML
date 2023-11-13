using SRML.Config;
using SRML.Core;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Console.Commands
{
    public class ConfigCommand : ConsoleCommand
    {
        public override string ID => "config";

        public override string Usage => "config [modid] [configname] [configsection] [configelement] [value]";

        public override string Description => "sets a config value";

        public override bool Execute(string[] args)
        {
            var srmlConfig = args[0] == "SRML";
            var mod = CoreLoader.Instance.GetMod(args[0]);
            var config = srmlConfig ? Main.Config : mod.GetConfigs().First(x => x.FileName == args[1]);
            var section = config.Sections.First(x => x.Name.ToLower() == args[srmlConfig ? 1 : 2].ToLower());
            var element = section.Elements.First(x => x.Options.Name.ToLower() == args[srmlConfig ? 2 : 3].ToLower());

            if (args.Length >= (srmlConfig ? 4 : 5))
                element.SetValue(element.Options.Parser.ParseObject(args[srmlConfig ? 3 : 4]));
            else
                Console.Instance.Log("Current Value: " + element.Options.Parser.EncodeObject(element.GetValue<object>()));
            
            if (!srmlConfig)
            {
                CoreLoader.Instance.ForceModContext(mod);
                config.SaveToFile();
                CoreLoader.Instance.ClearModContext();
            }
            else config.SaveToFile();
            
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0) 
                return CoreLoader.Instance.Mods.Where(x => ConfigManager.modConfigs.ContainsKey(x) && 
                    x.GetConfigs().Length > 0).Select(x => x.ModInfo.ID).Union(new[] { "SRML" }).ToList();

            if (args[0] == "SRML")
            {
                if (argIndex == 1) return Main.Config.Sections.Select(x => x.Name).ToList();

                var section = Main.Config.Sections.FirstOrDefault(x => x.Name.ToLower() == args[1].ToLower());

                if (argIndex == 2) return section?.Elements.Select(x => x.Options.Name).ToList();
            }
            else
            {
                var mod = CoreLoader.Instance.GetMod(args[0]);

                if (argIndex == 1) return mod?.GetConfigs().Select(x => x.FileName).ToList();

                var config = mod?.GetConfigs().FirstOrDefault(x => x.FileName.ToLower() == args[1].ToLower());

                if (argIndex == 2) return config?.Sections.Select(x => x.Name).ToList();

                var section = config?.Sections.FirstOrDefault(x => x.Name.ToLower() == args[2].ToLower());

                if (argIndex == 3) return section?.Elements.Select(x => x.Options.Name).ToList();
            }

            return base.GetAutoComplete(argIndex, args[argIndex]);
        }
    }
}

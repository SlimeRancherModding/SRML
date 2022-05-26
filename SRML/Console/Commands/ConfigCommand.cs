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
            var mod = SRModLoader.GetMod(args[0]);
            var config = mod.Configs.First(x => x.FileName.ToLower() == args[1].ToLower());
            var section = config.Sections.First(x => x.Name.ToLower() == args[2].ToLower());
            var element = section.Elements.First(x => x.Options.Name.ToLower() == args[3].ToLower());

            if (args.Length >= 5)
            {
                element.SetValue(element.Options.Parser.ParseObject(args[4]));
                //Debug.Log(element.GetValue<object>()+" "+element.GetType()+"!");
            }
            else
            {
                Console.Instance.Log("Current Value: " + element.Options.Parser.EncodeObject(element.GetValue<object>()));
            }
            SRMod.ForceModContext(mod);
            config.SaveToFile();
            SRMod.ClearModContext();
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0) return SRModLoader.GetMods().Where(x => x.Configs.Count > 0).Select(x => x.ModInfo.Id).ToList();

            var mod = SRModLoader.GetMod(args[0]);

            if (argIndex == 1) return mod?.Configs.Select(x => x.FileName).ToList();

            var config = mod?.Configs.FirstOrDefault(x => x.FileName.ToLower() == args[1].ToLower());

            if (argIndex == 2) return config?.Sections.Select(x => x.Name).ToList();

            var section = config?.Sections.FirstOrDefault(x => x.Name.ToLower() == args[2].ToLower());

            if (argIndex == 3) return section?.Elements.Select(x => x.Options.Name).ToList();

            return base.GetAutoComplete(argIndex, args[argIndex]);
        }
    }
}

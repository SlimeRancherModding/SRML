using SRML.SR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Mono.Cecil;
using SRML.Config.Attributes;
namespace SRML.Config
{
	internal static class ConfigManager
	{
		public static Dictionary<ConfigFile, SRMod> configToMod = new Dictionary<ConfigFile, SRMod>();
		//public static DateTime checkTime = DateTime.Now;
		public static void PopulateConfigs(SRMod mod)
		{
			SRMod.ForceModContext(mod);

			foreach (var file in GetConfigs(mod.EntryType.Module))
			{
				mod.Configs.Add(file);
				file.TryLoadFromFile();
			}

			SRMod.ClearModContext();
		}

		public static void InitLiveConfig()
		{
			foreach (SRMod mod in SRModLoader.GetMods())
			{
				foreach (ConfigFile cf in mod.Configs)
				{
					configToMod.Add(cf, mod);
				}
			}
			new GameObject("ConfigChecker", typeof(ConfigChecker)).DontDestroyOnLoad();
		}

		public static IEnumerable<ConfigFile> GetConfigs(Module module)
		{
			foreach (var v in module.GetTypes())
			{
				var file = ConfigFile.GenerateConfig(v);
				if (file != null) yield return file;
			}
		}
	}
}
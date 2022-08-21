using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace SRML.Config
{
	internal class ConfigChecker : SRBehaviour
	{
		public void Update()
		{
			foreach (KeyValuePair<ConfigFile, SRMod> kvp in ConfigManager.configToMod)
			{
				DateTime dt = File.GetLastWriteTime(FileSystem.GetConfigPath(kvp.Value) + "/" + kvp.Key.FileName + ".ini");
				if (dt > kvp.Key.checkTime)
				{
					Console.Console.Log("Config file updated for modid " + kvp.Value.ModInfo.Id);
					SRMod.ForceModContext(kvp.Value);
					kvp.Key.TryLoadFromFile();
					SRMod.ClearModContext();
					kvp.Key.checkTime = DateTime.Now;
				}
			}
		}
	}
}
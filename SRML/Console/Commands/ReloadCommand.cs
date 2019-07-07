using System;
using SRML.Console;

namespace SRML.Console.Commands
{
	/// <summary>
	/// A command to reload the mods
	/// </summary>
	public class ReloadCommand : ConsoleCommand
	{
		public override string ID { get; } = "reload";
		public override string Usage { get; } = "reload";
		public override string Description { get; } = "Reloads the mods";

		public override bool Execute(string[] args)
		{
			if (args != null)
			{
				Console.LogError($"The '<color=white>{ID}</color>' command takes no arguments");
				return false;
			}

			DateTime now = DateTime.Now;

			try
			{
				Console.ReloadMods();
				Console.LogSuccess($"Reloaded Successfully! (Took {(DateTime.Now - now).TotalMilliseconds} ms)");

				return true;
			}
			catch (Exception e)
			{
				Console.LogError("Reload Failed! Reason displayed below:");
				Console.LogError(e.Message + "\n" + e.StackTrace);
				return false;
			}
		}
	}
}

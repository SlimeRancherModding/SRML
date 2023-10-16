using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

namespace SRML.Console
{
	/// <summary>
	/// Binds user commands to the console command menu
	/// </summary>
	public static class ConsoleBinder
	{
		// BINDING FILE
		internal static string bindFile = Path.Combine(Main.StorageProvider.SavePath(), "SRML/userbinds.bindings");

		/// <summary>
		/// Reads all bindings
		/// </summary>
		internal static void ReadBinds()
		{
			if (!File.Exists(bindFile))
				return;

			foreach (string line in File.ReadAllLines(bindFile))
			{
				if (!line.Contains(":"))
					continue;

				string[] split = line.Split(':');
				Console.RegisterButton("user." + split[0], new ConsoleButton("U: " + split[1], split[2]));
			}
		}

		/// <summary>
		/// Registers a new bind
		/// </summary>
		/// <param name="id">The id of the button</param>
		/// <param name="text">The text to show on the button</param>
		/// <param name="command">The command to execute</param>
		public static void RegisterBind(string id, string text, string command)
		{
			if (id.Equals("all"))
			{
				Console.Instance.LogWarning($"Trying to register user defined button with id 'all' but 'all' is not a valid id!");
				return;
			}

			if (Console.RegisterButton("user." + id, new ConsoleButton("U: " + text, command)))
				File.AppendAllText(bindFile, $"{id}:{text}:{command}\n");
		}

		/// <summary>
		/// Removes a bind
		/// </summary>
		/// <param name="id">The id of the bind to remove</param>
		/// <returns>True if the bind got removed, false otherwise</returns>
		public static bool RemoveBind(string id)
		{
			if (!Console.cmdButtons.ContainsKey("user." + id))
				return false;
				
			Console.cmdButtons.Remove("user." + id);
			File.WriteAllText(bindFile, Regex.Replace(File.ReadAllText(bindFile), $@"{id}:.+\n", ""));
			return true;
		}

		/// <summary>
		/// Removes all binds
		/// </summary>
		public static void RemoveAll()
		{
			string[] lines = File.ReadAllLines(bindFile);
			foreach (string line in lines)
			{
				if (!line.Contains(":"))
					continue;

				RemoveBind(line.Substring(0, line.IndexOf(":")));
			}
		}

		/// <summary>
		/// Gets all the binds registered
		/// </summary>
		public static List<string> GetAllBinds()
		{
			List<string> result = new List<string>();

			foreach (string line in File.ReadAllLines(bindFile))
			{
				if (!line.Contains(":"))
					continue;

				result.Add(line.Substring(0, line.IndexOf(":")));
			}

			return result;
		}
	}
}

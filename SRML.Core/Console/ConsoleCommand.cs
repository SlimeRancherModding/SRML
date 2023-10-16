using System.Collections.Generic;

namespace SRML.Console
{
	/// <summary>
	/// Base class for all console commands
	/// </summary>
	public abstract class ConsoleCommand
	{

        internal SRMod belongingMod;
		/// <summary>
		/// The ID of this command (Always lowercase)
		/// </summary>
		public abstract string ID { get; }

		/// <summary>
		/// The usage info of this command
		/// </summary>
		public abstract string Usage { get; }

		/// <summary>
		/// The description of this command
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// The extended description of this command, with a description of each argument to
		/// display when you use help command on this command (Multiline is supported)
		/// </summary>
		public virtual string ExtendedDescription { get; } = null;

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
		/// <returns>True if it executed, false otherwise</returns>
		public abstract bool Execute(string[] args);

		/// <summary>
		/// Gets the auto complete list (word filter is done by the system)
		/// </summary>
		/// <param name="argIndex">The index of the argument in the command string</param>
		/// <param name="argText">The text of the argument</param>
		/// <returns>The list of auto complete options</returns>
		public virtual List<string> GetAutoComplete(int argIndex, string argText) { return null; }

		/// <summary>
		/// Gets the auto complete list (word filter is done by the system)
		/// </summary>
		/// <param name="argIndex">The index of the argument in the command string</param>
		/// <param name="args">A list of inputted arguments</param>
		/// <returns>The list of auto complete options</returns>
		public virtual List<string> GetAutoComplete(int argIndex, string[] args) { return null; }

		/// <summary>
		/// The arguments are out of bounds (either too many or too little)
		/// </summary>
		/// <param name="min">Amount of arguments</param>
		/// <param name="min">Minimun amount</param>
		/// <param name="max">Maximun amount</param>
		/// <returns>True if arguments are out of bounds, false otherwise</returns>
		protected virtual bool ArgsOutOfBounds(int argCount, int min = -1, int max = -1)
		{
			if (argCount < min && min > -1)
			{
				Console.Instance.LogError($"The '{ID}' command got less arguments then expected (Had: {argCount}; Min: {min})");
				return true;
			}

			if (argCount > max && max > -1)
			{
				Console.Instance.LogError($"The '{ID}' command got more arguments then expected (Had: {argCount}; Max: {max})");
				return true;
			}

			return false;
		}
	}
}

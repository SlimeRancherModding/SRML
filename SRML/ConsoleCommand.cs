namespace SRML.ConsoleSystem
{
	/// <summary>
	/// Base class for all console commands
	/// </summary>
	public abstract class ConsoleCommand
	{
		/// <summary>
		/// The ID of this command
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
		/// Executes the command
		/// </summary>
		/// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
		/// <returns>True if it executed, false otherwise</returns>
		public abstract bool Execute(string[] args);

		/// <summary>
		/// The arguments are invalid (either too many or too little)
		/// </summary>
		/// <param name="min">Amount of arguments</param>
		/// <param name="min">Minimun amount</param>
		/// <param name="max">Maximun amount</param>
		/// <returns>True if arguments are out of bounds, false otherwise</returns>
		protected virtual bool ArgsOutOfBounds(int argCount, int min = 0, int max = 0)
		{
			if (argCount < min)
			{
				Console.LogError($"The '{ID}' command got less arguments then expected (Had: {argCount}; Min: {min})");
				return true;
			}

			if (argCount > max)
			{
				Console.LogError($"The '{ID}' command got more arguments then expected (Had: {argCount}; Max: {max})");
				return true;
			}

			return false;
		}
	}
}

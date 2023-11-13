using System;

namespace SRML.Core.ModLoader
{
    public class MissingModInfoException : Exception
    {
        public MissingModInfoException() : base("Could not find mod info for mod") { }
        public MissingModInfoException(string message) : base($"Could not find mod info for mod: {message}") { }
    }
}

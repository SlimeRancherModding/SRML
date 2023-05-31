using SRML.Core;
using SRML.Core.ModLoader;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;

namespace SRML.API
{
    public class EntryPoint : CoreModEntryPoint
    {
        public EntryPoint(IModInfo info) : base(info)
        {
        }

        public override void Initialize()
        {
            ConsoleInstance.Log("SRML API initializing.");
        }
    }
}

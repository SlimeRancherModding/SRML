using SRML.Console.Commands;
using SRML.Core.ModLoader;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;

namespace SRML.Addons
{
    public class EntryPoint : CoreModEntryPoint
    {
        public EntryPoint(IModInfo info) : base(info)
        {
        }

        public override void Initialize()
        {
            Console.Console.RegisterCommand(new FastForwardCommand());
            Console.Console.RegisterCommand(new GiveCommand());
            Console.Console.RegisterCommand(new KillAllCommand());
            Console.Console.RegisterCommand(new KillCommand());
            Console.Console.RegisterCommand(new NoclipCommand());
            Console.Console.RegisterCommand(new SpawnCommand());
        }
    }
}

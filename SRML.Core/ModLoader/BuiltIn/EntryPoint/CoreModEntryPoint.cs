using SRML.Core.API;
using System.Linq;

namespace SRML.Core.ModLoader.BuiltIn.EntryPoint
{
    public abstract class CoreModEntryPoint : Core.ModLoader.EntryPoint
    {
        protected CoreModEntryPoint(IModInfo info) : base(info)
        {
        }

        public IMod[] GetMyMods() => CoreLoader.Main.Mods.Where(x => x.ModInfo.Dependencies.dependencies.Keys.Contains(Info.Id)).ToArray();
    }
}

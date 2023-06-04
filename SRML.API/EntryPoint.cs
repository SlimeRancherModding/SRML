using SRML.API.Identifiable;
using SRML.API.Identifiable.Slime;
using SRML.Core.API;
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
            CoreAPI.Instance.RegisterRegistry(new IdentifiablePrefabRegistry());
            CoreAPI.Instance.RegisterRegistry(new IdentifiableRegistry());
            CoreAPI.Instance.RegisterRegistry(new LiquidRegistry());
            CoreAPI.Instance.RegisterRegistry(new VacItemRegistry());

            CoreAPI.Instance.RegisterRegistry(new GordoRegistry());
            CoreAPI.Instance.RegisterRegistry(new ToyRegistry());
        }
    }
}

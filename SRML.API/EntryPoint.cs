using SRML.API.Gadget;
using SRML.API.Identifiable;
using SRML.API.Identifiable.Slime;
using SRML.API.Player;
using SRML.API.World;
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
            HarmonyInstance.PatchAll();

            CoreAPI.Instance.RegisterRegistry(new DroneTargetRegistry());
            CoreAPI.Instance.RegisterRegistry(new RefineryRegistry());
            CoreAPI.Instance.RegisterRegistry(new SnareRegistry());

            CoreAPI.Instance.RegisterRegistry(new FashionOffsetRegistry());
            CoreAPI.Instance.RegisterRegistry(new FashionSlotRegistry());
            CoreAPI.Instance.RegisterRegistry(new FoodGroupRegistry());
            CoreAPI.Instance.RegisterRegistry(new GordoRegistry());
            CoreAPI.Instance.RegisterRegistry(new ToyRegistry());

            CoreAPI.Instance.RegisterRegistry(new IdentifiablePrefabRegistry());
            CoreAPI.Instance.RegisterRegistry(new IdentifiableRegistry());
            CoreAPI.Instance.RegisterRegistry(new LiquidRegistry());
            CoreAPI.Instance.RegisterRegistry(new SpawnResourcePrefabRegistry());
            CoreAPI.Instance.RegisterRegistry(new VacItemRegistry());

            CoreAPI.Instance.RegisterRegistry(new PlayerAmmoRegistry());
            CoreAPI.Instance.RegisterRegistry(new TargetingRegistry());

            CoreAPI.Instance.RegisterRegistry(new SiloAmmoRegistry());
        }
    }
}

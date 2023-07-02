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

            CoreAPI api = CoreAPI.Instance;
            api.RegisterRegistry(new BlueprintRegistry());
            api.RegisterRegistry(new DroneTargetRegistry());
            api.RegisterRegistry(new GadgetRegistry());
            api.RegisterRegistry(new RefineryRegistry());
            api.RegisterRegistry(new SnareRegistry());

            api.RegisterRegistry(new FashionOffsetRegistry());
            api.RegisterRegistry(new FashionSlotRegistry());
            api.RegisterRegistry(new FoodGroupRegistry());
            api.RegisterRegistry(new GordoRegistry());
            api.RegisterRegistry(new ToyRegistry());

            api.RegisterRegistry(new IdentifiablePrefabRegistry());
            api.RegisterRegistry(new IdentifiableRegistry());
            api.RegisterRegistry(new LiquidRegistry());
            api.RegisterRegistry(new SpawnResourcePrefabRegistry());
            api.RegisterRegistry(new VacItemRegistry());

            api.RegisterRegistry(new AchievementsRegistry());
            api.RegisterRegistry(new AchievementTierRegistry());
            api.RegisterRegistry(new PlayerActionRegistry());
            api.RegisterRegistry(new PlayerAmmoRegistry());
            api.RegisterRegistry(new TargetingRegistry());

            api.RegisterRegistry(new SiloAmmoRegistry());
        }
    }
}

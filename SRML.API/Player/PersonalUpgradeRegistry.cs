using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRML.Core.API.BuiltIn;
using SRML.Core.ModLoader;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Player
{
    [HarmonyPatch]
    public class PersonalUpgradeRegistry : EnumRegistry<PersonalUpgradeRegistry, PlayerState.Upgrade>
    {
        protected Dictionary<PlayerState.Upgrade, (PlayerState.Upgrade?, PlayerState.UnlockCondition, float)> basicUpgradeLockers =
            new Dictionary<PlayerState.Upgrade, (PlayerState.Upgrade?, PlayerState.UnlockCondition, float)>();
        protected Dictionary<PlayerState.Upgrade, PlayerState.UpgradeLocker> upgradeLockers = new Dictionary<PlayerState.Upgrade, PlayerState.UpgradeLocker>();

        protected Dictionary<PlayerState.Upgrade, ApplyUpgrade> upgradeApplicators = new Dictionary<PlayerState.Upgrade, ApplyUpgrade>();
        protected List<PlayerState.Upgrade> defaultAvails = new List<PlayerState.Upgrade>();
        protected Dictionary<string, List<UpgradeDefinition>> moddedDefinitions = new Dictionary<string, List<UpgradeDefinition>>();

        public delegate void ApplyUpgrade(PlayerModel model, bool isFirstTime);
        public delegate void BasicUpgradeLockerRegisterEvent(PlayerState.Upgrade upgrade, 
            (PlayerState.Upgrade?, PlayerState.UnlockCondition, float) locker);
        public delegate void UpgradeLockerRegisterEvent(PlayerState.Upgrade upgrade, PlayerState.UpgradeLocker locker);
        public delegate void ApplicationDelegateRegisterEvent(PlayerState.Upgrade upgrade, ApplyUpgrade del);
        public delegate void UpgradeRegisterEvent(PlayerState.Upgrade upgrade);
        public delegate void DefinitionRegisterEvent(UpgradeDefinition definition);

        public BasicUpgradeLockerRegisterEvent OnRegisterBasicUpgradeLocker;
        public UpgradeLockerRegisterEvent OnRegisterUpgradeLocker;
        public ApplicationDelegateRegisterEvent OnRegisterApplicationDelegate;
        public UpgradeRegisterEvent OnRegisterDefaultAvailableUpgrade;
        public DefinitionRegisterEvent OnRegisterUpgradeDefinition;

        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterDefinitions(LookupDirector __instance) => Instance.RegisterIntoLookup(__instance);

        [HarmonyPatch(typeof(PlayerModel), "ApplyUpgrade")]
        [HarmonyPrefix]
        internal static bool RegisterApplicators(PlayerModel __instance, PlayerState.Upgrade upgrade, bool isFirstTime) =>
            Instance.RunUpgradeApplicator(__instance, upgrade, isFirstTime);

        [HarmonyPatch(typeof(PlayerState), "InitUpgradeLocks")]
        [HarmonyPostfix]
        internal static void RegisterLockers(PlayerState __instance, PlayerModel model) => 
            Instance.RegisterLockersIntoPlayerState(__instance, model);

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var upgrade in moddedDefinitions.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.upgradeDefinitionDict[upgrade.upgrade] = upgrade;
                lookupDirector.upgradeDefinitions.items.Add(upgrade);
            }
        }
        public virtual bool RunUpgradeApplicator(PlayerModel playerModel, PlayerState.Upgrade upgrade, bool isFirstTime)
        {
            var upgradeApplierDict = upgradeApplicators;
            if (!upgradeApplierDict.ContainsKey(upgrade))
                return true;

            upgradeApplierDict[upgrade](playerModel, isFirstTime);
            return false;
        }
        public virtual void RegisterLockersIntoPlayerState(PlayerState state, PlayerModel model)
        {
            if (Levels.isSpecial())
                return;

            foreach (PlayerState.Upgrade upgrade in defaultAvails)
                model.availUpgrades.Add(upgrade);
            foreach (var locker in basicUpgradeLockers)
                model.upgradeLocks[locker.Key] = state.CreateBasicLock(locker.Value.Item1, locker.Value.Item2, locker.Value.Item3);
            foreach (var locker in upgradeLockers)
                model.upgradeLocks[locker.Key] = locker.Value;
        }

        public virtual void RegisterUpgradeDefinition(UpgradeDefinition definition)
        {
            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedDefinitions.ContainsKey(executingId))
                moddedDefinitions[executingId] = new List<UpgradeDefinition>();

            moddedDefinitions[executingId].Add(definition);
            OnRegisterUpgradeDefinition?.Invoke(definition);
        }
        public virtual void RegisterBasicUpgradeLocker(PlayerState.Upgrade upgrade, PlayerState.Upgrade? waitForUpgrade,
            PlayerState.UnlockCondition extraCondition, float delayHrs)
        {
            basicUpgradeLockers[upgrade] = (waitForUpgrade, extraCondition, delayHrs);
            OnRegisterBasicUpgradeLocker?.Invoke(upgrade, (waitForUpgrade, extraCondition, delayHrs));
        }
        public virtual void RegisterUpgradeLocker(PlayerState.Upgrade upgrade, PlayerState.UpgradeLocker locker)
        {
            upgradeLockers[upgrade] = locker;
            OnRegisterUpgradeLocker?.Invoke(upgrade, locker);
        }
        public virtual void RegisterUpgradeApplicationDelegate(PlayerState.Upgrade upgrade, ApplyUpgrade del)
        {
            upgradeApplicators[upgrade] = del;
            OnRegisterApplicationDelegate?.Invoke(upgrade, del);
        }
        public virtual void RegisterDefaultAvailableUpgrade(PlayerState.Upgrade upgrade)
        {
            defaultAvails.Add(upgrade);
            OnRegisterDefaultAvailableUpgrade?.Invoke(upgrade);
        }

        public override void Initialize()
        {
        }
    }
}

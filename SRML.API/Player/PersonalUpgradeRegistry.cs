using MonomiPark.SlimeRancher.DataModel;
using SRML.Core.API.BuiltIn;
using System.Collections.Generic;

namespace SRML.API.Player
{
    public class PersonalUpgradeRegistry : EnumRegistry<PersonalUpgradeRegistry, PlayerState.Upgrade>
    {
        public delegate void ApplyUpgrade(PlayerModel model, bool isFirstTime);

        internal Dictionary<PlayerState.Upgrade, (PlayerState.Upgrade?, PlayerState.UnlockCondition, float)> basicUpgradeLockers =
            new Dictionary<PlayerState.Upgrade, (PlayerState.Upgrade?, PlayerState.UnlockCondition, float)>();
        internal Dictionary<PlayerState.Upgrade, PlayerState.UpgradeLocker> upgradeLockers = new Dictionary<PlayerState.Upgrade, PlayerState.UpgradeLocker>();

        internal Dictionary<PlayerState.Upgrade, ApplyUpgrade> upgradeApplicators = new Dictionary<PlayerState.Upgrade, ApplyUpgrade>();
        internal List<PlayerState.Upgrade> defaultAvails = new List<PlayerState.Upgrade>();

        public override void Process(PlayerState.Upgrade toProcess)
        {
        }

        public void CreateUpgradeLocker(PlayerState.Upgrade upgrade, PlayerState.Upgrade? waitForUpgrade, 
            PlayerState.UnlockCondition extraCondition, float delayHrs) => basicUpgradeLockers[upgrade] = (waitForUpgrade, extraCondition, delayHrs);

        public void CreateUpgradeLocker(PlayerState.Upgrade upgrade, PlayerState.UpgradeLocker locker) => upgradeLockers[upgrade] = locker;

        public void RegisterUpgradeUnlockDelegate(PlayerState.Upgrade upgrade, ApplyUpgrade del) =>
            upgradeApplicators[upgrade] = del;

        public void RegisterDefaultAvailableUpgrade(PlayerState.Upgrade upgrade) => defaultAvails.Add(upgrade);
    }
}

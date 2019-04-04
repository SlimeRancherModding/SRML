using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SRML.SR
{
    public static class PersonalUpgradeRegistry
    {
        public delegate void ApplyUpgradeDelegate(PlayerModel player, bool isFirstTime);

        internal static ModdedIDRegistry<PlayerState.Upgrade> moddedUpgrades = new ModdedIDRegistry<PlayerState.Upgrade>();

        internal static Dictionary<PlayerState.Upgrade,ApplyUpgradeDelegate> upgradeCallbacks = new Dictionary<PlayerState.Upgrade, ApplyUpgradeDelegate>();

        static PersonalUpgradeRegistry()
        {
            SaveRegistry.RegisterIDRegistry(moddedUpgrades);

        }

        public static PlayerState.Upgrade CreatePersonalUpgrade(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register gadgets outside of the PreLoad step");
            return moddedUpgrades.RegisterValueWithEnum((PlayerState.Upgrade) value, name);
        }

        public static bool IsModdedUpgrade(PlayerState.Upgrade upgrade)
        {
            return moddedUpgrades.ContainsKey(upgrade);
        }

        public static void RegisterUpgradeCallback(PlayerState.Upgrade upgrade, ApplyUpgradeDelegate callback)
        {
            if (upgradeCallbacks.ContainsKey(upgrade))
            {
                upgradeCallbacks[upgrade] += callback;
            }
            else
            {
                upgradeCallbacks[upgrade] = callback;
            }
        }

    }
}

using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem;
using SRML.SR.Translation;
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

        public delegate PlayerState.UpgradeLocker CreateUpgradeLockerDelegate(PlayerState state);

        internal static IDRegistry<PlayerState.Upgrade> moddedUpgrades = new IDRegistry<PlayerState.Upgrade>();

        internal static Dictionary<PlayerState.Upgrade,ApplyUpgradeDelegate> upgradeCallbacks = new Dictionary<PlayerState.Upgrade, ApplyUpgradeDelegate>();

        internal static Dictionary<PlayerState.Upgrade, CreateUpgradeLockerDelegate> moddedLockers =
            new Dictionary<PlayerState.Upgrade, CreateUpgradeLockerDelegate>();

        static PersonalUpgradeRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedUpgrades);

        }

        public static PlayerState.Upgrade CreatePersonalUpgrade(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register upgrades outside of the PreLoad step");
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



        public static void RegisterUpgradeLock(PlayerState.Upgrade upgrade, CreateUpgradeLockerDelegate del)
        {
            moddedLockers.Add(upgrade,del);
        }

        public static void RegisterDefaultUpgrade(PlayerState.Upgrade upgrade)
        {
            RegisterUpgradeLock(upgrade,null);
        }

        
    }
}

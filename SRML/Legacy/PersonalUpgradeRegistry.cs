using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem;
using SRML.SR.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class PersonalUpgradeRegistry
    {
        public delegate void ApplyUpgradeDelegate(PlayerModel player, bool isFirstTime);
        public delegate PlayerState.UpgradeLocker CreateUpgradeLockerDelegate(PlayerState state);

        internal static IDRegistry<PlayerState.Upgrade> moddedUpgrades = new IDRegistry<PlayerState.Upgrade>();
        internal static Dictionary<PlayerState.Upgrade, ApplyUpgradeDelegate> upgradeCallbacks = new Dictionary<PlayerState.Upgrade, ApplyUpgradeDelegate>();
        internal static Dictionary<PlayerState.Upgrade, CreateUpgradeLockerDelegate> moddedLockers =
            new Dictionary<PlayerState.Upgrade, CreateUpgradeLockerDelegate>();

        static PersonalUpgradeRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedUpgrades);
        }

        /// <summary>
        /// Creates a <see cref="PlayerState.Upgrade"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="PlayerState.Upgrade"/>.</param>
        /// <param name="name">The name of the <see cref="PlayerState.Upgrade"/>.</param>
        /// <returns>The created <see cref="PlayerState.Upgrade"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static PlayerState.Upgrade CreatePersonalUpgrade(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register upgrades outside of the PreLoad step");
            return moddedUpgrades.RegisterValueWithEnum((PlayerState.Upgrade) value, name);
        }

        /// <summary>
        /// Check if a <see cref="PlayerState.Upgrade"/> belongs to a modded upgrade.
        /// </summary>
        /// <param name="id">The <see cref="PlayerState.Upgrade"/> to check.</param>
        /// <returns>True if <see cref="PlayerState.Upgrade"/> belongs to a modded upgrade, otherwise false.</returns>
        public static bool IsModdedUpgrade(PlayerState.Upgrade upgrade) => moddedUpgrades.ContainsKey(upgrade);

        /// <summary>
        /// Registers a callback to be called when an upgrade is bought.
        /// </summary>
        /// <param name="upgrade">The <see cref="PlayerState.Upgrade"/> that calls the callback.</param>
        /// <param name="callback">The callback to be called.</param>
        public static void RegisterUpgradeCallback(PlayerState.Upgrade upgrade, ApplyUpgradeDelegate callback)
        {
            if (upgradeCallbacks.ContainsKey(upgrade))
                upgradeCallbacks[upgrade] += callback;
            else
                upgradeCallbacks[upgrade] = callback;
        }

        /// <summary>
        /// Registers an upgrade locker.
        /// </summary>
        /// <param name="upgrade">The <see cref="PlayerState.Upgrade"/> to be locked.</param>
        /// <param name="del">The locker locking the <see cref="PlayerState.Upgrade"/>.</param>
        public static void RegisterUpgradeLock(PlayerState.Upgrade upgrade, CreateUpgradeLockerDelegate del) => moddedLockers.Add(upgrade, del);

        /// <summary>
        /// Registers an upgrade that's automatically unlocked.
        /// </summary>
        /// <param name="upgrade">The <see cref="PlayerState.Upgrade"/> to be unlocked.</param>
        public static void RegisterDefaultUpgrade(PlayerState.Upgrade upgrade) => RegisterUpgradeLock(upgrade, null);
    }
}

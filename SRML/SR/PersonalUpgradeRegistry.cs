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

        internal static Dictionary<PlayerState.Upgrade, SRMod> moddedUpgrades = new Dictionary<PlayerState.Upgrade, SRMod>();

        internal static Dictionary<PlayerState.Upgrade,ApplyUpgradeDelegate> upgradeCallbacks = new Dictionary<PlayerState.Upgrade, ApplyUpgradeDelegate>();

        static PersonalUpgradeRegistry()
        {
            SaveRegistry.RegisterIDRegistry(new ModdedIDRegistry((id => moddedUpgrades[(PlayerState.Upgrade)id]), () => typeof(PlayerState.Upgrade), (x) => IsModdedUpgrade((PlayerState.Upgrade)x), (mod) => moddedUpgrades.Where((x) => x.Value == mod).Select((x) => x.Key).ToList()));

        }

        public static PlayerState.Upgrade CreatePersonalUpgrade(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register gadgets outside of the PreLoad step");
            var id = (PlayerState.Upgrade)value;
            if (moddedUpgrades.ContainsKey(id))
                throw new Exception(
                    $"Upgrade {value} is already registered to {moddedUpgrades[id].ModInfo.Id}");
            var sr = SRMod.GetCurrentMod();
            if (sr != null) moddedUpgrades[id] = sr;
            EnumPatcher.AddEnumValue(typeof(PlayerState.Upgrade), id, name);
            return id;
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

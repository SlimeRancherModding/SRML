using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class PersonalUpgradeRegistry
    {
        internal static HashSet<PlayerState.Upgrade> purchasableUpgradesToPatch = new HashSet<PlayerState.Upgrade>();

        public static void RegisterPurchasableUpgrade(PlayerState.Upgrade upgrade)
        {
            purchasableUpgradesToPatch.Add(upgrade);
        }
    }
}

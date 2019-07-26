using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class ToyRegistry
    {
        public static void RegisterBasePurchasableToy(Identifiable.Id id)
        {
            ToyDirector.BASE_TOYS.Add(id);
        }

        public static void RegisterUpgradedPurchasableToy(Identifiable.Id id)
        {
            ToyDirector.UPGRADED_TOYS.Add(id);
        }
    }
}

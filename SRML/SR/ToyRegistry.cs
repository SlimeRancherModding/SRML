using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class ToyRegistry
    {
        /// <summary>
        /// Registers a toy into the first group of toys.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> belonging to the toy.</param>
        public static void RegisterBasePurchasableToy(Identifiable.Id id)
        {
            ToyDirector.BASE_TOYS.Add(id);
        }

        /// <summary>
        /// Registers a toy into the second group of toys.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> belonging to the toy.</param>
        public static void RegisterUpgradedPurchasableToy(Identifiable.Id id) => ToyDirector.UPGRADED_TOYS.Add(id);
    }
}

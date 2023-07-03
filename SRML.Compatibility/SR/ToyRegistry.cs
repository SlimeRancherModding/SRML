using System;

namespace SRML.SR
{
    [Obsolete]
    public static class ToyRegistry
    {
        /// <summary>
        /// Registers a toy into the first group of toys.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> belonging to the toy.</param>
        public static void RegisterBasePurchasableToy(Identifiable.Id id) => API.Identifiable.Slime.ToyRegistry.Instance.MarkToyAsBase(id);

        /// <summary>
        /// Registers a toy into the second group of toys.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> belonging to the toy.</param>
        public static void RegisterUpgradedPurchasableToy(Identifiable.Id id) => API.Identifiable.Slime.ToyRegistry.Instance.MarkToyAsUpgraded(id);
    }
}

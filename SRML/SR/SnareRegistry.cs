using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.SR
{
    public static class SnareRegistry
    {
        internal static readonly HashSet<Identifiable.Id> snareables = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        internal static readonly HashSet<Func<Identifiable.Id, bool>> baitFuncs = new HashSet<Func<Identifiable.Id, bool>>();
        internal static readonly HashSet<Identifiable.Id> pinkLike = new HashSet<Identifiable.Id>(Identifiable.idComparer);

        /// <summary>
        /// Allows an <see cref="Identifiable.Id"/> to go onto a gordo snare.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to register.</param>
        public static void RegisterAsSnareable(this Identifiable.Id id)
        {
            if (!snareables.Contains(id))
                snareables.Add(id);
        }

        /// <summary>
        /// Registers a gordo that has a chance to be snared anywhere with anything, like the Pink Gordo.
        /// </summary>
        /// <param name="gordoId">The id of the gordo being registered.</param>
        public static void RegisterFallbackGordo(Identifiable.Id gordoId)
        {
            if (!pinkLike.Contains(gordoId))
                pinkLike.Add(gordoId);
        }

        /// <summary>
        /// Registers a delegate that handles multiple ids/complex behaviour for bait ids.
        /// </summary>
        /// <param name="predicate">The id of the gordo being registered.</param>
        public static void RegisterCollectiveBaitMethod(Func<Identifiable.Id, bool> predicate)
        {
            baitFuncs.Add(predicate);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.SR
{
    public static class SnareRegistry
    {
        internal static HashSet<Identifiable.Id> snareables = new HashSet<Identifiable.Id>(Identifiable.idComparer);

        /// <summary>
        /// Allows an <see cref="Identifiable.Id"/> to go onto a gordo snare.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to register.</param>
        public static void RegisterAsSnareable(this Identifiable.Id id)
        {
            if (!snareables.Contains(id)) 
                snareables.Add(id);
        }
    }
}

using System;

namespace SRML.SR
{
    [Obsolete]
    public static class SnareRegistry
    {
        /// <summary>
        /// Allows an <see cref="Identifiable.Id"/> to go onto a gordo snare.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to register.</param>
        public static void RegisterAsSnareable(this Identifiable.Id id) =>
            API.Gadget.SnareRegistry.Instance.Register(id);
    }
}

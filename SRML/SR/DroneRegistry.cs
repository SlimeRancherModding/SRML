using SRML.API.Gadget;
using System;

namespace SRML.SR
{
    [Obsolete]
    public static class DroneRegistry
    {
        /// <summary>
        /// Register an <see cref="Identifiable.Id"/> as a drone target.
        /// </summary>
        /// <param name="id"></param>
        public static void RegisterBasicTarget(Identifiable.Id id) => DroneTargetRegistry.Instance.Register(id);
    }
}

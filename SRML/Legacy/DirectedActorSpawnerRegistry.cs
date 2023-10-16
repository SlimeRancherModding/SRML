using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    // associated patch is DirectedActorSpawnerStartPatch
    public static class DirectedActorSpawnerRegistry
    {
        public delegate void DirectedActorSpawnerPatcher(DirectedActorSpawner spawner);
        internal static Dictionary<Predicate<DirectedActorSpawner>, DirectedActorSpawnerPatcher> spawnerFixers = new Dictionary<Predicate<DirectedActorSpawner>, DirectedActorSpawnerPatcher>();
        public static void RegisterDirectedActorSpawnerPatcher(Predicate<DirectedActorSpawner> pred, DirectedActorSpawnerPatcher fixer)
        {
            spawnerFixers.Add(pred, fixer);
        }
    }
}

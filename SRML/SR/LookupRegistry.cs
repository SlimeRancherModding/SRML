using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class LookupRegistry
    {
        internal static HashSet<GameObject> objectsToPatch = new HashSet<GameObject>();
        internal static HashSet<LookupDirector.VacEntry> vacEntriesToPatch = new HashSet<LookupDirector.VacEntry>();
        public static void RegisterIdentifiablePrefab(GameObject b)
        {
            objectsToPatch.Add(b);
        }

        public static void RegisterIdentifiablePrefab(Identifiable b)
        {
            RegisterIdentifiablePrefab(b.gameObject);
        }

        public static void RegisterVacEntry(LookupDirector.VacEntry entry)
        {
            vacEntriesToPatch.Add(entry);
        }

        public static void RegisterVacEntry(Identifiable.Id id, Color color, Sprite icon)
        {
            RegisterVacEntry(new LookupDirector.VacEntry(){id=id,color=color,icon=icon});
        }
    }
}

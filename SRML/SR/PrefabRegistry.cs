using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class PrefabRegistry
    {
        internal static HashSet<GameObject> objectsToPatch = new HashSet<GameObject>();
        public static void RegisterIdentifiablePrefab(GameObject b)
        {
            objectsToPatch.Add(b);
        }

        public static void RegisterIdentifiablePrefab(Identifiable b)
        {
            RegisterIdentifiablePrefab(b.gameObject);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static SRML.SRModLoader;

namespace SRML.SR
{
    public static class LookupRegistry
    {
        internal static HashSet<GameObject> objectsToPatch = new HashSet<GameObject>();
        internal static HashSet<LookupDirector.VacEntry> vacEntriesToPatch = new HashSet<LookupDirector.VacEntry>();
        public static void RegisterIdentifiablePrefab(GameObject b)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    objectsToPatch.Add(b);
                    break;
                default:
                    GameContext.Instance.LookupDirector.identifiablePrefabs.Add(b);
                    GameContext.Instance.LookupDirector.identifiablePrefabDict[Identifiable.GetId(b)] = b;
                    break;
            }
        }

        public static void RegisterIdentifiablePrefab(Identifiable b)
        {
            RegisterIdentifiablePrefab(b.gameObject);
        }

        public static void RegisterVacEntry(LookupDirector.VacEntry entry)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    vacEntriesToPatch.Add(entry);
                    break;
                default:
                    GameContext.Instance.LookupDirector.vacEntries.Add(entry);
                    GameContext.Instance.LookupDirector.vacEntryDict[entry.id] = entry;
                    break;
            }
        }

        public static void RegisterVacEntry(Identifiable.Id id, Color color, Sprite icon)
        {
            RegisterVacEntry(new LookupDirector.VacEntry(){id=id,color=color,icon=icon});
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using static SRML.SRModLoader;

namespace SRML.SR
{
    public static class LookupRegistry
    {
        internal static HashSet<GameObject> objectsToPatch = new HashSet<GameObject>();
        internal static HashSet<LookupDirector.VacEntry> vacEntriesToPatch = new HashSet<LookupDirector.VacEntry>();
        internal static HashSet<LookupDirector.GadgetEntry> gadgetEntriesToPatch = new HashSet<LookupDirector.GadgetEntry>();

        internal static HashSet<LookupDirector.UpgradeEntry> upgradeEntriesToPatch =
            new HashSet<LookupDirector.UpgradeEntry>();

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

        public static void RegisterGadget(LookupDirector.GadgetEntry entry)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    gadgetEntriesToPatch.Add(entry);
                    break;
                case LoadingStep.LOAD:
                    GameContext.Instance.LookupDirector.gadgetEntries.Add(entry);
                    GameContext.Instance.LookupDirector.gadgetEntryDict[entry.id] = entry;
                    break;
            }
        }

        public static void RegisterVacEntry(Identifiable.Id id, Color color, Sprite icon)
        {
            RegisterVacEntry(new LookupDirector.VacEntry(){id=id,color=color,icon=icon});
        }

        public static void RegisterUpgradeEntry(LookupDirector.UpgradeEntry entry)
        {
            switch (CurrentLoadingStep)
            {
                case LoadingStep.PRELOAD:
                    upgradeEntriesToPatch.Add(entry);
                    break;
                case LoadingStep.LOAD:
                    GameContext.Instance.LookupDirector.upgradeEntries.Add(entry);
                    GameContext.Instance.LookupDirector.upgradeEntryDict[entry.upgrade] = entry;
                    break;
            }
        }

        public static void RegisterUpgradeEntry(PlayerState.Upgrade upgrade, Sprite icon, int cost)
        {
            RegisterUpgradeEntry(new LookupDirector.UpgradeEntry()
            {
                cost = cost,
                icon = icon,
                upgrade = upgrade
            });
        }
    }
}

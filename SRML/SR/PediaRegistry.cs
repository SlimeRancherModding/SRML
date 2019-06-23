using Harmony;
using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class PediaRegistry
    {
        internal static IDRegistry<PediaDirector.Id> moddedIds = new IDRegistry<PediaDirector.Id>();

        internal static List<PediaDirector.IdentMapEntry> customIdentifiableLinks = new List<PediaDirector.IdentMapEntry>();
        internal static List<PediaDirector.IdEntry> customEntries = new List<PediaDirector.IdEntry>();
        internal static List<PediaDirector.Id> initialEntries = new List<PediaDirector.Id>();
        internal static Dictionary<PediaDirector.Id, PediaCategory> pediaMappings = new Dictionary<PediaDirector.Id, PediaCategory>();
        

        static PediaRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedIds);
        }

        public static PediaDirector.Id CreatePediaId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register slimepedia entries outside of the PreLoad step");
            return moddedIds.RegisterValueWithEnum((PediaDirector.Id)value, name);
        }

        public static void RegisterIdEntry(PediaDirector.IdEntry entry)
        {
            customEntries.Add(entry);
        }
        
        public static void RegisterIdEntry(PediaDirector.Id id, Sprite icon)
        {
            RegisterIdEntry(new PediaDirector.IdEntry() { id = id, icon = icon });
        }

        public static void RegisterInitialPediaEntry(PediaDirector.Id id)
        {
            initialEntries.Add(id);
        }
        
        public static void RegisterIdentifiableMapping(PediaDirector.IdentMapEntry entry)
        {
            customIdentifiableLinks.Add(entry);
        }

        public static void RegisterIdentifiableMapping(PediaDirector.Id pedia, Identifiable.Id ident)
        {
            RegisterIdentifiableMapping(new PediaDirector.IdentMapEntry() { identId = ident, pediaId = pedia });
        }

        static ref PediaDirector.Id[] GetCategory(PediaCategory cat)
        {
            switch (cat)
            {
                case PediaCategory.TUTORIALS:
                    return ref PediaUI.TUTORIALS_ENTRIES;
                case PediaCategory.SLIMES:
                    return ref PediaUI.SLIMES_ENTRIES;
                case PediaCategory.RESOURCES:
                    return ref PediaUI.RESOURCES_ENTRIES;
                case PediaCategory.RANCH:
                    return ref PediaUI.RANCH_ENTRIES;
                case PediaCategory.WORLD:
                    return ref PediaUI.WORLD_ENTRIES;
                case PediaCategory.SCIENCE:
                    return ref PediaUI.SCIENCE_ENTRIES;
            }
            throw new Exception();
        }

        public static void SetPediaCategory(PediaDirector.Id id, PediaCategory category)
        {
            GetCategory(category) = GetCategory(category).Add(id).ToArray();
        }
 
        public enum PediaCategory
        {
            TUTORIALS,
            SLIMES,
            RESOURCES,
            RANCH,
            WORLD,
            SCIENCE
        }
    }
}

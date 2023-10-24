using HarmonyLib;
using SRML.Core.API.BuiltIn;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Identifiable
{
    [HarmonyPatch]
    public class SpawnResourceRegistry : EnumRegistry<SpawnResourceRegistry, SpawnResource.Id>
    {
        internal Dictionary<string, List<SpawnResource>> moddedResources = new Dictionary<string, List<SpawnResource>>();

        public delegate void SpawnResourceRegisterEvent(SpawnResource resource);
        public readonly SpawnResourceRegisterEvent OnRegisterSpawnResource;

        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterPrefabs(LookupDirector __instance) => Instance.RegisterIntoLookup(__instance);

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var resource in Instance.moddedResources.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.resourcePrefabDict[resource.id] = resource.gameObject;
                lookupDirector.resourceSpawnerPrefabs.items.Add(resource.gameObject);
            }
        }

        public virtual void RegisterPrefab(SpawnResource resource)
        {
            if (resource.id == SpawnResource.Id.NONE)
                throw new ArgumentException("Attempting to register a spawn resource prefab with id NONE. This is not allowed.");

            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedResources.ContainsKey(executingId))
                moddedResources[executingId] = new List<SpawnResource>();

            moddedResources[executingId].Add(resource);
            OnRegisterSpawnResource?.Invoke(resource);
        }
        public override void Initialize()
        {
        }
    }
}

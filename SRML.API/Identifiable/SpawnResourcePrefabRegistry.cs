using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System;
using System.Reflection;

namespace SRML.API.Identifiable
{
    public class SpawnResourcePrefabRegistry : ComponentRegistry<SpawnResourcePrefabRegistry, SpawnResource, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        protected override void InitializeComponent(LookupDirector component)
        {
        }

        public override bool IsRegistered(SpawnResource toRegister, LookupDirector component) =>
            component.identifiablePrefabs.items.Contains(toRegister.gameObject);

        protected override void RegisterIntoComponent(SpawnResource toRegister, LookupDirector component)
        {
            if (toRegister.id == SpawnResource.Id.NONE)
                throw new ArgumentException("Attempting to register a spawn resource with id NONE. This is not allowed.");

            component.resourceSpawnerPrefabs.items.Add(toRegister.gameObject);
            component.resourcePrefabDict[toRegister.id] = toRegister.gameObject;
        }
    }
}

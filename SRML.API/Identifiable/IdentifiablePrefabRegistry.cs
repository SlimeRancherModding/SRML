using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System;
using System.Reflection;

namespace SRML.API.Identifiable
{
    public class IdentifiablePrefabRegistry : ComponentRegistry<IdentifiablePrefabRegistry, global::Identifiable, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        protected override void InitializeComponent(LookupDirector component)
        {
        }

        public override bool IsRegistered(global::Identifiable toRegister, LookupDirector component) =>
            component.identifiablePrefabs.items.Contains(toRegister.gameObject);

        protected override void RegisterIntoComponent(global::Identifiable toRegister, LookupDirector component)
        {
            if (toRegister.id == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register an object with id NONE. This is not allowed.");

            component.identifiablePrefabs.items.Add(toRegister.gameObject);
            component.identifiablePrefabDict[toRegister.id] = toRegister.gameObject;
        }
    }
}

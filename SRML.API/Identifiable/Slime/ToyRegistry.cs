using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Linq;
using System.Reflection;
using System;

namespace SRML.API.Identifiable.Slime
{
    public class ToyRegistry : ComponentRegistry<ToyRegistry, ToyDefinition, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        protected override void InitializeComponent(LookupDirector component)
        {
        }

        public override bool IsRegistered(ToyDefinition registered, LookupDirector component) => component.toyDefinitions.Contains(registered);

        protected override void RegisterIntoComponent(ToyDefinition toRegister, LookupDirector component)
        {
            if (toRegister.toyId == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a toy with id NONE. This is not allowed.");

            component.toyDefinitions.items.Add(toRegister);
            component.toyDict[toRegister.toyId] = toRegister;
        }

        public void MarkToyAsBase(global::Identifiable.Id toyId)
        {
            if (toyId == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to process a toy with id NONE. This is not allowed.");

            ToyDirector.BASE_TOYS.Add(toyId);
            ToyDirector.UPGRADED_TOYS.Remove(toyId);
        }

        public void MarkToyAsUpgraded(global::Identifiable.Id toyId)
        {
            if (toyId == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to process a toy with id NONE. This is not allowed.");

            ToyDirector.BASE_TOYS.Remove(toyId);
            ToyDirector.UPGRADED_TOYS.Add(toyId);
        }
    }
}

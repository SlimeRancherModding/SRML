using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System;
using System.Linq;
using System.Reflection;

namespace SRML.API.Identifiable
{
    public class VacItemRegistry : ComponentRegistry<VacItemRegistry, VacItemDefinition, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        protected override void InitializeComponent(LookupDirector component)
        {
        }

        public override bool IsRegistered(VacItemDefinition registered, LookupDirector component) => component.vacItemDefinitions.Contains(registered);

        protected override void RegisterIntoComponent(VacItemDefinition toRegister, LookupDirector component)
        {
            if (toRegister.id == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a vac item definition with id NONE. This is not allowed.");

            component.vacItemDefinitions.items.Add(toRegister);
            component.vacItemDict[toRegister.id] = toRegister;
        }
    }
}

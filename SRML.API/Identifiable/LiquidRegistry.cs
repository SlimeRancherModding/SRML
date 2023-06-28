using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System;
using System.Linq;
using System.Reflection;

namespace SRML.API.Identifiable
{
    public class LiquidRegistry : ComponentRegistry<LiquidRegistry, LiquidDefinition, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        protected override void InitializeComponent(LookupDirector component)
        {
        }

        public override bool IsRegistered(LiquidDefinition registered, LookupDirector component) => component.liquidDefinitions.Contains(registered);

        protected override void RegisterIntoComponent(LiquidDefinition toRegister, LookupDirector component)
        {
            if (toRegister.id == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a liquid with id NONE. This is not allowed.");

            component.liquidDefinitions.items.Add(toRegister);
            component.liquidDict[toRegister.id] = toRegister;
        }
    }
}

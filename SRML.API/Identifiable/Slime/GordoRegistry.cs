using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System;
using System.Linq;
using System.Reflection;

namespace SRML.API.Identifiable.Slime
{
    public class GordoRegistry : ComponentRegistry<GordoRegistry, GordoIdentifiable, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        protected override void InitializeComponent(LookupDirector component)
        {
        }

        public override bool IsRegistered(GordoIdentifiable registered, LookupDirector component) => component.gordoEntries.Contains(registered.gameObject);

        protected override void RegisterIntoComponent(GordoIdentifiable toRegister, LookupDirector component)
        {
            if (toRegister.id == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a gordo with id NONE. This is not allowed.");

            component.gordoEntries.items.Add(toRegister.gameObject);
            component.gordoDict[toRegister.id] = toRegister.gameObject;
        }
    }
}

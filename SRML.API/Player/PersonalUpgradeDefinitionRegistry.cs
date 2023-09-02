using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Reflection;

namespace SRML.API.Player
{
    public class PersonalUpgradeDefinitionRegistry : ComponentRegistry<PersonalUpgradeDefinitionRegistry, UpgradeDefinition, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        public override bool IsRegistered(UpgradeDefinition registered, LookupDirector component) =>
            component.upgradeDefinitions.items.Contains(registered);

        protected override void RegisterIntoComponent(UpgradeDefinition toRegister, LookupDirector component)
        {
            component.upgradeDefinitions.items.Add(toRegister);
            component.upgradeDefinitionDict[toRegister.upgrade] = toRegister;
        }
    }
}

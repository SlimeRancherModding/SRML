using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Collections.Generic;
using System.Reflection;

namespace SRML.API.Gadget
{
    public class BlueprintRegistry : ComponentRegistry<BlueprintRegistry, GadgetDefinition, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        public delegate GadgetDirector.BlueprintLocker BlueprintLock(GadgetDirector director);

        internal Dictionary<global::Gadget.Id, BlueprintLock> blueprintLocks = new Dictionary<global::Gadget.Id, BlueprintLock>();
        internal List<global::Gadget.Id> defaultUnlocked = new List<global::Gadget.Id>();
        internal List<global::Gadget.Id> defaultAvail = new List<global::Gadget.Id>();

        public void RegisterBlueprintLock(global::Gadget.Id id, BlueprintLock blueprintLock) => blueprintLocks[id] = blueprintLock;
        public void RegisterDefaultUnlockedBlueprint(global::Gadget.Id id) => defaultUnlocked.Add(id);
        public void RegisterDefaultAvailableBlueprint(global::Gadget.Id id) => defaultAvail.Add(id);

        public override bool IsRegistered(GadgetDefinition registered, LookupDirector component) =>
            component.gadgetDefinitions.items.Contains(registered);

        protected override void RegisterIntoComponent(GadgetDefinition toRegister, LookupDirector component)
        {
            component.gadgetDefinitions.items.Add(toRegister);
            component.gadgetDefinitionDict.Add(toRegister.id, toRegister);
        }
    }
}

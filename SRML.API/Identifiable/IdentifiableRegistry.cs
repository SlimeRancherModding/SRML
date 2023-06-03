using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Reflection;

namespace SRML.API.Identifiable
{
    public class IdentifiableRegistry : ComponentRegistry<global::Identifiable, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        public override void InitializeComponent(LookupDirector component)
        {
        }

        public override bool IsRegistered(global::Identifiable toRegister, LookupDirector component) =>
            component.identifiablePrefabs.items.Contains(toRegister.gameObject);

        public override void RegisterIntoComponent(global::Identifiable toRegister, LookupDirector component)
        {
            component.identifiablePrefabs.items.Add(toRegister.gameObject);
            component.identifiablePrefabDict[toRegister.id] = toRegister.gameObject;
        }
    }
}

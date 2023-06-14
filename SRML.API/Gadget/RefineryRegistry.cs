using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Linq;
using System.Reflection;

namespace SRML.API.Gadget
{
    public class RefineryRegistry : ComponentRegistry<RefineryRegistry, global::Identifiable.Id, RefineryUI>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(RefineryUI), "Awake");
        public override bool Prefix => true;

        public override bool IsRegistered(global::Identifiable.Id registered, RefineryUI component) =>
            component.listedItems.Contains(registered);

        protected override void InitializeComponent(RefineryUI component)
        {
        }

        protected override void RegisterIntoComponent(global::Identifiable.Id toRegister, RefineryUI component) =>
            component.listedItems = component.listedItems.AddToArray(toRegister);
    }
}

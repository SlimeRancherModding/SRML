using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Reflection;

namespace SRML.API.World
{
    public class LandPlotPrefabRegistry : ComponentRegistry<LandPlotPrefabRegistry, LandPlot, LookupDirector>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(LookupDirector), "Awake");
        public override bool Prefix => true;

        public override bool IsRegistered(LandPlot registered, LookupDirector component) =>
            component.plotPrefabs.items.Contains(registered.gameObject);

        protected override void RegisterIntoComponent(LandPlot toRegister, LookupDirector component)
        {
            component.plotPrefabs.items.Add(toRegister.gameObject);
            component.plotPrefabDict[toRegister.typeId] = toRegister.gameObject;
        }
    }
}

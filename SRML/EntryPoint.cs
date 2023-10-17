using SRML.Core.ModLoader;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.SR.Templates;
using SRML.SR.Utils.BaseObjects;

namespace SRML
{
    public class EntryPoint : CoreModEntryPoint
    {
        public EntryPoint(IModInfo info) : base(info)
        {
        }

        public override void Initialize()
        {
            BaseObjects.OnPopulateGlobalActions += () =>
            {
                TemplateActions.RegisterAction("buildSlime", BaseObjects.AssembleModules);
                TemplateActions.RegisterAction("populateSlime", BaseObjects.PopulateSlimeInfo);
            };
        }
    }
}

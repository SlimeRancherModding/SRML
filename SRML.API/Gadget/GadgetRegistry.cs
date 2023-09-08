using SRML.API.Gadget.Attributes;
using SRML.Core.API.BuiltIn;
using System.Collections.Generic;

namespace SRML.API.Gadget
{
    public class GadgetRegistry : NameCategorizedEnumRegistry<GadgetRegistry, global::Gadget.Id, GadgetCategorizationAttribute.Rule>
    {
        public override void Initialize()
        {
            base.Initialize();

            metadata.categorizationPrefixRules = new Dictionary<string, GadgetCategorizationAttribute.Rule>()
            {
                { "EXTRACTOR_", GadgetCategorizationAttribute.Rule.EXTRACTOR },
                { "TELEPORTER_", GadgetCategorizationAttribute.Rule.TELEPORTER },
                { "WARP_DEPOT_", GadgetCategorizationAttribute.Rule.WARP_DEPOT },
                { "ECHO_NET", GadgetCategorizationAttribute.Rule.ECHO_NET },
                { "LAMP_", GadgetCategorizationAttribute.Rule.LAMP },
                { "FASHION_POD_", GadgetCategorizationAttribute.Rule.FASHION_POD },
                { "GORDO_SNARE_", GadgetCategorizationAttribute.Rule.SNARE },
                { "DRONE", GadgetCategorizationAttribute.Rule.DRONE }
            };
            metadata.ruleLists = new Dictionary<GadgetCategorizationAttribute.Rule, List<HashSet<global::Gadget.Id>>>()
            {
                { GadgetCategorizationAttribute.Rule.EXTRACTOR, new List<HashSet<global::Gadget.Id>>() { global::Gadget.EXTRACTOR_CLASS } },
                { GadgetCategorizationAttribute.Rule.TELEPORTER, new List<HashSet<global::Gadget.Id>>() { global::Gadget.TELEPORTER_CLASS } },
                { GadgetCategorizationAttribute.Rule.WARP_DEPOT, new List<HashSet<global::Gadget.Id>>() { global::Gadget.WARP_DEPOT_CLASS } },
                { GadgetCategorizationAttribute.Rule.ECHO_NET, new List<HashSet<global::Gadget.Id>>() { global::Gadget.ECHO_NET_CLASS } },
                { GadgetCategorizationAttribute.Rule.LAMP, new List<HashSet<global::Gadget.Id>>() { global::Gadget.LAMP_CLASS } },
                { GadgetCategorizationAttribute.Rule.FASHION_POD, new List<HashSet<global::Gadget.Id>>() { global::Gadget.FASHION_POD_CLASS } },
                { GadgetCategorizationAttribute.Rule.SNARE, new List<HashSet<global::Gadget.Id>>() { global::Gadget.SNARE_CLASS } },
                { GadgetCategorizationAttribute.Rule.DRONE, new List<HashSet<global::Gadget.Id>>() { global::Gadget.DRONE_CLASS } }
            };
            metadata.baseRuleLists = new List<HashSet<global::Gadget.Id>>()
            {
                global::Gadget.EXTRACTOR_CLASS,
                global::Gadget.TELEPORTER_CLASS,
                global::Gadget.WARP_DEPOT_CLASS,
                global::Gadget.ECHO_NET_CLASS,
                global::Gadget.LAMP_CLASS,
                global::Gadget.FASHION_POD_CLASS,
                global::Gadget.SNARE_CLASS,
                global::Gadget.DRONE_CLASS
            };
        }
    }
}

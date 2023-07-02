using SRML.Core.API.BuiltIn;
using SRML.SR;
using System;
using System.Collections.Generic;

namespace SRML.API.Gadget
{
    public class GadgetRegistry : NameCategorizableEnumRegistry<GadgetRegistry, global::Gadget.Id, GadgetCategorization.Rule>,
        IAttributeCategorizeableEnum
    {
        public Type AttributeType => typeof(GadgetCategorization);
        public bool TakesPresidenceOverCategorizable => true;

        public override void Initialize()
        {
            base.Initialize();

            categorizationPrefixRules = new Dictionary<string, GadgetCategorization.Rule>()
            {
                { "EXTRACTOR_", GadgetCategorization.Rule.EXTRACTOR },
                { "TELEPORTER_", GadgetCategorization.Rule.TELEPORTER },
                { "WARP_DEPOT_", GadgetCategorization.Rule.WARP_DEPOT },
                { "ECHO_NET", GadgetCategorization.Rule.ECHO_NET },
                { "LAMP_", GadgetCategorization.Rule.LAMP },
                { "FASHION_POD_", GadgetCategorization.Rule.FASHION_POD },
                { "GORDO_SNARE_", GadgetCategorization.Rule.SNARE },
                { "DRONE", GadgetCategorization.Rule.DRONE }
            };
            ruleLists = new Dictionary<GadgetCategorization.Rule, List<HashSet<global::Gadget.Id>>>()
            {
                { GadgetCategorization.Rule.EXTRACTOR, new List<HashSet<global::Gadget.Id>>() { global::Gadget.EXTRACTOR_CLASS } },
                { GadgetCategorization.Rule.TELEPORTER, new List<HashSet<global::Gadget.Id>>() { global::Gadget.TELEPORTER_CLASS } },
                { GadgetCategorization.Rule.WARP_DEPOT, new List<HashSet<global::Gadget.Id>>() { global::Gadget.WARP_DEPOT_CLASS } },
                { GadgetCategorization.Rule.ECHO_NET, new List<HashSet<global::Gadget.Id>>() { global::Gadget.ECHO_NET_CLASS } },
                { GadgetCategorization.Rule.LAMP, new List<HashSet<global::Gadget.Id>>() { global::Gadget.LAMP_CLASS } },
                { GadgetCategorization.Rule.FASHION_POD, new List<HashSet<global::Gadget.Id>>() { global::Gadget.FASHION_POD_CLASS } },
                { GadgetCategorization.Rule.SNARE, new List<HashSet<global::Gadget.Id>>() { global::Gadget.SNARE_CLASS } },
                { GadgetCategorization.Rule.DRONE, new List<HashSet<global::Gadget.Id>>() { global::Gadget.DRONE_CLASS } }
            };
            baseRuleLists = new List<HashSet<global::Gadget.Id>>()
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

        public void Categorize(Enum toCategorize, Attribute att)
        {
            if (att.GetType() != typeof(GadgetCategorization))
                throw new ArgumentException("GadgetRegistry cannot process non-GadgetCategorization attributes.");

            Categorize((global::Gadget.Id)toCategorize, ((GadgetCategorization)att).rules);
        }

        public void RegisterIdentifiableTranslationMapping(global::Gadget.Id gadgetId, global::Identifiable.Id identId) =>
            global::Identifiable.GADGET_NAME_DICT.Add(identId, gadgetId);

        public override void Process(global::Gadget.Id toProcess)
        {
        }
    }
}

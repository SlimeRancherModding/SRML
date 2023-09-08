using SRML.API.Gadget.Attributes;
using SRML.Core.API.BuiltIn.Processors;

namespace SRML.API.Gadget.Processors
{
    public class GadgetProcessor : NameCategorizedEnumProcessor<global::Gadget.Id, GadgetCategorizationAttribute.Rule>
    {
        public GadgetProcessor(string name, NameCategorizedEnumMetadata<global::Gadget.Id, GadgetCategorizationAttribute.Rule> metadata) : 
            base(name, metadata)
        {
        }
        public GadgetProcessor(string name, object value, NameCategorizedEnumMetadata<global::Gadget.Id, GadgetCategorizationAttribute.Rule> metadata) : 
            base(name, value, metadata)
        {
        }
    }
}

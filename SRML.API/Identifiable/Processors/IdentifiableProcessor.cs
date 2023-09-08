using SRML.API.Identifiable.Attributes;
using SRML.Core.API.BuiltIn.Processors;

namespace SRML.API.Identifiable.Processors
{
    public class IdentifiableProcessor : NameCategorizedEnumProcessor<global::Identifiable.Id, IdentifiableCategorizationAttribute.Rule>
    {
        public IdentifiableProcessor(string name, NameCategorizedEnumMetadata<global::Identifiable.Id, IdentifiableCategorizationAttribute.Rule> metadata) : 
            base(name, metadata)
        {
        }
        public IdentifiableProcessor(string name, object value, NameCategorizedEnumMetadata<global::Identifiable.Id, IdentifiableCategorizationAttribute.Rule> metadata) : 
            base(name, value, metadata)
        {
        }

        public override void Initialize() =>
            OnCategorize += x => Slime.FoodGroupRegistry.Instance.Register(x);
    }
}

using System;
using System.Collections.Generic;

namespace SRML.Core.API.BuiltIn.Processors
{
    public abstract class NameCategorizedEnumProcessor<TEnum, TRule> : EnumProcessor<TEnum> 
        where TEnum : Enum
        where TRule : Enum
    {
        public List<TEnum> Categorized => new List<TEnum>();
        private readonly NameCategorizedEnumMetadata<TEnum, TRule> metadata;

        public bool IsCategorized(TEnum categorized) => Categorized.Contains(categorized);

        public void Categorize(TEnum toCategorize, TRule rule)
        {
            string name = Enum.GetName(typeof(TEnum), toCategorize);
            uint ruleInt = Convert.ToUInt32(rule);

            foreach (var ruleList in metadata.ruleLists)
            {
                if ((Convert.ToUInt32(ruleList.Key) & ruleInt) != 0)
                {
                    foreach (var hashset in ruleList.Value)
                        hashset.Add(toCategorize);
                }
            }
            foreach (var processor in metadata.processors)
                processor(toCategorize, name, rule);

            Categorized.Add(toCategorize);
            base.Categorize(toCategorize);
        }

        public override void Categorize(TEnum toCategorize)
        {
            string name = Enum.GetName(typeof(TEnum), toCategorize);

            uint rule = default;
            foreach (var prefix in metadata.categorizationPrefixRules)
            {
                if (name.StartsWith(prefix.Key))
                {
                    rule |= Convert.ToUInt32(prefix.Value);
                    break;
                }
            }
            foreach (var suffix in metadata.categorizationSuffixRules)
            {
                if (name.EndsWith(suffix.Key))
                {
                    rule |= Convert.ToUInt32(suffix.Value);
                    break;
                }
            }

            Categorize(toCategorize, (TRule)Enum.Parse(typeof(TRule), rule.ToString(), true)); // slightly less dumb but I still hate you
        }

        public override void Decategorize(TEnum toDecategorize)
        {
            foreach (var hashset in metadata.baseRuleLists)
                hashset.Remove(toDecategorize);
        }

        public NameCategorizedEnumProcessor(string name, NameCategorizedEnumMetadata<TEnum, TRule> metadata) : base(name) 
        {
            this.metadata = metadata;
        }
        public NameCategorizedEnumProcessor(string name, object value, NameCategorizedEnumMetadata<TEnum, TRule> metadata) : base(name, value) 
        { 
            this.metadata = metadata;
        }
    }
}

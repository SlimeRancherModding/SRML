using System;
using System.Collections.Generic;

namespace SRML.Core.API.BuiltIn.Processors
{
    public class NameCategorizedEnumMetadata<TEnum, TRule> 
        where TEnum : Enum
        where TRule : Enum
    {
        public delegate void CategorizationProcessor(TEnum id, string name, TRule rule);

        public Dictionary<string, TRule> categorizationSuffixRules = new Dictionary<string, TRule>();
        public Dictionary<string, TRule> categorizationPrefixRules = new Dictionary<string, TRule>();
        public Dictionary<TRule, List<HashSet<TEnum>>> ruleLists = new Dictionary<TRule, List<HashSet<TEnum>>>();
        public List<HashSet<TEnum>> baseRuleLists = new List<HashSet<TEnum>>();
        public List<CategorizationProcessor> processors = new List<CategorizationProcessor>();

        public delegate void CategorizationPostProcessor(TEnum value);
        public CategorizationPostProcessor OnCategorize;

        public List<TEnum> Categorized => new List<TEnum>();

        public bool IsCategorized(TEnum categorized) => Categorized.Contains(categorized);

        public void Categorize(TEnum toCategorize, TRule rule)
        {
            string name = Enum.GetName(typeof(TEnum), toCategorize);
            uint ruleInt = Convert.ToUInt32(rule);

            foreach (var ruleList in ruleLists)
            {
                if ((Convert.ToUInt32(ruleList.Key) & ruleInt) != 0)
                {
                    foreach (var hashset in ruleList.Value)
                        hashset.Add(toCategorize);
                }
            }
            foreach (var processor in processors)
                processor(toCategorize, name, rule);

            Categorized.Add(toCategorize);
            OnCategorize?.Invoke(toCategorize);
        }

        public void Categorize(TEnum toCategorize)
        {
            string name = Enum.GetName(typeof(TEnum), toCategorize);

            uint rule = default;
            foreach (var prefix in categorizationPrefixRules)
            {
                if (name.StartsWith(prefix.Key))
                {
                    rule |= Convert.ToUInt32(prefix.Value);
                    break;
                }
            }
            foreach (var suffix in categorizationSuffixRules)
            {
                if (name.EndsWith(suffix.Key))
                {
                    rule |= Convert.ToUInt32(suffix.Value);
                    break;
                }
            }

            Categorize(toCategorize, (TRule)Enum.Parse(typeof(TRule), rule.ToString(), true)); // slightly less dumb but I still hate you
        }

        public void Decategorize(TEnum toDecategorize)
        {
            foreach (var hashset in baseRuleLists)
                hashset.Remove(toDecategorize);
        }
    }
}

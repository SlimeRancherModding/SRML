using System;
using System.Collections.Generic;

namespace SRML.Core.API.BuiltIn
{
    public abstract class NameCategorizableEnumRegistry<R, TEnum, TRule> : EnumRegistry<R, TEnum>, 
        ICategorizableEnum
        where R : NameCategorizableEnumRegistry<R, TEnum, TRule>
        where TEnum : Enum
        where TRule : Enum
    {
        public delegate void CategorizationProcessor(TEnum id, string name, TRule rule);

        protected Dictionary<string, TRule> categorizationSuffixRules = new Dictionary<string, TRule>();
        protected Dictionary<string, TRule> categorizationPrefixRules = new Dictionary<string, TRule>();
        protected Dictionary<TRule, List<HashSet<TEnum>>> ruleLists = new Dictionary<TRule, List<HashSet<TEnum>>>();
        protected List<HashSet<TEnum>> baseRuleLists = new List<HashSet<TEnum>>();
        protected List<CategorizationProcessor> processors = new List<CategorizationProcessor>();

        public List<Enum> Categorized => new List<Enum>();

        protected virtual void OnCategorize(TEnum toCategorize)
        {
        }

        public bool IsCategorized(Enum categorized) => Categorized.Contains(categorized);

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
            OnCategorize(toCategorize);
        }

        public void Categorize(Enum toCategorize)
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

            Categorize((TEnum)toCategorize, (TRule)Enum.Parse(typeof(TRule), rule.ToString(), true)); // dumb I hate you
        }

        public void Decategorize(Enum toDecategorize)
        {
            TEnum id = (TEnum)toDecategorize;

            foreach (var hashset in baseRuleLists)
                hashset.Remove(id);
        }

        public void RegisterPrefixRule(string prefix, TRule rule) =>
            categorizationPrefixRules[prefix] = rule;

        public void RegisterSuffixRule(string suffix, TRule rule) =>
            categorizationSuffixRules[suffix] = rule;

        public void RegisterCategorizationProcessor(CategorizationProcessor processor) =>
            processors.Add(processor);

        public void RegisterHashSet(TRule rule, HashSet<TEnum> hashset)
        {
            if (!ruleLists.ContainsKey(rule))
                ruleLists[rule] = new List<HashSet<TEnum>>();

            ruleLists[rule].Add(hashset);
            baseRuleLists.AddIfDoesNotContain(hashset);
        }
    }
}

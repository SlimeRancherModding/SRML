using SRML.Core.API.BuiltIn.Processors;
using System;
using System.Collections.Generic;

namespace SRML.Core.API.BuiltIn
{
    public abstract class NameCategorizedEnumRegistry<R, TEnum, TRule> : Registry<R>
        where R : Registry<R>
        where TEnum : Enum
        where TRule : Enum
    {
        protected NameCategorizedEnumMetadata<TEnum, TRule> metadata;

        public override void Initialize() => metadata = new NameCategorizedEnumMetadata<TEnum, TRule>();

        public void RegisterPrefixRule(string prefix, TRule rule) =>
            metadata.categorizationPrefixRules[prefix] = rule;

        public void RegisterSuffixRule(string suffix, TRule rule) =>
            metadata.categorizationSuffixRules[suffix] = rule;

        public void RegisterCategorizationProcessor(NameCategorizedEnumMetadata<TEnum, TRule>.CategorizationProcessor processor) =>
            metadata.processors.Add(processor);

        public void RegisterHashSet(TRule rule, HashSet<TEnum> hashset)
        {
            if (!metadata.ruleLists.ContainsKey(rule))
                metadata.ruleLists[rule] = new List<HashSet<TEnum>>();

            metadata.ruleLists[rule].Add(hashset);
            metadata.baseRuleLists.AddIfDoesNotContain(hashset);
        }
    }
}

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
    }
}

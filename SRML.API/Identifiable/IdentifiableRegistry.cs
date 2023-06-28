using SRML.Console;
using SRML.Core.API.BuiltIn;
using SRML.SR;
using System;
using System.Collections.Generic;

namespace SRML.API.Identifiable
{
    public class IdentifiableRegistry : EnumRegistry<IdentifiableRegistry, global::Identifiable.Id>, 
        ICategorizableEnum, IAttributeCategorizeableEnum
    {
        public List<Enum> Categorized => new List<Enum>();
        public Type AttributeType => typeof(IdentifiableCategorization);
        public bool TakesPresidenceOverCategorizable => true;

        public delegate void CategorizationProcessor(global::Identifiable.Id id, string name, IdentifiableCategorization.Rule rule);

        // this is now extendable! but it's still TERRIBLE
        internal Dictionary<string, IdentifiableCategorization.Rule> categorizationSuffixRules = new Dictionary<string, IdentifiableCategorization.Rule>()
        {
            { "_VEGGIE", IdentifiableCategorization.Rule.VEGGIE },
            { "_FRUIT", IdentifiableCategorization.Rule.FRUIT },
            { "_TOFU", IdentifiableCategorization.Rule.TOFU },
            { "_SLIME", IdentifiableCategorization.Rule.SLIME },
            { "_LARGO", IdentifiableCategorization.Rule.LARGO },
            { "_GORDO", IdentifiableCategorization.Rule.GORDO },
            { "_PLORT", IdentifiableCategorization.Rule.PLORT },
            { "HEN", IdentifiableCategorization.Rule.MEAT },
            { "ROOSTER", IdentifiableCategorization.Rule.MEAT },
            { "CHICK", IdentifiableCategorization.Rule.CHICK },
            { "_LIQUID", IdentifiableCategorization.Rule.LIQUID },
            { "_CRAFT", IdentifiableCategorization.Rule.CRAFT },
            { "_FASHION", IdentifiableCategorization.Rule.FASHION },
            { "_ECHO", IdentifiableCategorization.Rule.ECHO },
            { "_ORNAMENT", IdentifiableCategorization.Rule.ORNAMENT },
            { "_TOY", IdentifiableCategorization.Rule.TOY }
        };
        internal Dictionary<string, IdentifiableCategorization.Rule> categorizationPrefixRules = new Dictionary<string, IdentifiableCategorization.Rule>()
        {
            { "ECHO_NOTE_", IdentifiableCategorization.Rule.ECHO_NOTE },
            { "ELDER_", IdentifiableCategorization.Rule.ELDER }
        };
        internal Dictionary<IdentifiableCategorization.Rule, List<HashSet<global::Identifiable.Id>>> ruleLists =
            new Dictionary<IdentifiableCategorization.Rule, List<HashSet<global::Identifiable.Id>>>()
        {
                { IdentifiableCategorization.Rule.VEGGIE, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.FOOD_CLASS, global::Identifiable.VEGGIE_CLASS } },
                { IdentifiableCategorization.Rule.FRUIT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.FOOD_CLASS, global::Identifiable.FRUIT_CLASS } },
                { IdentifiableCategorization.Rule.MEAT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.FOOD_CLASS, global::Identifiable.MEAT_CLASS } },
                { IdentifiableCategorization.Rule.TOFU, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.FOOD_CLASS, global::Identifiable.TOFU_CLASS } },
                { IdentifiableCategorization.Rule.PLORT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.PLORT_CLASS } },
                { IdentifiableCategorization.Rule.CHICK, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.CHICK_CLASS } },
                { IdentifiableCategorization.Rule.CRAFT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.CRAFT_CLASS } },
                { IdentifiableCategorization.Rule.SLIME, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.SLIME_CLASS,
                global::Identifiable.EATERS_CLASS } },
                { IdentifiableCategorization.Rule.LARGO, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.LARGO_CLASS,
                global::Identifiable.EATERS_CLASS } },
                { IdentifiableCategorization.Rule.GORDO, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.GORDO_CLASS } },
                { IdentifiableCategorization.Rule.LIQUID, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.LIQUID_CLASS } },
                { IdentifiableCategorization.Rule.FASHION, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.FASHION_CLASS } },
                { IdentifiableCategorization.Rule.ECHO, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ECHO_CLASS } },
                { IdentifiableCategorization.Rule.ECHO_NOTE, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ECHO_NOTE_CLASS } },
                { IdentifiableCategorization.Rule.ORNAMENT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ORNAMENT_CLASS } },
                { IdentifiableCategorization.Rule.TOY, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.TOY_CLASS } },
                { IdentifiableCategorization.Rule.ELDER, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ELDER_CLASS } },
        };
        internal List<HashSet<global::Identifiable.Id>> baseRuleLists = new List<HashSet<global::Identifiable.Id>>()
        {
            global::Identifiable.ALLERGY_FREE_CLASS,
            global::Identifiable.BOOP_CLASS,
            global::Identifiable.CHICK_CLASS,
            global::Identifiable.CRAFT_CLASS,
            global::Identifiable.EATERS_CLASS,
            global::Identifiable.ECHO_CLASS,
            global::Identifiable.ECHO_NOTE_CLASS,
            global::Identifiable.ELDER_CLASS,
            global::Identifiable.FASHION_CLASS,
            global::Identifiable.FOOD_CLASS,
            global::Identifiable.FRUIT_CLASS,
            global::Identifiable.GORDO_CLASS,
            global::Identifiable.LARGO_CLASS,
            global::Identifiable.LIQUID_CLASS,
            global::Identifiable.MEAT_CLASS,
            global::Identifiable.NON_SLIMES_CLASS,
            global::Identifiable.ORNAMENT_CLASS,
            global::Identifiable.PLORT_CLASS,
            global::Identifiable.SLIME_CLASS,
            global::Identifiable.STANDARD_CRATE_CLASS,
            global::Identifiable.TARR_CLASS,
            global::Identifiable.TOFU_CLASS,
            global::Identifiable.TOY_CLASS,
        };
        internal List<CategorizationProcessor> processors = new List<CategorizationProcessor>()
        {
            (x, y, z) =>
            {
                if (y.Contains("TANGLE"))
                    global::Identifiable.ALLERGY_FREE_CLASS.Add(x);
            },
            (x, y, z) =>
            {
                if (y.Contains("TABBY"))
                    global::Identifiable.BOOP_CLASS.Add(x);
            },
            (x, y, z) =>
            {
                if (y.Contains("TARR"))
                    global::Identifiable.TARR_CLASS.Add(x);
            },
        };

        public bool IsCategorized(Enum categorized) => Categorized.Contains(categorized);

        public void Categorize(Enum toCategorize, Attribute att)
        {
            if (att.GetType() != typeof(IdentifiableCategorization))
                throw new ArgumentException("IdentifiableRegistry cannot process non-IdentifiableCategorization attributes.");

            Categorize((global::Identifiable.Id)toCategorize, ((IdentifiableCategorization)att).rules);
        }

        public void Categorize(global::Identifiable.Id id, IdentifiableCategorization.Rule rule)
        {
            string name = Enum.GetName(typeof(global::Identifiable.Id), id);
            if (rule == IdentifiableCategorization.Rule.NONE) return;

            foreach (var ruleList in ruleLists)
            {
                if ((ruleList.Key & rule) != 0)
                {
                    Console.Console.Instance.Log($"registering {id} as {rule}");
                    foreach (var hashset in ruleList.Value)
                        hashset.Add(id);
                }
            }
            foreach (var processor in processors)
                processor(id, name, rule);

            Categorized.Add(id);

            Slime.FoodGroupRegistry.Instance.Register(id);
        }

        public void Categorize(Enum toCategorize)
        {
            string name = Enum.GetName(typeof(global::Identifiable.Id), toCategorize);

            IdentifiableCategorization.Rule rule = IdentifiableCategorization.Rule.NONE;
            foreach (var prefix in categorizationPrefixRules)
            {
                if (name.StartsWith(prefix.Key))
                {
                    rule |= prefix.Value;
                    break;
                }
            }
            foreach (var suffix in categorizationSuffixRules)
            {
                if (name.EndsWith(suffix.Key))
                {
                    rule |= suffix.Value;
                    break;
                }
            }

            Categorize((global::Identifiable.Id)toCategorize, rule);
        }

        public void Decategorize(Enum toDecategorize)
        {
            global::Identifiable.Id id = (global::Identifiable.Id)toDecategorize;

            foreach (var hashset in baseRuleLists)
                hashset.Remove(id);
        }

        public void RegisterPrefixRule(string prefix, IdentifiableCategorization.Rule rule) =>
            categorizationPrefixRules[prefix] = rule;

        public void RegisterSuffixRule(string suffix, IdentifiableCategorization.Rule rule) =>
            categorizationSuffixRules[suffix] = rule;

        public void RegisterCategorizationProcessor(CategorizationProcessor processor) =>
            processors.Add(processor);

        public void RegisterHashSet(IdentifiableCategorization.Rule rule, HashSet<global::Identifiable.Id> hashset)
        {
            if (!ruleLists.ContainsKey(rule))
                ruleLists[rule] = new List<HashSet<global::Identifiable.Id>>();

            ruleLists[rule].Add(hashset);
            baseRuleLists.AddIfDoesNotContain(hashset);
        }

        public override void Process(global::Identifiable.Id toProcess)
        {
        }
    }
}

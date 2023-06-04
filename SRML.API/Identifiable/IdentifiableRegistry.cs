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

        // TODO: remake this ENTIRE thing in a more extendable way
        // currently, the system is just directly ported from the original IdentifiableRegistry
        // and it's incredibly stupid
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
            { "ECHO_NOTE_", IdentifiableCategorization.Rule.ECHO_NOTE }
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
            if (name.Contains("TANGLE"))
            {
                global::Identifiable.ALLERGY_FREE_CLASS.Add(id);
            }
            if (rule == IdentifiableCategorization.Rule.NONE) return;

            if ((rule & (IdentifiableCategorization.Rule.VEGGIE |
                IdentifiableCategorization.Rule.FRUIT |
                IdentifiableCategorization.Rule.TOFU |
                IdentifiableCategorization.Rule.PLORT |
                IdentifiableCategorization.Rule.MEAT |
                IdentifiableCategorization.Rule.CHICK |
                IdentifiableCategorization.Rule.CRAFT)) != 0) global::Identifiable.NON_SLIMES_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.VEGGIE |
                IdentifiableCategorization.Rule.FRUIT |
                IdentifiableCategorization.Rule.TOFU |
                IdentifiableCategorization.Rule.MEAT)) != 0) global::Identifiable.FOOD_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.VEGGIE)) != 0) global::Identifiable.VEGGIE_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.FRUIT)) != 0) global::Identifiable.FRUIT_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.TOFU)) != 0) global::Identifiable.TOFU_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.SLIME)) != 0) global::Identifiable.SLIME_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.LARGO)) != 0) global::Identifiable.LARGO_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.GORDO)) != 0) global::Identifiable.GORDO_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.PLORT)) != 0) global::Identifiable.PLORT_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.MEAT)) != 0) global::Identifiable.MEAT_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.CHICK)) != 0) global::Identifiable.CHICK_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.LIQUID)) != 0) global::Identifiable.LIQUID_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.CRAFT)) != 0) global::Identifiable.CRAFT_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.FASHION)) != 0) global::Identifiable.FASHION_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.ECHO)) != 0) global::Identifiable.ECHO_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.ECHO_NOTE)) != 0) global::Identifiable.ECHO_NOTE_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.ORNAMENT)) != 0) global::Identifiable.ORNAMENT_CLASS.Add(id);
            if ((rule & (IdentifiableCategorization.Rule.TOY)) != 0) global::Identifiable.TOY_CLASS.Add(id);

            global::Identifiable.EATERS_CLASS.UnionWith(global::Identifiable.SLIME_CLASS);
            global::Identifiable.EATERS_CLASS.UnionWith(global::Identifiable.LARGO_CLASS);

            Categorized.Add(id);
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

            global::Identifiable.ALLERGY_FREE_CLASS.Remove(id);
            global::Identifiable.BOOP_CLASS.Remove(id);
            global::Identifiable.CHICK_CLASS.Remove(id);
            global::Identifiable.CRAFT_CLASS.Remove(id);
            global::Identifiable.EATERS_CLASS.Remove(id);
            global::Identifiable.ECHO_CLASS.Remove(id);
            global::Identifiable.ECHO_NOTE_CLASS.Remove(id);
            global::Identifiable.ELDER_CLASS.Remove(id);
            global::Identifiable.FASHION_CLASS.Remove(id);
            global::Identifiable.FOOD_CLASS.Remove(id);
            global::Identifiable.FRUIT_CLASS.Remove(id);
            global::Identifiable.GORDO_CLASS.Remove(id);
            global::Identifiable.LARGO_CLASS.Remove(id);
            global::Identifiable.LIQUID_CLASS.Remove(id);
            global::Identifiable.MEAT_CLASS.Remove(id);
            global::Identifiable.NON_SLIMES_CLASS.Remove(id);
            global::Identifiable.ORNAMENT_CLASS.Remove(id);
            global::Identifiable.PLORT_CLASS.Remove(id);
            global::Identifiable.SLIME_CLASS.Remove(id);
            global::Identifiable.STANDARD_CRATE_CLASS.Remove(id);
            global::Identifiable.TARR_CLASS.Remove(id);
            global::Identifiable.TOFU_CLASS.Remove(id);
            global::Identifiable.TOY_CLASS.Remove(id);
        }

        public void RegisterPrefixRule(string prefix, IdentifiableCategorization.Rule rule) =>
            categorizationPrefixRules[prefix] = rule;

        public void RegisterSuffixRule(string suffix, IdentifiableCategorization.Rule rule) =>
            categorizationSuffixRules[suffix] = rule;

        public override void Process(global::Identifiable.Id toProcess)
        {
        }
    }
}

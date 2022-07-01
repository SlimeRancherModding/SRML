using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.SR.SaveSystem;
using UnityEngine;

namespace SRML.SR
{
    public static class IdentifiableRegistry
    {
        internal static IDRegistry<Identifiable.Id> moddedIdentifiables = new IDRegistry<Identifiable.Id>();
        internal static Dictionary<Identifiable.Id, IdentifiableCategorization.Rule> rules = new Dictionary<Identifiable.Id, IdentifiableCategorization.Rule>();

        static IdentifiableRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedIdentifiables);
        }

        public static Identifiable.Id CreateIdentifiableId(object value, string name) => CreateIdentifiableId(value, name, true);

        public static Identifiable.Id CreateIdentifiableId(object value, string name, bool shouldCategorize = true)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register identifiables outside of the PreLoad step");
            var id = moddedIdentifiables.RegisterValueWithEnum((Identifiable.Id)value,name);
            if (!shouldCategorize) id.Categorize(IdentifiableCategorization.Rule.NONE);
            return id;
        }

        /// <summary>
        /// Manually set the <see cref="IdentifiableCategorization.Rule"/> of the <see cref="Identifiable.Id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rule"></param>
        public static void Categorize(this Identifiable.Id id, IdentifiableCategorization.Rule rule)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
            {
                CategorizeId(id, rule);
                return;
            }
            rules.Add(id, rule);
        }

        /// <summary>
        /// Remove all instances of an <see cref="Identifiable.Id"/> from every class in <see cref="Identifiable"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rule"></param>
        public static void Uncategorize(this Identifiable.Id id)
        {
            Identifiable.ALLERGY_FREE_CLASS.Remove(id);
            Identifiable.BOOP_CLASS.Remove(id);
            Identifiable.CHICK_CLASS.Remove(id);
            Identifiable.CRAFT_CLASS.Remove(id);
            Identifiable.EATERS_CLASS.Remove(id);
            Identifiable.ECHO_CLASS.Remove(id);
            Identifiable.ECHO_NOTE_CLASS.Remove(id);
            Identifiable.ELDER_CLASS.Remove(id);
            Identifiable.FASHION_CLASS.Remove(id);
            Identifiable.FOOD_CLASS.Remove(id);
            Identifiable.FRUIT_CLASS.Remove(id);
            Identifiable.GORDO_CLASS.Remove(id);
            Identifiable.LARGO_CLASS.Remove(id);
            Identifiable.LIQUID_CLASS.Remove(id);
            Identifiable.MEAT_CLASS.Remove(id);
            Identifiable.NON_SLIMES_CLASS.Remove(id);
            Identifiable.ORNAMENT_CLASS.Remove(id);
            Identifiable.PLORT_CLASS.Remove(id);
            Identifiable.SLIME_CLASS.Remove(id);
            Identifiable.STANDARD_CRATE_CLASS.Remove(id);
            Identifiable.TARR_CLASS.Remove(id);
            Identifiable.TOFU_CLASS.Remove(id);
            Identifiable.TOY_CLASS.Remove(id);
        }

        /// <summary>
        /// Check if an <see cref="Identifiable.Id"/> was registered by a mod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsModdedIdentifiable(this Identifiable.Id id) => moddedIdentifiables.ContainsKey(id);

        public static HashSet<Identifiable.Id> GetIdentifiablesForMod(string id) => moddedIdentifiables.Where(x => x.Value.ModInfo.Id == id).Select(x => x.Key).ToHashSet();

        internal static void CategorizeAllIds()
        {
            foreach (Identifiable.Id id in moddedIdentifiables.Keys)
            {
                if (rules.ContainsKey(id)) return;
                else CategorizeId(id);
                if (!FoodGroupRegistry.alreadyRegistered.Contains(id)) FoodGroupRegistry.RegisterToFoodGroup(id);
            }
            foreach (Identifiable.Id id in rules.Keys) id.Categorize(rules[id]);
        }

        /// <summary>
        /// Puts an <see cref="Identifiable.Id"/> into one of the vanilla categories based on its name postfix (see <see cref="LookupDirector"/>)
        /// </summary>
        /// <param name="id"></param>
        public static void CategorizeId(Identifiable.Id id)
        {
            string name = Enum.GetName(typeof(Identifiable.Id), id);
            if (name.EndsWith("_VEGGIE"))
                CategorizeId(id, IdentifiableCategorization.Rule.VEGGIE);
            else if (name.EndsWith("_FRUIT"))
                CategorizeId(id, IdentifiableCategorization.Rule.FRUIT);
            else if (name.EndsWith("_TOFU"))
                CategorizeId(id, IdentifiableCategorization.Rule.TOFU);
            else if (name.EndsWith("_SLIME"))
                CategorizeId(id, IdentifiableCategorization.Rule.SLIME);
            else if (name.EndsWith("_LARGO"))
                CategorizeId(id, IdentifiableCategorization.Rule.LARGO);
            else if (name.EndsWith("_GORDO"))
                CategorizeId(id, IdentifiableCategorization.Rule.GORDO);
            else if (name.EndsWith("_PLORT"))
                CategorizeId(id, IdentifiableCategorization.Rule.PLORT);
            else if (name.EndsWith("HEN") || name.EndsWith("ROOSTER"))
                CategorizeId(id, IdentifiableCategorization.Rule.MEAT);
            else if (name.EndsWith("CHICK"))
                CategorizeId(id, IdentifiableCategorization.Rule.CHICK);
            else if (name.EndsWith("_LIQUID"))
                CategorizeId(id, IdentifiableCategorization.Rule.LIQUID);
            else if (name.EndsWith("_CRAFT"))
                CategorizeId(id, IdentifiableCategorization.Rule.CRAFT);
            else if (name.EndsWith("_FASHION"))
                CategorizeId(id, IdentifiableCategorization.Rule.FASHION);
            else if (name.EndsWith("_ECHO"))
                CategorizeId(id, IdentifiableCategorization.Rule.ECHO);
            else if (name.StartsWith("ECHO_NOTE_"))
                CategorizeId(id, IdentifiableCategorization.Rule.ECHO_NOTE);
            else if (name.EndsWith("_ORNAMENT"))
                CategorizeId(id, IdentifiableCategorization.Rule.ORNAMENT);
            else if (name.EndsWith("_TOY") || id == Identifiable.Id.KOOKADOBA_BALL)
                CategorizeId(id, IdentifiableCategorization.Rule.TOY);
        }
        /// <summary>
        /// Put an <see cref="Identifiable.Id"/> into one of the vanilla categories
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        public static void CategorizeId(Identifiable.Id id, IdentifiableCategorization.Rule category)
        {
            string name = Enum.GetName(typeof(Identifiable.Id), id);
            if (name.Contains("TANGLE"))
            {
                Identifiable.ALLERGY_FREE_CLASS.Add(id);
            }
            if (category == IdentifiableCategorization.Rule.NONE) return;

            if ((category & (IdentifiableCategorization.Rule.VEGGIE | 
                IdentifiableCategorization.Rule.FRUIT | 
                IdentifiableCategorization.Rule.TOFU | 
                IdentifiableCategorization.Rule.PLORT | 
                IdentifiableCategorization.Rule.MEAT |
                IdentifiableCategorization.Rule.CHICK |
                IdentifiableCategorization.Rule.CRAFT)) != 0) Identifiable.NON_SLIMES_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.VEGGIE |
                IdentifiableCategorization.Rule.FRUIT |
                IdentifiableCategorization.Rule.TOFU |
                IdentifiableCategorization.Rule.MEAT)) != 0) Identifiable.FOOD_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.VEGGIE)) != 0) Identifiable.VEGGIE_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.FRUIT)) != 0) Identifiable.FRUIT_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.TOFU)) != 0) Identifiable.TOFU_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.SLIME)) != 0) Identifiable.SLIME_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.LARGO)) != 0) Identifiable.LARGO_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.GORDO)) != 0) Identifiable.GORDO_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.PLORT)) != 0) Identifiable.PLORT_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.MEAT)) != 0) Identifiable.MEAT_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.CHICK)) != 0) Identifiable.CHICK_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.LIQUID)) != 0) Identifiable.LIQUID_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.CRAFT)) != 0) Identifiable.CRAFT_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.FASHION)) != 0) Identifiable.FASHION_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.ECHO)) != 0) Identifiable.ECHO_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.ECHO_NOTE)) != 0) Identifiable.ECHO_NOTE_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.ORNAMENT)) != 0) Identifiable.ORNAMENT_CLASS.Add(id);
            if ((category & (IdentifiableCategorization.Rule.TOY)) != 0) Identifiable.TOY_CLASS.Add(id);

            Identifiable.EATERS_CLASS.UnionWith(Identifiable.SLIME_CLASS);
            Identifiable.EATERS_CLASS.UnionWith(Identifiable.LARGO_CLASS);
        }
    }
}

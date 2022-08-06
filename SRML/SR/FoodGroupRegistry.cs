using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
    public static class FoodGroupRegistry
    {
        internal static List<Identifiable.Id> alreadyRegistered = new List<Identifiable.Id>();
        internal static Dictionary<Func<Identifiable.Id, bool>, SlimeEat.FoodGroup> foodGroupRules = new Dictionary<Func<Identifiable.Id, bool>, SlimeEat.FoodGroup>()
        {
            { x => Identifiable.IsFruit(x), SlimeEat.FoodGroup.FRUIT },
            { x => x.ToString().Contains("GINGER"), SlimeEat.FoodGroup.GINGER },
            { x => Identifiable.IsAnimal(x), SlimeEat.FoodGroup.MEAT },
            { x => Identifiable.IsSlime(x), SlimeEat.FoodGroup.NONTARRGOLD_SLIMES },
            { x => Identifiable.IsPlort(x), SlimeEat.FoodGroup.PLORTS },
            { x => Identifiable.IsVeggie(x), SlimeEat.FoodGroup.VEGGIES }
        };

        /// <summary>
        /// Adds an <see cref="Identifiable.Id"/> to a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be added.</param>
        public static void RegisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id)
        {
            if (!SlimeEat.foodGroupIds.ContainsKey(foodGroup))
                SlimeEat.foodGroupIds.Add(foodGroup, new Identifiable.Id[] { id });
            else
                SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].AddToArray(id);

            alreadyRegistered.Add(id);
        }

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRange(this SlimeEat.FoodGroup foodGroup, ICollection<Identifiable.Id> idRange)
        {
            foreach (Identifiable.Id id in idRange)
                foodGroup.RegisterId(id);
        }

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRange(this SlimeEat.FoodGroup foodGroup, Identifiable.Id[] idRange)
        {
            foreach (Identifiable.Id id in idRange)
                foodGroup.RegisterId(id);
        }

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRangeToFoodGroup(this ICollection<Identifiable.Id> idRange)
        {
            foreach (Identifiable.Id id in idRange) 
                id.RegisterToFoodGroup();
        }

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRangeToFoodGroup(this Identifiable.Id[] idRange)
        {
            foreach (Identifiable.Id id in idRange) 
                id.RegisterToFoodGroup();
        }

        /// <summary>
        /// Removes an <see cref="Identifiable.Id"/> from a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to be removed from.</param>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be removed.</param>
        public static void UnregisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id) => 
            SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].Where(x => x != id).ToArray();

        /// <summary>
        /// Removes an <see cref="Identifiable.Id"/> from a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be removed.</param>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to be removed from.</param>
        public static void UnregisterFromFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) => UnregisterId(foodGroup, id);

        /// <summary>
        /// Adds an <see cref="Identifiable.Id"/> to a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be added.</param>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        public static void RegisterToFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) => RegisterId(foodGroup, id);

        /// <summary>
        /// Automatically adds an <see cref="Identifiable.Id"/> to a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be added.</param>
        public static void RegisterToFoodGroup(this Identifiable.Id id)
        {
            List<SlimeEat.FoodGroup> validGroups = foodGroupRules.Where(x => x.Key.Invoke(id)).Select(x => x.Value).ToList();

            foreach (SlimeEat.FoodGroup foodGroup in validGroups)
                RegisterToFoodGroup(id, foodGroup);
        }

        /// <summary>
        /// Registers a rule for what <see cref="SlimeEat.FoodGroup"/> to register a <see cref="Identifiable.Id"/> into.
        /// </summary>
        /// <param name="rule">The condition that the <see cref="Identifiable.Id"/> is registered into the <see cref="SlimeEat.FoodGroup"/></param>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to register an <see cref="Identifiable.Id"/> into.</param>
        public static void RegisterFoodGroupRule(Func<Identifiable.Id, bool> rule, SlimeEat.FoodGroup foodGroup) => foodGroupRules[rule] = foodGroup;
    }
}

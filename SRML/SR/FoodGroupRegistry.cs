using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
    [Obsolete]
    public static class FoodGroupRegistry
    {
        /// <summary>
        /// Adds an <see cref="Identifiable.Id"/> to a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be added.</param>
        public static void RegisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.Register(foodGroup, id);

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRange(this SlimeEat.FoodGroup foodGroup, ICollection<Identifiable.Id> idRange) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.RegisterRange(foodGroup, idRange);

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRange(this SlimeEat.FoodGroup foodGroup, Identifiable.Id[] idRange) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.RegisterRange(foodGroup, idRange.ToArray());

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRangeToFoodGroup(this ICollection<Identifiable.Id> idRange) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.RegisterRange(idRange);

        /// <summary>
        /// Registers a range of <see cref="Identifiable.Id"/>s into a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="idRange">The <see cref="Identifiable.Id"/>s to be added.</param>
        public static void RegisterIdRangeToFoodGroup(this Identifiable.Id[] idRange) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.RegisterRange(idRange.ToArray());

        /// <summary>
        /// Removes an <see cref="Identifiable.Id"/> from a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to be removed from.</param>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be removed.</param>
        public static void UnregisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.Deregister(foodGroup, id);

        /// <summary>
        /// Removes an <see cref="Identifiable.Id"/> from a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be removed.</param>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to be removed from.</param>
        public static void UnregisterFromFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.Deregister(foodGroup, id);

        /// <summary>
        /// Adds an <see cref="Identifiable.Id"/> to a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be added.</param>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to add to.</param>
        public static void RegisterToFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.Register(foodGroup, id);

        /// <summary>
        /// Automatically adds an <see cref="Identifiable.Id"/> to a <see cref="SlimeEat.FoodGroup"/>.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> to be added.</param>
        public static void RegisterToFoodGroup(this Identifiable.Id id) => API.Identifiable.Slime.FoodGroupRegistry.Instance.Register(id);

        /// <summary>
        /// Registers a rule for what <see cref="SlimeEat.FoodGroup"/> to register a <see cref="Identifiable.Id"/> into.
        /// </summary>
        /// <param name="rule">The condition that the <see cref="Identifiable.Id"/> is registered into the <see cref="SlimeEat.FoodGroup"/></param>
        /// <param name="foodGroup">The <see cref="SlimeEat.FoodGroup"/> to register an <see cref="Identifiable.Id"/> into.</param>
        public static void RegisterFoodGroupRule(Func<Identifiable.Id, bool> rule, SlimeEat.FoodGroup foodGroup) =>
            API.Identifiable.Slime.FoodGroupRegistry.Instance.RegisterFoodGroupRule(new Predicate<Identifiable.Id>(rule), foodGroup);
    }
}

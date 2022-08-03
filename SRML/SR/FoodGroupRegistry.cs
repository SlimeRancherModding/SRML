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

        public static void RegisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id)
        {
            if (!SlimeEat.foodGroupIds.ContainsKey(foodGroup))
                SlimeEat.foodGroupIds.Add(foodGroup, new Identifiable.Id[] { id });
            else
                SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].AddToArray(id);

            alreadyRegistered.Add(id);
        }

        public static void RegisterIdRange(this SlimeEat.FoodGroup foodGroup, Identifiable.Id[] idRange)
        {
            foreach (Identifiable.Id id in idRange) 
                foodGroup.RegisterId(id);
        }

        public static void RegisterIdRangeToFoodGroup(this ICollection<Identifiable.Id> idRange)
        {
            foreach (Identifiable.Id id in idRange) 
                id.RegisterToFoodGroup();
        }

        public static void RegisterIdRangeToFoodGroup(this Identifiable.Id[] idRange)
        {
            foreach (Identifiable.Id id in idRange) 
                id.RegisterToFoodGroup();
        }

        public static void UnregisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id) => 
            SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].Where(x => x != id).ToArray();
        
        public static void UnregisterFromFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) => UnregisterId(foodGroup, id);

        public static void RegisterToFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) => RegisterId(foodGroup, id);

        public static void RegisterToFoodGroup(this Identifiable.Id id)
        {
            List<SlimeEat.FoodGroup> validGroups = foodGroupRules.Where(x => x.Key.Invoke(id)).Select(x => x.Value).ToList();

            foreach (SlimeEat.FoodGroup foodGroup in validGroups)
                RegisterToFoodGroup(id, foodGroup);
        }

        public static void RegisterFoodGroupRule(Func<Identifiable.Id, bool> rule, SlimeEat.FoodGroup foodGroup) => foodGroupRules[rule] = foodGroup;
    }
}

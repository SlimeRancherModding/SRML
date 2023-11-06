using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Identifiable.Slime
{
    public class FoodGroupRegistry : EnumRegistry<FoodGroupRegistry, SlimeEat.FoodGroup>
    {
        protected Dictionary<SlimeEat.FoodGroup, Predicate<global::Identifiable.Id>> foodGroupRules = 
            new Dictionary<SlimeEat.FoodGroup, Predicate<global::Identifiable.Id>>()
        {
            { SlimeEat.FoodGroup.FRUIT, x => global::Identifiable.IsFruit(x) },
            { SlimeEat.FoodGroup.GINGER, x => x.ToString().Contains("GINGER") },
            { SlimeEat.FoodGroup.MEAT, x => global::Identifiable.IsAnimal(x) },
            { SlimeEat.FoodGroup.NONTARRGOLD_SLIMES, x => global::Identifiable.IsSlime(x) },
            { SlimeEat.FoodGroup.PLORTS, x => global::Identifiable.IsPlort(x) },
            { SlimeEat.FoodGroup.VEGGIES, x => global::Identifiable.IsVeggie(x) }
        };

        public override void Initialize()
        {
        }
        
        public virtual void AddRangeToFoodGroup(ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                AddToFoodGroup(id);
        }
        
        public virtual void AddRangeToFoodGroup(SlimeEat.FoodGroup foodGroup, ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                AddToFoodGroup(foodGroup, id);
        }

        public virtual void AddToFoodGroup(global::Identifiable.Id id)
        {
            foreach (var item in foodGroupRules)
            {
                if (item.Value.Invoke(id))
                    AddToFoodGroup(item.Key, id);
                else
                    RemoveFromFoodGroup(item.Key, id);
            }
        }

        public virtual void RemoveFromFoodGroups(global::Identifiable.Id id)
        {
            foreach (SlimeEat.FoodGroup foodGroup in (SlimeEat.FoodGroup[])Enum.GetValues(typeof(SlimeEat.FoodGroup)))
                RemoveFromFoodGroup(foodGroup, id);
        }

        public virtual void RemoveFromFoodGroup(SlimeEat.FoodGroup foodGroup, global::Identifiable.Id id)
        {
            if (SlimeEat.foodGroupIds.ContainsKey(foodGroup))
                SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].Where(x => x != id).ToArray();
        }

        public virtual void RegisterFoodGroupRule(Predicate<global::Identifiable.Id> func, SlimeEat.FoodGroup foodGroup) =>
            foodGroupRules[foodGroup] = func;

        public virtual void AddToFoodGroup(SlimeEat.FoodGroup foodGroup, global::Identifiable.Id id)
        {
            if (!SlimeEat.foodGroupIds.ContainsKey(foodGroup))
                SlimeEat.foodGroupIds[foodGroup] = new global::Identifiable.Id[0];

            SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].AddToArray(id);
        }
    }
}

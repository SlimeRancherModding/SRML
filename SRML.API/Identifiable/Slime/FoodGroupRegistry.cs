using HarmonyLib;
using SRML.Core.API;
using SRML.SR.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Identifiable.Slime
{
    public class FoodGroupRegistry : Registry<FoodGroupRegistry, SlimeEat.FoodGroup, global::Identifiable.Id>
    {
        internal static Dictionary<SlimeEat.FoodGroup, Predicate<global::Identifiable.Id>> foodGroupRules = 
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
        
        public void RegisterRange(ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                Register(id);
        }
        
        public void RegisterRange(SlimeEat.FoodGroup foodGroup, ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                Register(foodGroup, id);
        }

        public void Register(global::Identifiable.Id id)
        {
            foreach (var item in foodGroupRules)
            {
                if (item.Value.Invoke(id))
                    Register(item.Key, id);
                else
                    Deregister(item.Key, id);
            }
        }

        public void Deregister(global::Identifiable.Id id)
        {
            foreach (SlimeEat.FoodGroup foodGroup in (SlimeEat.FoodGroup[])Enum.GetValues(typeof(SlimeEat.FoodGroup)))
                Deregister(foodGroup, id);
        }

        public void Deregister(SlimeEat.FoodGroup foodGroup, global::Identifiable.Id id)
        {
            if (SlimeEat.foodGroupIds.ContainsKey(foodGroup))
                SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].Where(x => x != id).ToArray();
        }

        public void RegisterFoodGroupRule(Predicate<global::Identifiable.Id> func, SlimeEat.FoodGroup foodGroup) =>
            foodGroupRules[foodGroup] = func;

        public override bool IsRegistered(SlimeEat.FoodGroup foodGroup, global::Identifiable.Id id)
        {
            if (!SlimeEat.foodGroupIds.ContainsKey(foodGroup))
                return false;

            return SlimeEat.foodGroupIds[foodGroup].Contains(id);
        }

        public override void Register(SlimeEat.FoodGroup foodGroup, global::Identifiable.Id id)
        {
            if (!SlimeEat.foodGroupIds.ContainsKey(foodGroup))
                SlimeEat.foodGroupIds[foodGroup] = new global::Identifiable.Id[0];

            SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].AddToArray(id);
        }
    }
}

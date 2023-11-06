using System.Collections.Generic;

namespace SRML.API.Identifiable.Slime.Utils
{
    public static class FoodGroupExtensions
    {
        public static void Add(this SlimeEat.FoodGroup foodgroup, global::Identifiable.Id id) => 
            FoodGroupRegistry.Instance.AddToFoodGroup(foodgroup, id);
        public static void Remove(this SlimeEat.FoodGroup foodgroup, global::Identifiable.Id id) => 
            FoodGroupRegistry.Instance.RemoveFromFoodGroup(foodgroup, id);
        public static void AddRange(this SlimeEat.FoodGroup foodgroup, ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                foodgroup.Add(id);
        }
        public static void RemoveRange(this SlimeEat.FoodGroup foodgroup, ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                foodgroup.Remove(id);
        }

        public static void AddToFoodGroup(this global::Identifiable.Id id) => FoodGroupRegistry.Instance.AddToFoodGroup(id);
        public static void AddToFoodGroup(this global::Identifiable.Id id, SlimeEat.FoodGroup foodgroup) =>
            foodgroup.Add(id);
        public static void AddToFoodGroup(this ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                id.AddToFoodGroup();
        }
        public static void AddToFoodGroup(this ICollection<global::Identifiable.Id> ids, SlimeEat.FoodGroup foodgroup)
        {
            foreach (global::Identifiable.Id id in ids)
                foodgroup.Add(id);
        }
        public static void RemoveFromFoodGroups(this global::Identifiable.Id id) => FoodGroupRegistry.Instance.RemoveFromFoodGroups(id);
        public static void RemoveFromFoodGroup(this global::Identifiable.Id id, SlimeEat.FoodGroup foodgroup) =>
            foodgroup.Remove(id);
        public static void RemoveFromFoodGroups(this ICollection<global::Identifiable.Id> ids)
        {
            foreach (global::Identifiable.Id id in ids)
                id.RemoveFromFoodGroups();
        }
        public static void RemoveFromFoodGroup(this ICollection<global::Identifiable.Id> ids, SlimeEat.FoodGroup foodgroup)
        {
            foreach (global::Identifiable.Id id in ids)
                foodgroup.Remove(id);
        }
    }
}

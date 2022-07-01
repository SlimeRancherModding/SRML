using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
    public static class FoodGroupRegistry
    {
        internal static List<Identifiable.Id> alreadyRegistered = new List<Identifiable.Id>();

        internal static void CategorizeAllIds() => RegisterIdRangeToFoodGroup(IdentifiableRegistry.moddedIdentifiables.Keys.Where(x => !alreadyRegistered.Contains(x)).ToArray());

        public static void RegisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id)
        {
            if (!SlimeEat.foodGroupIds.ContainsKey(foodGroup)) SlimeEat.foodGroupIds.Add(foodGroup, new Identifiable.Id[] { id });
            else SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].AddToArray(id);
            alreadyRegistered.Add(id);
        }

        public static void RegisterIdRange(this SlimeEat.FoodGroup foodGroup, Identifiable.Id[] idRange)
        {
            foreach (Identifiable.Id id in idRange) foodGroup.RegisterId(id);
        }

        public static void RegisterIdRangeToFoodGroup(this Identifiable.Id[] idRange)
        {
            foreach (Identifiable.Id id in idRange) id.RegisterToFoodGroup();
        }

        public static void UnregisterId(this SlimeEat.FoodGroup foodGroup, Identifiable.Id id) => SlimeEat.foodGroupIds[foodGroup] = SlimeEat.foodGroupIds[foodGroup].Where(x => x != id).ToArray();
        
        public static void UnregisterFromFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) => UnregisterId(foodGroup, id);

        public static void RegisterToFoodGroup(this Identifiable.Id id, SlimeEat.FoodGroup foodGroup) => RegisterId(foodGroup, id);

        public static void RegisterToFoodGroup(this Identifiable.Id id)
        {
            if (Identifiable.IsFruit(id)) id.RegisterToFoodGroup(SlimeEat.FoodGroup.FRUIT);
            if (id.ToString().Contains("GINGER")) id.RegisterToFoodGroup(SlimeEat.FoodGroup.GINGER);
            if (Identifiable.IsAnimal(id)) id.RegisterToFoodGroup(SlimeEat.FoodGroup.MEAT);
            if (Identifiable.IsSlime(id) && !Identifiable.IsTarr(id)) id.RegisterToFoodGroup(SlimeEat.FoodGroup.NONTARRGOLD_SLIMES);
            if (Identifiable.IsPlort(id)) id.RegisterToFoodGroup(SlimeEat.FoodGroup.PLORTS);
            if (Identifiable.IsVeggie(id)) id.RegisterToFoodGroup(SlimeEat.FoodGroup.VEGGIES);
        }
    }
}

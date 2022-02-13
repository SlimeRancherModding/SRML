using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
	public static class FoodGroupRegistry
	{
		public static void Register(Identifiable.Id id)
		{
			if (Identifiable.IsSlime(id) && !Identifiable.IsTarr(id))
				Register(id, SlimeEat.FoodGroup.NONTARRGOLD_SLIMES);

			if (Identifiable.IsPlort(id))
				Register(id, SlimeEat.FoodGroup.PLORTS);

			if (Identifiable.IsVeggie(id))
				Register(id, SlimeEat.FoodGroup.VEGGIES);

			if (Identifiable.IsFruit(id))
				Register(id, SlimeEat.FoodGroup.FRUIT);

			if (Identifiable.MEAT_CLASS.Contains(id))
				Register(id, SlimeEat.FoodGroup.MEAT);
		}

		public static void Register(Identifiable.Id id, SlimeEat.FoodGroup group)
		{
			if (SlimeEat.foodGroupIds.ContainsKey(group))
			{
				if (!SlimeEat.foodGroupIds[group].Contains(id))
					SlimeEat.foodGroupIds[group] = SlimeEat.foodGroupIds[group].AddToArray(id);
			}
			else
				SlimeEat.foodGroupIds.Add(group, new Identifiable.Id[] { id });
		}

		public static void RegisterRange(Identifiable.Id[] ids, SlimeEat.FoodGroup group)
		{
			foreach (Identifiable.Id id in ids)
				Register(id, group);
		}

		public static void Add(this SlimeEat.FoodGroup group, Identifiable.Id id) => Register(id, group);
		public static void AddRange(this SlimeEat.FoodGroup group, Identifiable.Id[] ids) => RegisterRange(ids, group);
	}
}
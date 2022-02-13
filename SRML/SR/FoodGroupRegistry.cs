using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
	public static class FoodGroupRegistry
	{
		internal static Dictionary<SlimeEat.FoodGroup, HashSet<Identifiable.Id>> FOOD_GROUPS = new Dictionary<SlimeEat.FoodGroup, HashSet<Identifiable.Id>>();

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
			if (FOOD_GROUPS.ContainsKey(group))
			{
				if (!FOOD_GROUPS[group].Contains(id))
					FOOD_GROUPS[group].Add(id);
			}
			else
				FOOD_GROUPS.Add(group, new HashSet<Identifiable.Id>(Identifiable.idComparer) { id });
		}

		public static void RegisterRange(Identifiable.Id[] ids, SlimeEat.FoodGroup group)
		{
			foreach (Identifiable.Id id in ids)
				Register(id, group);
		}
	}
}
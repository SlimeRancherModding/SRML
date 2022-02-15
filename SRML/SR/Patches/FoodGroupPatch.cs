using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
namespace SRML.SR.Patches
{
	[HarmonyPatch(typeof(SlimeEat))]
	[HarmonyPatch("GetFoodGroupIds")]
	internal static class FoodGroupPatch
	{
		[HarmonyPriority(Priority.First)]
		public static void Postfix(SlimeEat __instance, ref Identifiable.Id[] __result, SlimeEat.FoodGroup group)
		{
			if (FoodGroupRegistry.FOOD_GROUPS.ContainsKey(group))
			{
				List<Identifiable.Id> foodGroupIds;
				if (__result != null)
					foodGroupIds = __result.ToList();
				else
					foodGroupIds = new List<Identifiable.Id>();

				foreach (Identifiable.Id id in FoodGroupRegistry.FOOD_GROUPS[group])
				{
					if (!foodGroupIds.Contains(id))
						foodGroupIds.Add(id);
				}
				__result = foodGroupIds.ToArray();
			}
		}
	}
}

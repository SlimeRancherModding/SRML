using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GordoSnare), "GetGordoIdForBait")]
    [HarmonyPriority(Priority.First)]
    internal static class GordoSnareGetGordoIdPatch
    {
        private static readonly Func<Zone, bool> HasAccessToZone = ZoneDirector.HasAccessToZone;

        public static bool Prefix(GordoSnare __instance, ref Identifiable.Id __result)
        {
            Dictionary<Identifiable.Id, float> dictionary = new Dictionary<Identifiable.Id, float>(Identifiable.idComparer);
            List<Identifiable.Id> normalIds = new List<Identifiable.Id>();
            List<Identifiable.Id> favIds = new List<Identifiable.Id>(); // To handle cases where multiple slimes favour the same id
            Identifiable.Id baitId = __instance.GetPrivateField<SnareModel>("model").baitTypeId;

            foreach (GameObject gordoEntry in GameContext.Instance.LookupDirector.GordoEntries)
            {
                GordoIdentifiable gordo = gordoEntry.GetComponent<GordoIdentifiable>();

                if (SnareRegistry.pinkLike.Contains(gordo.id) || !gordo.nativeZones.Any(HasAccessToZone))
                    continue;

                SlimeDiet diet = gordoEntry.GetComponent<GordoEat>().slimeDefinition.Diet;
                List<SlimeDiet.EatMapEntry> list2 = new List<SlimeDiet.EatMapEntry>();
                diet.AddEatMapEntries(baitId, list2);
                SlimeDiet.EatMapEntry eatMapEntry = list2.FirstOrDefault();

                if (eatMapEntry == null)
                    continue;

                (string message, List<Identifiable.Id> ids) = eatMapEntry.isFavorite ? ("Found favorite", favIds) : ("Adding potential", normalIds);
                Log.Debug(message, "gordo", gordo.id, "hasAccess", true); // Not using flag because the base game never even logs false here
                ids.Add(gordo.id);
            }

            if (normalIds.Count > 0)
            {
                float value = __instance.foodTypeSnareWeight / normalIds.Count;

                for (var j = 0; j < normalIds.Count; j++)
                    dictionary.Add(normalIds[j], value);
            }

            if (favIds.Count > 0)
            {
                float value = __instance.favoredFoodSnareWeight / favIds.Count;

                for (var j = 0; j < favIds.Count; j++)
                    dictionary.Add(favIds[j], value);
            }

            float value2 = __instance.pinkSnareWeight / SnareRegistry.pinkLike.Count;

            foreach (Identifiable.Id id in SnareRegistry.pinkLike)
                dictionary.Add(id, value2);

            Identifiable.Id pink = Randoms.SHARED.Pick(SnareRegistry.pinkLike);
            __result = Randoms.SHARED.Pick(dictionary, pink);
            return false;
        }
    }

    private static T Pick<T>(this Randoms random, HashSet<T> vals)
    {
        return vals.ElementAt(random.GetInt(vals.Count));
    }
}

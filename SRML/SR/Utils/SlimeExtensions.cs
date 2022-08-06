using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.SR.Utils
{
    public static class SlimeExtensions
    {
        public static SlimeAppearance GetAppearanceById(this SlimeDefinitions defs, Identifiable.Id id) => defs.GetSlimeByIdentifiableId(id)?.AppearancesDefault[0];

        public static SlimeDefinition GetDefinitionByAppearance(this SlimeDefinitions defs, SlimeAppearance appearance) => defs.Slimes.First(x => x.Appearances.Contains(appearance));

        public static SlimeAppearanceStructure Clone(this SlimeAppearanceStructure structure)
        {
            SlimeAppearanceStructure cloned = new SlimeAppearanceStructure(structure);
            cloned.Element.name = cloned.Element.name.Replace("(Clone)", string.Empty);
            return cloned;
        }

        public static GameObject GetPrefab(this SlimeDefinition def) => GameContext.Instance.LookupDirector.GetPrefab(def.IdentifiableId);

        public static SlimeDiet LoadLargoDiet(this SlimeDefinition def)
        {
            if (!def.IsLargo || def.BaseSlimes.Length == 0) return null;
            SlimeDiet diet = new SlimeDiet();

            diet.AdditionalFoods = new Identifiable.Id[0];
            diet.Favorites = new Identifiable.Id[0];
            diet.MajorFoodGroups = new SlimeEat.FoodGroup[0];
            diet.Produces = new Identifiable.Id[0];
            foreach (SlimeDefinition baseDef in def.BaseSlimes)
            {
                SlimeDiet baseDiet = baseDef.Diet;
                if (baseDiet.AdditionalFoods != null) diet.AdditionalFoods = diet.AdditionalFoods.Union(baseDiet.AdditionalFoods).ToArray();
                if (baseDiet.Favorites != null) diet.Favorites = diet.Favorites.Union(baseDiet.Favorites).ToArray();
                if (baseDiet.MajorFoodGroups != null) diet.MajorFoodGroups = diet.MajorFoodGroups.Union(baseDiet.MajorFoodGroups).ToArray();
                if (baseDiet.Produces != null) diet.Produces = diet.Produces.Concat(baseDiet.Produces).ToArray();
            }
            diet.FavoriteProductionCount = 2;

            def.Diet = diet;
            diet.RefreshEatMap(GameContext.Instance.SlimeDefinitions, def);
            return diet;
        }

        internal static Dictionary<Identifiable.Id, List<SlimeDiet.EatMapEntry>> extraEatEntries = new Dictionary<Identifiable.Id, List<SlimeDiet.EatMapEntry>>();

        public static void AddEatMapEntry(this SlimeDefinition def, SlimeDiet.EatMapEntry entry)
        {
            def.Diet.EatMap.Add(entry);
            if (!extraEatEntries.ContainsKey(def.IdentifiableId)) extraEatEntries[def.IdentifiableId] = new List<SlimeDiet.EatMapEntry>();
            extraEatEntries[def.IdentifiableId].Add(entry);
        }

        public static void RefreshEatmaps(this SlimeDefinitions defs)
        {
            foreach (SlimeDefinition def in defs.Slimes) def.Diet?.RefreshEatMap(defs, def);
        }
    }
}

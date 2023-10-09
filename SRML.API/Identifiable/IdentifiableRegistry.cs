using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SRML.Core.API;
using SRML.Core.ModLoader;
using static SECTR_AudioSystem;

namespace SRML.API.Identifiable
{
    [HarmonyPatch]
    public class IdentifiableRegistry : Registry<IdentifiableRegistry>
    {
        internal Dictionary<string, List<global::Identifiable.Id>> moddedIds = new Dictionary<string, List<global::Identifiable.Id>>();
        internal Dictionary<string, List<global::Identifiable>> moddedPrefabs = new Dictionary<string, List<global::Identifiable>>();

        public delegate void IdentifiableRegisterEvent(global::Identifiable identifiable);
        public delegate void IdentifiableIdRegisterEvent(global::Identifiable.Id id);
        public readonly IdentifiableRegisterEvent OnRegisterPrefab;
        public readonly IdentifiableIdRegisterEvent OnRegisterId;


        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        internal static void RegisterPrefabs(LookupDirector __instance)
        {
            Instance.RegisterIntoLookup(__instance);
        }

        public override void Initialize()
        {
        }

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var ident in Instance.moddedPrefabs.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.identifiablePrefabDict[ident.id] = ident.gameObject;
                lookupDirector.identifiablePrefabs.items.Add(ident.gameObject);
            }
        }

        public virtual void RegisterPrefab(global::Identifiable identifiable)
        {
            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedPrefabs.ContainsKey(executingId))
                moddedPrefabs[executingId] = new List<global::Identifiable>();

            moddedPrefabs[executingId].Add(identifiable);
            OnRegisterPrefab?.Invoke(identifiable);
        }

        public global::Identifiable.Id Create(string name) => Create(name, null);

        public global::Identifiable.Id Create(string name, object value)
        {
            if (value == null)
                value = EnumPatcher.AddEnumValue(typeof(global::Identifiable.Id), name);
            else
                EnumPatcher.AddEnumValue(typeof(global::Identifiable.Id), value, name);

            global::Identifiable.Id result = (global::Identifiable.Id)value;
            OnRegisterId?.Invoke(result);
            // TODO: Add processors, add categorization.

            return result;
        }

        /*public override void Initialize()
        {
            // hell
            base.Initialize();

            metadata.categorizationSuffixRules = new Dictionary<string, IdentifiableCategorizationAttribute.Rule>()
            {
                { "_VEGGIE", IdentifiableCategorizationAttribute.Rule.VEGGIE },
                { "_FRUIT", IdentifiableCategorizationAttribute.Rule.FRUIT },
                { "_TOFU", IdentifiableCategorizationAttribute.Rule.TOFU },
                { "_SLIME", IdentifiableCategorizationAttribute.Rule.SLIME },
                { "_LARGO", IdentifiableCategorizationAttribute.Rule.LARGO },
                { "_GORDO", IdentifiableCategorizationAttribute.Rule.GORDO },
                { "_PLORT", IdentifiableCategorizationAttribute.Rule.PLORT },
                { "HEN", IdentifiableCategorizationAttribute.Rule.MEAT },
                { "ROOSTER", IdentifiableCategorizationAttribute.Rule.MEAT },
                { "CHICK", IdentifiableCategorizationAttribute.Rule.CHICK },
                { "_LIQUID", IdentifiableCategorizationAttribute.Rule.LIQUID },
                { "_CRAFT", IdentifiableCategorizationAttribute.Rule.CRAFT },
                { "_FASHION", IdentifiableCategorizationAttribute.Rule.FASHION },
                { "_ECHO", IdentifiableCategorizationAttribute.Rule.ECHO },
                { "_ORNAMENT", IdentifiableCategorizationAttribute.Rule.ORNAMENT },
                { "_TOY", IdentifiableCategorizationAttribute.Rule.TOY }
            };
            metadata.categorizationPrefixRules = new Dictionary<string, IdentifiableCategorizationAttribute.Rule>()
            {
                { "ECHO_NOTE_", IdentifiableCategorizationAttribute.Rule.ECHO_NOTE },
                { "ELDER_", IdentifiableCategorizationAttribute.Rule.ELDER }
            };
            metadata.ruleLists = new Dictionary<IdentifiableCategorizationAttribute.Rule, List<HashSet<global::Identifiable.Id>>>()
            {
                { IdentifiableCategorizationAttribute.Rule.VEGGIE, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                    global::Identifiable.FOOD_CLASS, global::Identifiable.VEGGIE_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.FRUIT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                    global::Identifiable.FOOD_CLASS, global::Identifiable.FRUIT_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.MEAT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                    global::Identifiable.FOOD_CLASS, global::Identifiable.MEAT_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.TOFU, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                    global::Identifiable.FOOD_CLASS, global::Identifiable.TOFU_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.PLORT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                    global::Identifiable.PLORT_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.CHICK, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                    global::Identifiable.CHICK_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.CRAFT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.NON_SLIMES_CLASS,
                    global::Identifiable.CRAFT_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.SLIME, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.SLIME_CLASS,
                    global::Identifiable.EATERS_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.LARGO, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.LARGO_CLASS,
                    global::Identifiable.EATERS_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.GORDO, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.GORDO_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.LIQUID, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.LIQUID_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.FASHION, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.FASHION_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.ECHO, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ECHO_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.ECHO_NOTE, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ECHO_NOTE_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.ORNAMENT, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ORNAMENT_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.TOY, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.TOY_CLASS } },
                { IdentifiableCategorizationAttribute.Rule.ELDER, new List<HashSet<global::Identifiable.Id>>() { global::Identifiable.ELDER_CLASS } },
            };
            metadata.baseRuleLists = new List<HashSet<global::Identifiable.Id>>()
            {
                global::Identifiable.ALLERGY_FREE_CLASS,
                global::Identifiable.BOOP_CLASS,
                global::Identifiable.CHICK_CLASS,
                global::Identifiable.CRAFT_CLASS,
                global::Identifiable.EATERS_CLASS,
                global::Identifiable.ECHO_CLASS,
                global::Identifiable.ECHO_NOTE_CLASS,
                global::Identifiable.ELDER_CLASS,
                global::Identifiable.FASHION_CLASS,
                global::Identifiable.FOOD_CLASS,
                global::Identifiable.FRUIT_CLASS,
                global::Identifiable.GORDO_CLASS,
                global::Identifiable.LARGO_CLASS,
                global::Identifiable.LIQUID_CLASS,
                global::Identifiable.MEAT_CLASS,
                global::Identifiable.NON_SLIMES_CLASS,
                global::Identifiable.ORNAMENT_CLASS,
                global::Identifiable.PLORT_CLASS,
                global::Identifiable.SLIME_CLASS,
                global::Identifiable.STANDARD_CRATE_CLASS,
                global::Identifiable.TARR_CLASS,
                global::Identifiable.TOFU_CLASS,
                global::Identifiable.TOY_CLASS,
            };
            metadata.processors = new List<NameCategorizedEnumMetadata<global::Identifiable.Id, IdentifiableCategorizationAttribute.Rule>.CategorizationProcessor>()
            {
                (x, y, z) =>
                {
                    if (y.Contains("TANGLE"))
                    global::Identifiable.ALLERGY_FREE_CLASS.Add(x);
                },
                (x, y, z) =>
                {
                    if (y.Contains("TABBY"))
                        global::Identifiable.BOOP_CLASS.Add(x);
                },
                (x, y, z) =>
                {
                    if (y.Contains("TARR"))
                        global::Identifiable.TARR_CLASS.Add(x);
                },
            };
        }*/
    }
}

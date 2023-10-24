using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRML.API.Gadget.Attributes;
using SRML.Core.API.BuiltIn;
using SRML.Core.API.BuiltIn.Processors;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Gadget
{
    public class GadgetRegistry : EnumRegistry<GadgetRegistry, global::Gadget.Id>
    {
        protected Dictionary<global::Gadget.Id, BlueprintLock> blueprintLocks = new Dictionary<global::Gadget.Id, BlueprintLock>();
        protected List<global::Gadget.Id> defaultUnlocked = new List<global::Gadget.Id>();
        protected List<global::Gadget.Id> defaultAvail = new List<global::Gadget.Id>();
        protected Dictionary<string, List<GadgetDefinition>> moddedDefinitions = new Dictionary<string, List<GadgetDefinition>>();

        public delegate GadgetDirector.BlueprintLocker BlueprintLock(GadgetDirector director);
        public delegate void GadgetIdRegisterEvent(global::Gadget.Id id);
        public delegate void BlueprintLockRegisterEvent(global::Gadget.Id id, BlueprintLock blueprintLock);
        public delegate void GadgetDefinitionRegisterEvent(GadgetDefinition definition);

        public GadgetIdRegisterEvent OnRegisterDefaultUnlocked;
        public GadgetIdRegisterEvent OnRegisterDefaultAvailable;
        public BlueprintLockRegisterEvent OnRegisterBlueprintLock;
        public GadgetDefinitionRegisterEvent OnRegisterGadgetDefinition;

        public NameCategorizedEnumMetadata<global::Gadget.Id, GadgetCategorizationAttribute.Rule> Categorization
            { get; protected set; }

        [HarmonyPatch(typeof(GadgetsModel), "InitInitialBlueprints")]
        [HarmonyPrefix]
        internal static void AddToGadgetsModelPrefix(GadgetsModel __instance) => Instance.RegisterDefaultsIntoGadgetsModel(__instance);

        [HarmonyPatch(typeof(GadgetDirector), "InitBlueprintLocks")]
        [HarmonyPostfix]
        internal static void AddBlueprintLocksPostfix(GadgetDirector __instance) => Instance.RegisterLocksIntoGadgetDirector(__instance);

        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterPrefabs(LookupDirector __instance) => Instance.RegisterDefinitionsIntoLookup(__instance);

        public virtual void RegisterDefaultsIntoGadgetsModel(GadgetsModel model)
        {
            model.availBlueprints.UnionWith(defaultAvail);
            model.availBlueprints.UnionWith(defaultUnlocked);
            model.blueprints.UnionWith(defaultUnlocked);
        }
        public virtual void RegisterLocksIntoGadgetDirector(GadgetDirector director)
        {
            foreach (var bpLock in blueprintLocks)
                director.blueprintLocks[bpLock.Key] = bpLock.Value.Invoke(director);
        }
        public virtual void RegisterDefinitionsIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var def in Instance.moddedDefinitions.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.gadgetDefinitionDict[def.id] = def;
                lookupDirector.gadgetDefinitions.items.Add(def);
            }
        }

        public virtual void RegisterDefinition(GadgetDefinition definition)
        {
            if (definition.id == global::Gadget.Id.NONE)
                throw new ArgumentException("Attempting to register a gadget with id NONE. This is not allowed.");

            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedDefinitions.ContainsKey(executingId))
                moddedDefinitions[executingId] = new List<GadgetDefinition>();

            moddedDefinitions[executingId].Add(definition);
            OnRegisterGadgetDefinition?.Invoke(definition);
        }

        public virtual void RegisterBlueprintLock(global::Gadget.Id id, BlueprintLock blueprintLock)
        {
            blueprintLocks[id] = blueprintLock;
            OnRegisterBlueprintLock?.Invoke(id, blueprintLock);
        }
        public virtual void RegisterDefaultUnlockedBlueprint(global::Gadget.Id id)
        {
            defaultUnlocked.Add(id);
            OnRegisterDefaultUnlocked?.Invoke(id);
        }
        public virtual void RegisterDefaultAvailableBlueprint(global::Gadget.Id id)
        {
            defaultAvail.Add(id);
            OnRegisterDefaultUnlocked?.Invoke(id);
        }

        public override void Initialize()
        {
            Categorization.categorizationPrefixRules = new Dictionary<string, GadgetCategorizationAttribute.Rule>()
            {
                { "EXTRACTOR_", GadgetCategorizationAttribute.Rule.EXTRACTOR },
                { "TELEPORTER_", GadgetCategorizationAttribute.Rule.TELEPORTER },
                { "WARP_DEPOT_", GadgetCategorizationAttribute.Rule.WARP_DEPOT },
                { "ECHO_NET", GadgetCategorizationAttribute.Rule.ECHO_NET },
                { "LAMP_", GadgetCategorizationAttribute.Rule.LAMP },
                { "FASHION_POD_", GadgetCategorizationAttribute.Rule.FASHION_POD },
                { "GORDO_SNARE_", GadgetCategorizationAttribute.Rule.SNARE },
                { "DRONE", GadgetCategorizationAttribute.Rule.DRONE }
            };
            Categorization.ruleLists = new Dictionary<GadgetCategorizationAttribute.Rule, List<HashSet<global::Gadget.Id>>>()
            {
                { GadgetCategorizationAttribute.Rule.EXTRACTOR, new List<HashSet<global::Gadget.Id>>() { global::Gadget.EXTRACTOR_CLASS } },
                { GadgetCategorizationAttribute.Rule.TELEPORTER, new List<HashSet<global::Gadget.Id>>() { global::Gadget.TELEPORTER_CLASS } },
                { GadgetCategorizationAttribute.Rule.WARP_DEPOT, new List<HashSet<global::Gadget.Id>>() { global::Gadget.WARP_DEPOT_CLASS } },
                { GadgetCategorizationAttribute.Rule.ECHO_NET, new List<HashSet<global::Gadget.Id>>() { global::Gadget.ECHO_NET_CLASS } },
                { GadgetCategorizationAttribute.Rule.LAMP, new List<HashSet<global::Gadget.Id>>() { global::Gadget.LAMP_CLASS } },
                { GadgetCategorizationAttribute.Rule.FASHION_POD, new List<HashSet<global::Gadget.Id>>() { global::Gadget.FASHION_POD_CLASS } },
                { GadgetCategorizationAttribute.Rule.SNARE, new List<HashSet<global::Gadget.Id>>() { global::Gadget.SNARE_CLASS } },
                { GadgetCategorizationAttribute.Rule.DRONE, new List<HashSet<global::Gadget.Id>>() { global::Gadget.DRONE_CLASS } }
            };
            Categorization.baseRuleLists = new List<HashSet<global::Gadget.Id>>()
            {
                global::Gadget.EXTRACTOR_CLASS,
                global::Gadget.TELEPORTER_CLASS,
                global::Gadget.WARP_DEPOT_CLASS,
                global::Gadget.ECHO_NET_CLASS,
                global::Gadget.LAMP_CLASS,
                global::Gadget.FASHION_POD_CLASS,
                global::Gadget.SNARE_CLASS,
                global::Gadget.DRONE_CLASS
            };
        }
    }
}

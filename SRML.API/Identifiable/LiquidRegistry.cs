using HarmonyLib;
using SRML.Core.API;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Identifiable
{
    [HarmonyPatch]
    public class LiquidRegistry : Registry<LiquidRegistry>
    {
        internal Dictionary<string, List<LiquidDefinition>> moddedLiquids = new Dictionary<string, List<LiquidDefinition>>();

        public delegate void LiquidRegisterEvent(LiquidDefinition definition);
        public readonly LiquidRegisterEvent OnRegisterLiquid;


        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterLiquids(LookupDirector __instance) => Instance.RegisterIntoLookup(__instance);

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var liquid in Instance.moddedLiquids.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.liquidDict[liquid.id] = liquid;
                lookupDirector.liquidDefinitions.items.Add(liquid);
            }
        }

        public virtual void RegisterDefinition(LiquidDefinition definition)
        {
            if (definition.id == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a liquid definition with id NONE. This is not allowed.");

            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedLiquids.ContainsKey(executingId))
                moddedLiquids[executingId] = new List<LiquidDefinition>();

            moddedLiquids[executingId].Add(definition);
            OnRegisterLiquid?.Invoke(definition);
        }

        public override void Initialize()
        {
        }
    }
}

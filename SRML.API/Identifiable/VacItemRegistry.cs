using HarmonyLib;
using SRML.Core.API;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Identifiable
{
    [HarmonyPatch]
    public class VacItemRegistry : Registry<VacItemRegistry>
    {
        internal Dictionary<string, List<VacItemDefinition>> moddedVacItems = new Dictionary<string, List<VacItemDefinition>>();

        public delegate void VacItemRegisterEvent(VacItemDefinition definition);
        public readonly VacItemRegisterEvent OnRegisterVacItem;


        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterVacItems(LookupDirector __instance) => Instance.RegisterIntoLookup(__instance);

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var vacItem in Instance.moddedVacItems.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.vacItemDict[vacItem.id] = vacItem;
                lookupDirector.vacItemDefinitions.items.Add(vacItem);
            }
        }

        public virtual void RegisterDefinition(VacItemDefinition definition)
        {
            if (definition.id == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a vac item definition with id NONE. This is not allowed.");

            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedVacItems.ContainsKey(executingId))
                moddedVacItems[executingId] = new List<VacItemDefinition>();

            moddedVacItems[executingId].Add(definition);
            OnRegisterVacItem?.Invoke(definition);
        }

        public override void Initialize()
        {
        }
    }
}

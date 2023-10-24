using HarmonyLib;
using System.Linq;
using System;
using SRML.Core.API;
using SRML.Core.ModLoader;
using System.Collections.Generic;

namespace SRML.API.Identifiable.Slime
{
    [HarmonyPatch]
    public class ToyRegistry : Registry<ToyRegistry>
    {
        internal Dictionary<string, List<ToyDefinition>> moddedToys = new Dictionary<string, List<ToyDefinition>>();

        public delegate void ToyRegisterEvent(ToyDefinition definition);
        public readonly ToyRegisterEvent OnRegisterToy;

        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterPrefabs(LookupDirector __instance) => Instance.RegisterIntoLookup(__instance);

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var toy in Instance.moddedToys.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.toyDict[toy.toyId] = toy;
                lookupDirector.toyDefinitions.items.Add(toy);
            }
        }

        public virtual void RegisterDefinition(ToyDefinition toy)
        {
            if (toy.toyId == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a toy with id NONE. This is not allowed.");

            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedToys.ContainsKey(executingId))
                moddedToys[executingId] = new List<ToyDefinition>();

            moddedToys[executingId].Add(toy);
            OnRegisterToy?.Invoke(toy);
        }

        public void MarkToyAsBase(global::Identifiable.Id toyId)
        {
            if (toyId == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to process a toy with id NONE. This is not allowed.");

            ToyDirector.BASE_TOYS.Add(toyId);
            ToyDirector.UPGRADED_TOYS.Remove(toyId);
        }

        public void MarkToyAsUpgraded(global::Identifiable.Id toyId)
        {
            if (toyId == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to process a toy with id NONE. This is not allowed.");

            ToyDirector.BASE_TOYS.Remove(toyId);
            ToyDirector.UPGRADED_TOYS.Add(toyId);
        }

        public override void Initialize()
        {
        }
    }
}

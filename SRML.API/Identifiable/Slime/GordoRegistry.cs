using HarmonyLib;
using SRML.Core.API;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.API.Identifiable.Slime
{
    public class GordoRegistry : Registry<GordoRegistry>
    {
        internal Dictionary<string, List<GordoIdentifiable>> moddedGordos = new Dictionary<string, List<GordoIdentifiable>>();

        public delegate void GordoRegisterEvent(GordoIdentifiable identifiable);
        public readonly GordoRegisterEvent OnRegisterGordo;

        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        internal static void RegisterPrefabs(LookupDirector __instance) => Instance.RegisterIntoLookup(__instance);

        public virtual void RegisterIntoLookup(LookupDirector lookupDirector)
        {
            foreach (var gordo in Instance.moddedGordos.SelectMany(x => x.Value, (y, z) => z))
            {
                lookupDirector.gordoDict[gordo.id] = gordo.gameObject;
                lookupDirector.gordoEntries.items.Add(gordo.gameObject);
            }
        }

        public virtual void RegisterPrefab(GordoIdentifiable gordo)
        {
            if (gordo.id == global::Identifiable.Id.NONE)
                throw new ArgumentException("Attempting to register a gordo with id NONE. This is not allowed.");

            string executingId = CoreLoader.Instance.GetExecutingModContext().ModInfo.Id;
            if (!moddedGordos.ContainsKey(executingId))
                moddedGordos[executingId] = new List<GordoIdentifiable>();

            moddedGordos[executingId].Add(gordo);
            OnRegisterGordo?.Invoke(gordo);
        }

        public override void Initialize()
        {
        }
    }
}

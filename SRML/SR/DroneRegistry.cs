using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class DroneRegistry
    {
        internal static Dictionary<Identifiable.Id, SRMod> customBasicTarget = new Dictionary<Identifiable.Id, SRMod>();

        /// <summary>
        /// Register an <see cref="Identifiable.Id"/> as a drone target.
        /// </summary>
        /// <param name="id"></param>
        public static void RegisterBasicTarget(Identifiable.Id id)
        {
            customBasicTarget.Add(id, SRMod.GetCurrentMod());

            foreach(var v in GetMetadatas())
                v.targets = v.targets.AddToArray(new DroneMetadata.Program.Target.Basic(id));
        }

        static IEnumerable<DroneMetadata> GetMetadatas()
        {
            GameContext context;
            switch (SRModLoader.CurrentLoadingStep)
            {
                case SRModLoader.LoadingStep.PRELOAD:
                    context = GameObject.FindObjectOfType<GameContext>();
                    break;
                default:
                    context = GameContext.Instance;
                    break;
            }

            HashSet<DroneMetadata> metadataCache = new HashSet<DroneMetadata>();
            foreach(var v in Gadget.DRONE_CLASS)
            {
                if (!context.LookupDirector.gadgetDefinitionDict.ContainsKey(v)) continue;

                var data = context.LookupDirector.GetGadgetDefinition(v).prefab.GetComponentInChildren<DroneGadget>().metadata;
                if (metadataCache.Add(data)) yield return data;
            }
        }
    }
}

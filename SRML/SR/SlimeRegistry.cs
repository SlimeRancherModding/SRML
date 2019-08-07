using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class SlimeRegistry
    {
        internal static Dictionary<SlimeDefinition, SRMod> slimeDefinitions = new Dictionary<SlimeDefinition, SRMod>();
        public static void RegisterSlimeDefinition(SlimeDefinition definition)
        {
            slimeDefinitions[definition] = SRMod.GetCurrentMod();
            SlimeDefinitions definitions;
            switch (SRModLoader.CurrentLoadingStep)
            {
                case SRModLoader.LoadingStep.PRELOAD:
                    definitions = UnityEngine.Object.FindObjectOfType<GameContext>().SlimeDefinitions;
                    break;
                default:
                    definitions = GameContext.Instance.SlimeDefinitions;
                    break;
            }
            definitions.largoDefinitionByBasePlorts.Clear();
            definitions.largoDefinitionByBaseDefinitions.Clear();
            definitions.slimeDefinitionsByIdentifiable.Clear();
            definitions.Slimes = definitions.Slimes.Where(x => x.IdentifiableId != definition.IdentifiableId).ToArray();
            definitions.Slimes = definitions.Slimes.AddToArray(definition);
            definitions.OnEnable();

        }
    }
}

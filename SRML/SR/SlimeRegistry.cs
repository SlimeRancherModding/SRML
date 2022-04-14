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

        /// <summary>
        /// Register a slime definition in the <see cref="SlimeDefinitions"/> database
        /// </summary>
        /// <param name="definition">Slime definition to register</param>
        public static void RegisterSlimeDefinition(SlimeDefinition definition)
        {
            RegisterSlimeDefinition(definition, true);
        }

        /// <summary>
        /// Register a slime definition in the <see cref="SlimeDefinitions"/> database
        /// </summary>
        /// <param name="definition">Slime definition to register</param>
        public static void RegisterSlimeDefinition(SlimeDefinition definition, bool refreshEatMaps = true)
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
            if (definition.IsLargo && definition.BaseSlimes != null && definition.BaseSlimes.Length == 2 && definition.BaseSlimes[0].Diet.ProducePlorts() && definition.BaseSlimes[1].Diet.ProducePlorts())
            {
                SlimeDefinitions.PlortPair pair = new SlimeDefinitions.PlortPair(definition.BaseSlimes[0].Diet.Produces[0], definition.BaseSlimes[1].Diet.Produces[0]);
                definitions.largoDefinitionByBasePlorts = definitions.largoDefinitionByBasePlorts.Where(x => !x.Key.Equals(pair)).ToDictionary(x => x.Key, y => y.Value);
                definitions.largoDefinitionByBasePlorts.Add(pair, definition);
            }
            if (definition.IsLargo && definition.BaseSlimes != null && definition.BaseSlimes.Length == 2)
            {
                SlimeDefinitions.SlimeDefinitionPair pair = new SlimeDefinitions.SlimeDefinitionPair(definition.BaseSlimes[0], definition.BaseSlimes[1]);
                definitions.largoDefinitionByBaseDefinitions = definitions.largoDefinitionByBaseDefinitions.Where(x => !x.Key.Equals(pair)).ToDictionary(x => x.Key, y => y.Value);
                definitions.largoDefinitionByBaseDefinitions.Add(pair, definition);
            }
            definitions.slimeDefinitionsByIdentifiable.Add(definition.IdentifiableId, definition);
            definitions.Slimes = definitions.Slimes.Where(x => x.IdentifiableId != definition.IdentifiableId).ToArray();
            definitions.Slimes = definitions.Slimes.AddToArray(definition);
            if (refreshEatMaps)
            {
                definition.Diet.RefreshEatMap(definitions, definition);
                if (definition.BaseSlimes != null) foreach (SlimeDefinition child in definition.BaseSlimes)
                    child.Diet.RefreshEatMap(definitions, child);
            }
        }
    }
}

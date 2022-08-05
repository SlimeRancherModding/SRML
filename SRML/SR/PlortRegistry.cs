using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class PlortRegistry
    {
        internal static List<MarketUI.PlortEntry> plortsToPatch = new List<MarketUI.PlortEntry>();
        internal static List<EconomyDirector.ValueMap> valueMapsToPatch = new List<EconomyDirector.ValueMap>();

        /// <summary>
        /// Registers a plort entry into the Plort Market.
        /// </summary>
        /// <param name="entry">The plort's market entry.</param>
        public static void AddPlortEntry(MarketUI.PlortEntry entry) => plortsToPatch.Add(entry);

        /// <summary>
        /// Creates and registers a plort entry into the Plort Market.
        /// </summary>
        /// <param name="plortid">The <see cref="Identifiable.Id"/> belonging to the plort.</param>
        /// <param name="progressRequired">The progress required to show the plort as unlocked.</param>
        public static void AddPlortEntry(Identifiable.Id plortid, ProgressDirector.ProgressType[] progressRequired) => 
            AddPlortEntry(new MarketUI.PlortEntry() { id = plortid, toUnlock = progressRequired });

        /// <summary>
        /// Creates and registers a plort entry into the Plort Market that is automatically unlocked.
        /// </summary>
        /// <param name="plortId">The <see cref="Identifiable.Id"/> belonging to the plort.</param>
        public static void AddPlortEntry(Identifiable.Id plortId) => AddPlortEntry(plortId, new ProgressDirector.ProgressType[0]);

        /// <summary>
        /// Registers an economy entry into the Plort Market.
        /// </summary>
        /// <param name="map">The entry to register.</param>
        public static void AddEconomyEntry(EconomyDirector.ValueMap map) => valueMapsToPatch.Add(map);

        /// <summary>
        /// Creates and registers an economy entry into the Plort Market.
        /// </summary>
        /// <param name="accept">The <see cref="Identifiable"/> belonging to the plort.</param>
        /// <param name="value">The base value of the plort.</param>
        /// <param name="fullSaturation">How many plorts needed to saturate the market (cut the price in half).</param>
        public static void AddEconomyEntry(Identifiable accept, float value, float fullSaturation)
        {
            AddEconomyEntry(new EconomyDirector.ValueMap()
            {
                accept = accept,
                fullSaturation = fullSaturation,
                value = value
            });
        }

        /// <summary>
        /// Creates and registers an economy entry into the Plort Market.
        /// </summary>
        /// <param name="plortId">The <see cref="Identifiable.Id"/> belonging to the plort.</param>
        /// <param name="value">The base value of the plort.</param>
        /// <param name="fullSaturation">How many plorts needed to saturate the market (cut the price in half).</param>
        /// <exception cref="Exception">Throws if ran in PreLoad.</exception>
        public static void AddEconomyEntry(Identifiable.Id plortId, float value, float fullSaturation)
        {
            if (SRModLoader.CurrentLoadingStep == SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Could not locate Identifiable instance from given Identifiable.Id");
            AddEconomyEntry(new EconomyDirector.ValueMap()
            {
                accept = GameContext.Instance.LookupDirector.GetPrefab(plortId).GetComponent<Identifiable>(),
                fullSaturation = fullSaturation,
                value = value
            });
        }

        /// <summary>
        /// Creates and registers a plort entry, as well as an economy entry into the Plort Market.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> belonging to the plort.</param>
        /// <param name="value">The base value of the plort.</param>
        /// <param name="fullSaturation">How many plorts needed to saturate the market (cut the price in half).</param>
        public static void RegisterPlort(Identifiable.Id id, float value, float fullSaturationValue)
        {
            AddPlortEntry(id);
            AddEconomyEntry(id,value,fullSaturationValue);
        }
    }
}

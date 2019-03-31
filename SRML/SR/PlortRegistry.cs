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

        public static void AddPlortEntry(MarketUI.PlortEntry entry)
        {
            plortsToPatch.Add(entry);
        }

        public static void AddPlortEntry(Identifiable.Id plortid, ProgressDirector.ProgressType[] progressRequired)
        {
            AddPlortEntry(new MarketUI.PlortEntry(){id=plortid,toUnlock = progressRequired});
        }

        public static void AddPlortEntry(Identifiable.Id plortId)
        {
            AddPlortEntry(plortId,new ProgressDirector.ProgressType[0]);
        }

        public static void AddEconomyEntry(EconomyDirector.ValueMap map)
        {
            valueMapsToPatch.Add(map);
        }

        public static void AddEconomyEntry(Identifiable accept, float value, float fullSaturation)
        {
            AddEconomyEntry(new EconomyDirector.ValueMap()
            {
                accept = accept,
                fullSaturation = fullSaturation,
                value = value
            });
        }

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

        public static void RegisterPlort(Identifiable.Id id, float value, float fullSaturationValue)
        {
            AddPlortEntry(id);
            AddEconomyEntry(id,value,fullSaturationValue);
        }
    }
}

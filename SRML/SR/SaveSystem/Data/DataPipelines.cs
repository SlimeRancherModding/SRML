using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Data.Ammo;
using SRML.SR.SaveSystem.Data.Appearances;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Data.LandPlot;
using SRML.SR.SaveSystem.Data.Macro;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Data.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data
{
    static class DataPipelines
    {
        static DataPipelines()
        { 
            KeyValuePair<string, object> ConvertLocal<T>(KeyValuePair<string, T> x) => new KeyValuePair<string, object>(x.Key, x.Value);

            SimplePartialDataPipeline<T> GetSimplePipelineFromList<T>(Func<GameV12,IEnumerable<object>> listGen) where T : PartialData => new SimplePartialDataPipeline<T>($"{typeof(T).Name}_pipeline", listGen);
            SimplePartialDataPipeline<T> GetSimplePipelineFromDict<T>(Func<GameV12, IEnumerable<KeyValuePair<string,object>>> dictGen) where T : PartialData => new SimplePartialDataPipeline<T>($"{typeof(T).Name}_pipeline", (x)=>dictGen(x).Select(DataIdentifier.Convert));

            SimpleFullCustomDataPipeline<T> GetSimpleFullPipelineFromList<T>(Func<GameV12, IList<T>> listGen) where T : PersistedDataSet => new SimpleFullCustomDataPipeline<T>($"{typeof(T).Name}_pipeline", x => listGen(x).Select(y=> new KeyValuePair<DataIdentifier,T>(DataIdentifier.GetIdentifier(x,y),y)), (x, y, z) => listGen(x).Add(z), (x, y, z) => listGen(x).Remove(z));
            SimpleFullCustomDataPipeline<T> GetSimpleFullPipelineFromDict<T>(Func<GameV12, IDictionary<string,T>> listGen) where T : PersistedDataSet => new SimpleFullCustomDataPipeline<T>($"{typeof(T).Name}_pipeline", x => listGen(x).Select(y => new KeyValuePair<DataIdentifier, T>(DataIdentifier.GetIdentifier(x, y.Value), y.Value)), (x, y, z) => { if (ModdedStringRegistry.IsValidString(y.stringID)) listGen(x)[y.stringID] = z; }, (x, y, z) => listGen(x).Remove(y.stringID));

            SaveRegistry.RegisterPipeline(new PersistentAmmoDataPipeline());
            SaveRegistry.RegisterPipeline(new ExtendedData.SimpleExtendedDataPipeline("ActorExtendedData_pipeline", (x) => x.actors.Select(y => DataIdentifier.GetActorIdentifier(y.actorId))));

            SaveRegistry.RegisterPipeline(GetSimpleFullPipelineFromList(x => x.actors));
            SaveRegistry.RegisterPipeline(GetSimpleFullPipelineFromList(x => x.ranch.plots));
            SaveRegistry.RegisterPipeline(GetSimpleFullPipelineFromDict(x => x.world.placedGadgets));

            SaveRegistry.RegisterPipeline(GetSimplePipelineFromList<PartialActorData>((x) => x.actors));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromDict<PartialGadgetData>((x) => x.world.placedGadgets.Select(ConvertLocal)));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromDict<PartialTreasurePodData>((x) => x.world.treasurePods.Select(ConvertLocal)));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromDict<PartialGordoData>((x) => x.world.gordos.Select(ConvertLocal)));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromList<PartialLandPlotData>((x) => x.ranch.plots));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromList<PartialExchangeData>((x) => x.world.offers.Values));

            SaveRegistry.RegisterPipeline(new SimplePartialDataPipeline<PartialWorldData>("PartialWorldData_pipeline", x => new WorldV22[] { x.world }, priorityOffset:10));
            SaveRegistry.RegisterPipeline(new SimplePartialDataPipeline<PartialPlayerData>("PartialPlayerData_pipeline", x => new PlayerV14[] { x.player }, priorityOffset: 10));
            SaveRegistry.RegisterPipeline(new SimplePartialDataPipeline<PartialRanchData>("PartialRanchData_pipeline", x => new RanchV07[] { x.ranch }, priorityOffset: 10));
            SaveRegistry.RegisterPipeline(new SimplePartialDataPipeline<PartialAppearancesData>("PartialAppearancesData_pipeline", x => new AppearancesV01[] { x.appearances }, priorityOffset: 10));
            SaveRegistry.RegisterPipeline(new SimplePartialDataPipeline<PartialPediaData>("PartialPediaData_pipeline", x => new PediaV03[] { x.pedia }, priorityOffset: 10));

            SaveRegistry.RegisterPipeline(new PartialAmmoDataPipeline());

            SaveRegistry.RegisterPipeline(new ExtendedData.WorldDataPipeline());
        }
    }
}

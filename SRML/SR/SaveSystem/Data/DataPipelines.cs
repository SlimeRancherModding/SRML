using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Actor;
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

            SaveRegistry.RegisterPipeline(GetSimplePipelineFromList<PartialActorData>((x) => x.actors));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromDict<PartialGadgetData>((x) => x.world.placedGadgets.Select(ConvertLocal)));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromDict<PartialTreasurePodData>((x) => x.world.treasurePods.Select(ConvertLocal)));
            SaveRegistry.RegisterPipeline(GetSimplePipelineFromDict<PartialGordoData>((x) => x.world.gordos.Select(ConvertLocal)));

            SaveRegistry.RegisterPipeline(new SimplePartialDataPipeline<PartialWorldData>("PartialWorldData_pipeline", x => new WorldV22[] { x.world }, priorityOffset:10));


            SaveRegistry.RegisterPipeline(new ExtendedData.WorldDataPipeline());
        }
    }
}

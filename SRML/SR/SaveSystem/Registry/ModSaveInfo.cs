using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Data.LandPlot;
using SRML.SR.SaveSystem.Pipeline;
using UnityEngine;

namespace SRML.SR.SaveSystem.Registry
{
    public delegate void WorldDataPreLoadDelegate(CompoundDataPiece data);
    public delegate void WorldDataLoadDelegate(CompoundDataPiece data);
    public delegate void WorldDataSaveDelegate(CompoundDataPiece data);
    public class ModSaveInfo
    {
        DataRegistry<CustomActorData> CustomActorDataRegistry = new DataRegistry<CustomActorData>();
        DataRegistry<CustomGadgetData> CustomGadgetDataRegistry = new DataRegistry<CustomGadgetData>();
        DataRegistry<CustomLandPlotData> CustomLandPlotRegistry = new DataRegistry<CustomLandPlotData>();

        internal List<ISavePipeline> customPipelines = new List<ISavePipeline>();

        public readonly HashSet<DataRegistry> Registries = new HashSet<DataRegistry>();


        public delegate void ExtendedActorDataLoaded(ActorModel actor, GameObject obj, CompoundDataPiece data);

        public ExtendedActorDataLoaded onExtendedActorDataLoaded;

        public void OnExtendedActorDataLoaded(ActorModel model, GameObject obj, CompoundDataPiece piece)
        {
            onExtendedActorDataLoaded?.Invoke(model, obj, piece);
        }

        

        internal WorldDataPreLoadDelegate OnDataPreload;
        internal WorldDataLoadDelegate OnDataLoad;
        internal WorldDataSaveDelegate OnWorldSave;

        internal void WorldDataPreLoad(CompoundDataPiece tag) => OnDataPreload?.Invoke(tag);
        internal void WorldDataLoad(CompoundDataPiece tag) => OnDataLoad?.Invoke(tag);
        internal void WorldDataSave(CompoundDataPiece tag) => OnWorldSave?.Invoke(tag);

        public string ModID { get; }

        public ModSaveInfo(string modId)
        {
            ModID = modId;
            Registries.Add(CustomActorDataRegistry);
            Registries.Add(CustomGadgetDataRegistry);
            Registries.Add(CustomLandPlotRegistry);
        }

        public bool BelongsToMe(object b)
        {
            return Registries.Any((x) => x.BelongsToMe(b)) || (b.GetType().IsEnum&&ModdedIDRegistry.IsModdedID(b) && ModdedIDRegistry.ModForID(b).ModInfo.Id==ModID) || (b is string id && ModdedStringRegistry.IsValidModdedString(id) && ModdedStringRegistry.GetModForModdedString(id).ModInfo.Id==ModID);
        }

        public bool IsModelRegistered(Type model)
        {
            return Registries.Any((x) => x.IsModelRegistered(model));
        }

        public DataRegistry<T> GetRegistryFor<T>() where T : IDataRegistryMember
        {
            foreach (var r in Registries)
            {
                if (r is DataRegistry<T> reg) return reg;
            }
            return null;
        }

        public void RegisterCustomPipeline(ISavePipeline pipeline)
        {
            customPipelines.Add(pipeline);
        }
    }
}

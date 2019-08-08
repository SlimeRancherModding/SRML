using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Data.LandPlot;
using UnityEngine;

namespace SRML.SR.SaveSystem.Registry
{
    public delegate void WorldDataPreLoadDelegate(CompoundDataPiece data);
    public delegate void WorldDataLoadDelegate(CompoundDataPiece data);
    public delegate void WorldDataSaveDelegate(CompoundDataPiece data);
    internal class ModSaveInfo
    {
        DataRegistry<CustomActorData> CustomActorDataRegistry = new DataRegistry<CustomActorData>();
        DataRegistry<CustomGadgetData> CustomGadgetDataRegistry = new DataRegistry<CustomGadgetData>();
        DataRegistry<CustomLandPlotData> CustomLandPlotRegistry = new DataRegistry<CustomLandPlotData>();

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
        public ModSaveInfo()
        {
            Registries.Add(CustomActorDataRegistry);
            Registries.Add(CustomGadgetDataRegistry);
            Registries.Add(CustomLandPlotRegistry);
        }

        public bool BelongsToMe(object b)
        {
            return Registries.Any((x) => x.BelongsToMe(b));
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
    }
}

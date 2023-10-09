using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace SRML.SR
{
    public static class DataModelRegistry
    {
        public delegate ActorModel CreateActorDelegate(long actorid, Identifiable.Id ident, RegionRegistry.RegionSetId regionSetId, GameObject gameObj);
        internal static Dictionary<Predicate<Identifiable.Id>,CreateActorDelegate> actorOverrideMapping = new Dictionary<Predicate<Identifiable.Id>, CreateActorDelegate>();

        public delegate GadgetModel CreateGadgetDelegate(GadgetSiteModel site, GameObject gameObj);
        internal static Dictionary<Predicate<Gadget.Id>,CreateGadgetDelegate> gadgetOverrideMapping = new Dictionary<Predicate<Gadget.Id>, CreateGadgetDelegate>();

        public delegate LandPlotModel CreateLandPlotDelegate();
        internal static Dictionary<Predicate<LandPlot.Id>, CreateLandPlotDelegate> landPlotOverrides = new Dictionary<Predicate<LandPlot.Id>, CreateLandPlotDelegate>();

        public delegate void ActorDataSaveDelegate(ActorModel model, ActorDataV09 data);
        internal static Dictionary<Predicate<ActorModel>, ActorDataSaveDelegate> actorSavers = new Dictionary<Predicate<ActorModel>, ActorDataSaveDelegate>();

        public delegate void ActorDataLoadDelegate(ActorModel model, ActorDataV09 data);
        internal static Dictionary<Predicate<ActorModel>, ActorDataLoadDelegate> actorLoaders = new Dictionary<Predicate<ActorModel>, ActorDataLoadDelegate>();

        public static void RegisterActorModelOverride(Predicate<Identifiable.Id> pred, CreateActorDelegate creator)
        {
            actorOverrideMapping.Add(pred,creator);
        }

        public static void RegisterActorSaver(Predicate<ActorModel> pred, ActorDataSaveDelegate del)
        {
            actorSavers.Add(pred, del);
        }

        public static void RegisterActorLoader(Predicate<ActorModel> pred, ActorDataLoadDelegate del)
        {
            actorLoaders.Add(pred, del);
        }

        public static void RegisterCustomActorModel(Identifiable.Id id, CreateActorDelegate del)
        {
            RegisterActorModelOverride((x)=>x==id,del);
        }

        public static void RegisterCustomActorModel(Identifiable.Id id, Type actorType)
        {
            if (!typeof(ActorModel).IsAssignableFrom(actorType))
                throw new Exception("Given type is not a valid ActorModel!");
            RegisterCustomActorModel(id,(x,y,z,w)=>(ActorModel)Activator.CreateInstance(actorType,x,y,z,w.transform));
        }

        public static void RegisterLandPlotModelOverride(Predicate<LandPlot.Id> pred,CreateLandPlotDelegate creator)
        {
            landPlotOverrides.Add(pred, creator);
        }

        public static void RegisterGadgetModelOverride(Predicate<Gadget.Id> pred, CreateGadgetDelegate creator)
        {
            gadgetOverrideMapping.Add(pred,creator);
        }

        public static void RegisterCustomGadgetModel(Gadget.Id id, CreateGadgetDelegate g)
        {
            RegisterGadgetModelOverride((x)=>x==id,g);
        }

        public static void RegisterCustomGadgetModel(Gadget.Id id, Type gadgetType)
        {
            if (!typeof(GadgetModel).IsAssignableFrom(gadgetType))
                throw new Exception("Given type is not a valid GadgetModel!");
            RegisterCustomGadgetModel(id,(site,obj)=>(GadgetModel)Activator.CreateInstance(gadgetType,obj.GetComponent<Gadget>().id,site.id,obj.transform));
        }
    }
}

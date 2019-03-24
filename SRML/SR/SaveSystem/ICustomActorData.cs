using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
namespace SRML.SR.SaveSystem
{
    public interface ICustoasdmActorData<T> : Persistable where T:ActorModel
    {
        void PullCustomModel(T model);
        void PushCustomModel(T model);

        void WriteCustomData(BinaryWriter writer);
        void LoadCustomData(BinaryReader reader);

        Type GetModelType();

        VanillaActorData GetVanillaDataPortion();
    }

    internal class ActorDataWrapper<T> : CustomActorData<ActorModel> where T : ActorModel
    {
        public ActorDataWrapper(CustomActorData<T> wrapped)
        {
            wrappedObject = wrapped;
        }

        public CustomActorData<T> wrappedObject;

        public override void PullCustomModel(ActorModel model)
        {
            wrappedObject.PullCustomModel((T) model);
        }

        public override void PushCustomModel(ActorModel model)
        {
            wrappedObject.PushCustomModel((T) model);
        }

        public override void WriteCustomData(BinaryWriter writer)
        {
            wrappedObject.WriteCustomData(writer);
        }

        public override void LoadCustomData(BinaryReader reader)
        {
            wrappedObject.LoadCustomData(reader);
        }


        public override Type GetModelType()
        {
            return wrappedObject.GetModelType();
        }
    }

}

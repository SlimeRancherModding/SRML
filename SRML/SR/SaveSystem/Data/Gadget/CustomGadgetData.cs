using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV06;
namespace SRML.SR.SaveSystem.Data.Gadget
{
    internal abstract class CustomGadgetData<T> : CustomGadgetData where T : GadgetModel
    {

        public override Type GetModelType()
        {
            return typeof(T);
        }


        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

        public sealed override void PushCustomModel(GadgetModel model)
        {
            this.PushCustomModel((T)model);
        }

        public sealed override void PullCustomModel(GadgetModel model)
        {
            this.PullCustomModel((T)model);
        }

        public sealed override void Load(Stream stream, bool skipPayloadEnd)
        {
            base.Load(stream, false);
            var reader = new BinaryReader(stream);
            LoadCustomData(reader);
            if (!skipPayloadEnd) ReadDataPayloadEnd(reader);
        }

        public sealed override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            WriteDataPayloadEnd(writer);
            WriteCustomData(writer);
        }

    }
    public abstract class CustomGadgetData : VanillaGadgetData, IDataRegistryMember
    {
        public abstract void PullCustomModel(GadgetModel model);

        public abstract void PushCustomModel(GadgetModel model);

        public abstract void WriteCustomData(BinaryWriter writer);

        public abstract void LoadCustomData(BinaryReader reader);

        public abstract Type GetModelType();
    }
}

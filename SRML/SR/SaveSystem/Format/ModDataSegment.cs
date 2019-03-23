using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Format
{
    class ModDataSegment
    {
        public int version;
        public string modid;
        public List<ICustomActorData<ActorModel>> customActorData = new List<ICustomActorData<ActorModel>>();

        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
            modid = reader.ReadString();
            if (!(SRModLoader.GetMod(modid) is SRMod mod)) throw new Exception($"Unrecognized mod id: {modid}");
            var saveInfo = SaveRegistry.GetSaveInfo(mod);

            customActorData.Clear();
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                int version = reader.ReadInt32();
                int dataId = reader.ReadInt32();

                var newData = saveInfo.CustomActorDataRegistry.GetDataForID(dataId);

                newData.LoadData(reader);

                customActorData.Add(newData);
            }
        }
    }
}

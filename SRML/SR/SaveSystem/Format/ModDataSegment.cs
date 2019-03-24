using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using rail;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
using UnityEngine;

namespace SRML.SR.SaveSystem.Format
{
    class ModDataSegment
    {
        public int version;
        public string modid;
        public long byteLength;
        public List<CustomActorData<ActorModel>> customActorData = new List<CustomActorData<ActorModel>>();
        public List<VanillaActorData> normalActorData = new List<VanillaActorData>();
        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
            modid = reader.ReadString();
            byteLength = reader.ReadInt64();
            if (!(SRModLoader.GetMod(modid) is SRMod mod)) throw new Exception($"Unrecognized mod id: {modid}");
            var saveInfo = SaveRegistry.GetSaveInfo(mod);

            customActorData.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int dataId = reader.ReadInt32();

                var newData = saveInfo.CustomActorDataRegistry.GetDataForID(dataId);

                
                newData.Load(reader.BaseStream);

                customActorData.Add(newData);
            }

            normalActorData.Clear();

            count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var v = new VanillaActorData();
                v.Load(reader.BaseStream);
                normalActorData.Add(v);
            }
        }

        public void Write(BinaryWriter writer)
        {
            var start = writer.BaseStream.Position;
            writer.Write(version);
            writer.Write(modid);
            var overwritePosition = writer.BaseStream.Position;
            writer.Write((long)0);
            if (!(SRModLoader.GetMod(modid) is SRMod mod)) throw new Exception($"Unrecognized mod id: {modid}");
            var saveInfo = SaveRegistry.GetSaveInfo(mod);

            writer.Write(customActorData.Count);

            foreach (var v in customActorData)
            {
                writer.Write(saveInfo.CustomActorDataRegistry.GetIDForModel(v.GetModelType()));
                v.Write(writer.BaseStream);
            }

            writer.Write(normalActorData.Count);

            foreach (var v in normalActorData)
            {
                v.Write(writer.BaseStream);
            }

            var cur = writer.BaseStream.Position;
            writer.BaseStream.Seek(overwritePosition, SeekOrigin.Begin);
            writer.Write(cur-(overwritePosition+sizeof(long)));
            writer.BaseStream.Seek(cur, SeekOrigin.Begin);

        }
    }
}

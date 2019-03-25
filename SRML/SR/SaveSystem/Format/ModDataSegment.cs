using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using rail;
using SRML.SR.SaveSystem.Data.Actor;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
using UnityEngine;

namespace SRML.SR.SaveSystem.Format
{
    class ModDataSegment
    {
        public int version;
        public string modid;
        public long byteLength;
        public List<CustomActorData> customActorData = new List<CustomActorData>();
        public List<VanillaActorData> normalActorData = new List<VanillaActorData>();

        public List<ExtendedDataTree> extendedData = new List<ExtendedDataTree>();

        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
            modid = reader.ReadString();
            byteLength = reader.ReadInt64();
            if (!(SRModLoader.GetMod(modid) is SRMod mod)) throw new Exception($"Unrecognized mod id: {modid}");
            var saveInfo = SaveRegistry.GetSaveInfo(mod);
            var registry = saveInfo.GetRegistryFor<CustomActorData>();
            customActorData.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int dataId = reader.ReadInt32();

                var newData = registry.GetDataForID(dataId);


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

            extendedData.Clear();

            count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var e = new ExtendedDataTree();
                e.Read(reader);
                extendedData.Add(e);
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
            var registry = saveInfo.GetRegistryFor<CustomActorData>();
            foreach (var v in customActorData)
            {
                writer.Write(registry.GetIDForModel(v.GetModelType()));
                v.Write(writer.BaseStream);
            }

            writer.Write(normalActorData.Count);

            foreach (var v in normalActorData)
            {
                v.Write(writer.BaseStream);
            }

            writer.Write(extendedData.Count);

            foreach (var data in extendedData)
            {
                data.Write(writer);
            }

            var cur = writer.BaseStream.Position;
            writer.BaseStream.Seek(overwritePosition, SeekOrigin.Begin);
            byteLength = cur - (start);
            writer.Write(byteLength);
            writer.BaseStream.Seek(cur, SeekOrigin.Begin);

        }
    }
}

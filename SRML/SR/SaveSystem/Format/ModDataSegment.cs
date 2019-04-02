using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using rail;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Actor;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
using UnityEngine;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV06;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Registry;

namespace SRML.SR.SaveSystem.Format
{
    class ModDataSegment
    {
        public const int DATA_VERSION = 1;

        public int version;
        public string modid;
        public long byteLength;

        public List<IdentifiedData> identifiableData = new List<IdentifiedData>();

        public List<ExtendedDataTree> extendedData = new List<ExtendedDataTree>();

        public ModPlayerData playerData = new ModPlayerData();

        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
            modid = reader.ReadString();
            byteLength = reader.ReadInt64();
            if (!(SRModLoader.GetMod(modid) is SRMod mod)) throw new Exception($"Unrecognized mod id: {modid}");
            var saveInfo = SaveRegistry.GetSaveInfo(mod);

            identifiableData.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var e = new IdentifiedData();
                e.Read(reader,saveInfo);
                identifiableData.Add(e);
            }

            extendedData.Clear();
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var e = new ExtendedDataTree();
                e.Read(reader);
                extendedData.Add(e);
            }

            if (version >= 1)
            {
                playerData.Read(reader);
            }
        }

        public void Write(BinaryWriter writer)
        {
            version = DATA_VERSION;
            var start = writer.BaseStream.Position;
            writer.Write(version);
            writer.Write(modid);
            var overwritePosition = writer.BaseStream.Position;
            writer.Write((long)0);
            if (!(SRModLoader.GetMod(modid) is SRMod mod)) throw new Exception($"Unrecognized mod id: {modid}");
            var saveInfo = SaveRegistry.GetSaveInfo(mod);

            writer.Write(identifiableData.Count);
            foreach (var data in identifiableData)
            {
                data.Write(writer,saveInfo);
            }

            writer.Write(extendedData.Count);
            foreach (var data in extendedData)
            {
                data.Write(writer);
            }

            playerData.Write(writer);

            var cur = writer.BaseStream.Position;
            writer.BaseStream.Seek(overwritePosition, SeekOrigin.Begin);
            byteLength = cur - (start);
            writer.Write(byteLength);
            writer.BaseStream.Seek(cur, SeekOrigin.Begin);

        }

        
    }

    
}

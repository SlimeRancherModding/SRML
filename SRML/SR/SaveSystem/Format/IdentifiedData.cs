using System;
using System.IO;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Registry;
using UnityEngine;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV07;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV06;
using VanillaPlotData = MonomiPark.SlimeRancher.Persist.LandPlotV08;
namespace SRML.SR.SaveSystem.Format
{
    internal class IdentifiedData
    {
        public DataIdentifier dataID;
        public PersistedDataSet data;
        public bool IsCustomModel;
        public long dataLength;

        static IdentifiedData()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, IdentifiedData data) =>
                {
                    EnumTranslator.FixEnumValues(translator,mode,data.data);
                });
        }
        public void Read(BinaryReader reader, ModSaveInfo info)
        {
            dataLength = reader.ReadInt64();
            dataID = DataIdentifier.Read(reader);
            IsCustomModel = reader.ReadBoolean();
            switch (dataID.Type)
            {
                case IdentifierType.ACTOR:
                    ReadData<CustomActorData,VanillaActorData>(reader,info);
                    break;
                case IdentifierType.GADGET:
                    ReadData<CustomGadgetData,VanillaGadgetData>(reader,info);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(BinaryWriter writer, ModSaveInfo info)
        {
            long start = writer.BaseStream.Position;
            writer.Write(0L);
            IsCustomModel = info.BelongsToMe(data);
            DataIdentifier.Write(writer,dataID);
            writer.Write(IsCustomModel);
            switch (dataID.Type)
            {
                case IdentifierType.ACTOR:
                    WriteData<CustomActorData>(writer, info);
                    break;
                case IdentifierType.GADGET:
                    WriteData<CustomGadgetData>(writer, info);
                    break;
                default:
                    throw new NotImplementedException();
            }

            long current = writer.BaseStream.Position;

            writer.BaseStream.Seek(start, SeekOrigin.Begin);
            dataLength = current - start;
            writer.Write(dataLength);
            writer.BaseStream.Seek(current, SeekOrigin.Begin);
        }

        void ReadData<T, V>(BinaryReader reader, ModSaveInfo info) where T : V, IDataRegistryMember where V : PersistedDataSet, new()
        {
            if (!IsCustomModel)
            {
                data = new V();
                data.Load(reader.BaseStream);
            }
            else
            {
                var actorId = reader.ReadInt32();
                data = info.GetRegistryFor<T>().GetDataForID(actorId);

                data.Load(reader.BaseStream);
            }

        }

        void WriteData<T>(BinaryWriter writer, ModSaveInfo info) where T : PersistedDataSet, IDataRegistryMember
        {
            if (!IsCustomModel)
            {
                data.Write(writer.BaseStream);
            }
            else
            {
                writer.Write(info.GetRegistryFor<T>().GetIDForModel((data as IDataRegistryMember).GetModelType()));

                data.Write(writer.BaseStream);
            }
        }
    }
}

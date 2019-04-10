using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Ammo;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using UnityEngine;

namespace SRML.SR.SaveSystem.Format
{
    class ModdedSaveData
    {

        public const int DATA_VERSION = 2;
        public int version;
        public List<ModDataSegment> segments = new List<ModDataSegment>();

        public List<IdentifiableAmmoData> ammoDataEntries = new List<IdentifiableAmmoData>();

        public EnumTranslator enumTranslator;

        public Dictionary<DataIdentifier,PartialData> partialData = new Dictionary<DataIdentifier, PartialData>();

        public void ReadHeader(BinaryReader reader)
        {
            version = reader.ReadInt32();


        }

        public void WriteHeader(BinaryWriter writer)
        {
            writer.Write(DATA_VERSION);


        }

        public void ReadData(BinaryReader reader)
        {

            ammoDataEntries.Clear();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var newEntry = new IdentifiableAmmoData();
                newEntry.Read(reader);

                ammoDataEntries.Add(newEntry);
            }
            
            segments.Clear();
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                long start = reader.BaseStream.Position;
                var mod = new ModDataSegment();
                try
                {
                    mod.Read(reader);
                    segments.Add(mod);
                }
                catch(Exception e)
                {
                    Debug.Log($"Encountered exception {e}\nskipping loading {mod.modid} skipping {mod.byteLength} bytes in the stream");
                    reader.BaseStream.Seek(start + mod.byteLength,SeekOrigin.Begin);
                }
            }

            if (version >= 1)
            {
                enumTranslator = new EnumTranslator();
                enumTranslator.Read(reader);
                if (version >= 2)
                {
                    partialData.Clear();
                    count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        var id = DataIdentifier.Read(reader);
                        var dataType = DataIdentifier.IdentifierTypeToData[id.Type];
                        if(PartialData.TryGetPartialData(dataType,out var data))
                        {
                            data.Read(reader);
                            partialData[id] = data;
                        }
                        else Debug.LogError("No partial data for data identifier type "+id.Type);
                    }
                }
            }
        }

        public void WriteData(BinaryWriter writer)
        {
            writer.Write(ammoDataEntries.Count);
            foreach (var entry in ammoDataEntries)
            {
                entry.Write(writer);
            }
            writer.Write(segments.Count);
            foreach (var mod in segments)
            {
                mod.Write(writer);
                Debug.Log($"Saving mod {mod.modid} which is {mod.byteLength} bytes long");
            }

            enumTranslator.Write(writer);

            writer.Write(partialData.Count);

            foreach (var pair in partialData)
            {
                DataIdentifier.Write(writer,pair.Key);
                pair.Value.Write(writer);
            }
        }

        public void Write(BinaryWriter writer)
        {
            WriteHeader(writer);
            WriteData(writer);
        }

        public void Read(BinaryReader reader)
        {
            ReadHeader(reader);
            ReadData(reader);
        }

        public ModDataSegment GetSegmentForMod(SRMod mod)
        {
            if (!(segments.FirstOrDefault((x) => x.modid == mod.ModInfo.Id) is ModDataSegment seg))
            {
                var segment = new ModDataSegment();
                segment.modid = mod.ModInfo.Id;
                segments.Add(segment);
                return segment;
            }

            return seg;
        }

        public void InitializeEnumTranslator()
        {
            enumTranslator = SaveRegistry.GenerateEnumTranslator();
        }

        public void FixAllEnumValues(EnumTranslator.TranslationMode mode)
        {
            enumTranslator?.FixEnumValues(mode,partialData);
            foreach (var v in segments)
            {
                v.FixAllValues(enumTranslator,mode);
            }
        }
    }
}

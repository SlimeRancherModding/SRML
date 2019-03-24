using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRML.Utils;
using UnityEngine;

namespace SRML.SR.SaveSystem.Format
{
    class ModdedSaveData
    {
        public int version;
        public List<ModDataSegment> segments = new List<ModDataSegment>();
        public void ReadHeader(BinaryReader reader)
        {
            version = reader.ReadInt32();


        }

        public void WriteHeader(BinaryWriter writer)
        {
            writer.Write(version);


        }

        public void ReadData(BinaryReader reader)
        {
            segments.Clear();
            int count = reader.ReadInt32();
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
        }

        public void WriteData(BinaryWriter writer)
        {
            writer.Write(segments.Count);
            foreach (var mod in segments)
            {
                
                

                mod.Write(writer); //new BinaryWriter(container));
                Debug.Log($"Saving mod {mod.modid} which has {mod.normalActorData.Count} normal actors and is {mod.byteLength} bytes long");
                //writer.Write(//container.GetBuffer());

                //Debug.Log($"Encountered exception {e}\n skipping saving {mod.modid}");


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

        
    }
}

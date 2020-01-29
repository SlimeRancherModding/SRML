using SRML.SR.SaveSystem.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Pipeline
{
    public static class PipelineSerializer
    {
        public static void WritePipelineObject(BinaryWriter writer,ModSaveInfo info,IPipelineData item,bool writeMod)
        {
            if(writeMod)writer.Write(info.ModID);
            writer.Write(item.Pipeline.UniqueID);

            using (var tempStream = new MemoryStream())
            {
                item.Pipeline.Write(new BinaryWriter(tempStream), info, item);
                writer.Write(tempStream.Position);
                tempStream.Position = 0;
                tempStream.CopyTo(writer.BaseStream);
            }
        }


        public static IPipelineData ReadPipelineObject(BinaryReader reader, IEnumerable<ISavePipeline> candidatePipelines, string mod)
        {
            var pipeline = reader.ReadString();

            var size = reader.ReadInt64();

            if (!candidatePipelines.Any(x => x.UniqueID == pipeline) || !SRModLoader.IsModPresent(mod))
            {
                reader.BaseStream.Position += size;
                throw new Exception($"Missing Mod: {mod} or missing data pipeline: {pipeline}, skipping {size} bytes...");
                
            }
            return candidatePipelines.First(x => x.UniqueID == pipeline).Read(reader, SaveRegistry.GetSaveInfo(mod));
        }
        public static IPipelineData ReadPipelineObject(BinaryReader reader,IEnumerable<ISavePipeline> candidatePipelines)
        {
            var mod = reader.ReadString();
            return ReadPipelineObject(reader, candidatePipelines, mod);
        }
    }
}

using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game = MonomiPark.SlimeRancher.Persist.GameV12;

namespace SRML.SR.SaveSystem.Pipeline
{
    public delegate void AddBackDelegate();

    

    public interface ISavePipeline
    {
        string UniqueID { get; }
        
        int PullPriority { get; }

        bool OurData(IPipelineData data);

        IEnumerable<IPipelineData> Pull(ModSaveInfo mod,Game data);
        void Push(ModSaveInfo mod, Game data, IPipelineData item);

        void RemoveExtraModdedData(ModSaveInfo mod, Game data);


        void Write(BinaryWriter writer, ModSaveInfo info, IPipelineData item);
        IPipelineData Read(BinaryReader reader, ModSaveInfo info);

    }

    public abstract class SavePipeline<T>: ISavePipeline where T : IPipelineData
    {
        public abstract string UniqueID { get; }

        public abstract int LatestVersion { get; }

        public int Version { get; protected set; }

        public virtual int PullPriority => 1000;

        public bool OurData(IPipelineData data) => data is T;

        public abstract IEnumerable<IPipelineData> Pull(ModSaveInfo mod, Game data);

        public void Push(ModSaveInfo mod, Game data, IPipelineData item) => PushData(mod, data, (T)item);

        protected abstract void PushData(ModSaveInfo mod, Game data, T item);

        public IPipelineData Read(BinaryReader reader, ModSaveInfo info)
        {
            Version = reader.ReadInt32();
            return ReadData(reader, info);
        }

        public abstract T ReadData(BinaryReader reader, ModSaveInfo info);
        public abstract void RemoveExtraModdedData(ModSaveInfo mod, Game data);

        public void Write(BinaryWriter writer, ModSaveInfo info, IPipelineData item)
        {
            writer.Write(LatestVersion);
            WriteData(writer, info, (T)item);
        }
        protected abstract void WriteData(BinaryWriter writer, ModSaveInfo info, T item);

        
    }

    public interface IPipelineData
    {
        ISavePipeline Pipeline {get;}
    }

    public abstract class PipelineData : IPipelineData
    {
        public ISavePipeline Pipeline { get; }
        public PipelineData(ISavePipeline pipeline)
        {
            Pipeline = pipeline;
        }
    }
}

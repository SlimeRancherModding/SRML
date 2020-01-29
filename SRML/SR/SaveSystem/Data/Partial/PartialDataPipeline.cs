using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Actor;
using SRML.SR.SaveSystem.Pipeline;
using SRML.SR.SaveSystem.Registry;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Partial
{
    public abstract class PartialDataPipeline<T>: SavePipeline<IdentifiedPartialData<T>> where T : PartialData
    {
        public override int PullPriority => 2000;
        public abstract IEnumerable<KeyValuePair<DataIdentifier, object>> GetAllRelevantObjects(GameV12 data);
        private IEnumerable<KeyValuePair<DataIdentifier,object>> GetAllRelevantObjectsForMod(GameV12 data, ModSaveInfo mod)
        {
            var list = GetAllRelevantObjects(data).Where(x=>CustomChecker.GetCustomLevel(x.Value)==CustomChecker.CustomLevel.PARTIAL).ToList();
            return list.Where(x =>
            {

                var tempCont = SRMod.GetCurrentMod();
                SRMod.ClearModContext();
                SRMod.ForceModContext(SRModLoader.GetMod(mod.ModID));
                bool result = CustomChecker.GetCustomLevel(x.Value)==CustomChecker.CustomLevel.PARTIAL;
                SRMod.ClearModContext();
                SRMod.ForceModContext(tempCont);
                return result;
            });
        }
        public override IEnumerable<IPipelineData> Pull(ModSaveInfo mod, GameV12 data)
        {   

            var objs = GetAllRelevantObjectsForMod(data, mod);
            SRMod.ForceModContext(SRModLoader.GetMod(mod.ModID));
            foreach(var v in objs)
            {
                Debug.Log(mod.ModID + " " + v);
                var partial = PartialData.GetPartialData(v.Value.GetType());
                partial.Pull(v.Value);
                yield return new IdentifiedPartialData<T>(this, v.Key, partial as T);
                

            }
            SRMod.ClearModContext();
        }

        public override IPipelineData Read(BinaryReader reader, ModSaveInfo info)
        {
            var id = DataIdentifier.Read(reader);
            var dataType = DataIdentifier.IdentifierTypeToData[id.Type];
            if (PartialData.TryGetPartialData(dataType, out var data))
            {
                data.Read(reader);
                Debug.Log(data);
                return new IdentifiedPartialData<T>(this, id, data as T);
            }
            throw new Exception();
        }

        public override void RemoveExtraModdedData(ModSaveInfo mod, GameV12 data)
        {
            return;
            var objs = GetAllRelevantObjectsForMod(data, mod);
            SRMod.ForceModContext(SRModLoader.GetMod(mod.ModID));
            foreach (var v in objs)
            {
                var h = v;
                var partial = PartialData.GetPartialData(h.GetType());
                partial.Pull(h);
            }
            SRMod.ClearModContext();
        }

        protected override void PushData(ModSaveInfo mod, GameV12 data, IdentifiedPartialData<T> item)
        {
            var v = GetAllRelevantObjects(data).FirstOrDefault(x => x.Key == item.Identifier);
            if (v.Value == null) return;
            item.Data.Push(v.Value);
        }

        protected override void WriteData(BinaryWriter writer, ModSaveInfo info, IdentifiedPartialData<T> item)
        {
            DataIdentifier.Write(writer,item.Identifier);
            item.Data.Write(writer);
        }
        
    }
    public class IdentifiedPartialData<T> : PipelineData,IdentifiedPartialData where T : PartialData
    {
        public DataIdentifier Identifier { get; set; }
        public T Data;
        public IdentifiedPartialData(ISavePipeline pipeline, DataIdentifier id, T data) : base(pipeline)
        {
            Identifier = id;
            Data = data;
        }


        PartialData IdentifiedPartialData.PartialData => Data;

        static IdentifiedPartialData()
        {
            EnumTranslator.RegisterEnumFixer<IdentifiedPartialData>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.PartialData);
            });
        }
    }

    public interface IdentifiedPartialData
    {
        DataIdentifier Identifier { get; set; }
        PartialData PartialData { get; }
    }

    public class SimplePartialDataPipeline<T> : PartialDataPipeline<T> where T : PartialData
    {
        private Func<GameV12, IEnumerable<KeyValuePair<DataIdentifier, object>>> generatorFunc;

        public override string UniqueID { get; }

        public override int PullPriority => base.PullPriority + priorityOffset;
        int priorityOffset;
        public override IEnumerable<KeyValuePair<DataIdentifier, object>> GetAllRelevantObjects(GameV12 data)=>generatorFunc(data);
        public SimplePartialDataPipeline(string id, Func<GameV12, IEnumerable<KeyValuePair<DataIdentifier, object>>> generator,int priorityOffset = 0)
        {
            UniqueID = id;
            generatorFunc = generator;
            this.priorityOffset = priorityOffset;
        }
        public SimplePartialDataPipeline(string id, Func<GameV12, IEnumerable<object>> generator, int priorityOffset = 0) :this(id,(x)=>generator(x).Select(y=>new KeyValuePair<DataIdentifier,object>(DataIdentifier.GetIdentifier(x,y),y)),priorityOffset)
        {
        }

    }
}

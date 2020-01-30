using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Format;
using SRML.SR.SaveSystem.Pipeline;
using SRML.SR.SaveSystem.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data
{
    public abstract class FullCustomDataPipeline<T> : SavePipeline<IdentifiedData> where T : PersistedDataSet
    {
        public override int PullPriority => base.PullPriority-100;

        public abstract IEnumerable<KeyValuePair<DataIdentifier, T>> GetAllRelevantObjects(GameV12 game);
        public abstract void InsertObject(GameV12 game, DataIdentifier identifier, T obj);
        public abstract void RemoveObject(GameV12 game, DataIdentifier identifier, T obj);

        public override IEnumerable<IPipelineData> Pull(ModSaveInfo mod, GameV12 data)
        {
            foreach(var g in GetAllRelevantObjects(data))
            {
                if(SaveRegistry.ModForData(g.Value) is SRMod modData && modData.ModInfo.Id == mod.ModID)
                {
                    yield return new IdentifiedData(this) { dataID = g.Key, data = g.Value };
                }
            }
        }

        public override IdentifiedData ReadData(BinaryReader reader, ModSaveInfo info)
        {
            var id = new IdentifiedData(this);
            id.Read(reader, info);
            return id;
        }

        public override void RemoveExtraModdedData(ModSaveInfo mod, GameV12 data)
        {
            var list = GetAllRelevantObjects(data).ToList();
            foreach (var g in list)
            {
                if (SaveRegistry.ModForData(g.Value) is SRMod modData && modData.ModInfo.Id == mod.ModID)
                {
                    RemoveObject(data, DataIdentifier.GetIdentifier(data, g.Value), g.Value);
                }
            }
        }

        protected override void PushData(ModSaveInfo mod, GameV12 data, IdentifiedData item)
        {
            InsertObject(data, item.dataID, item.data as T);
        }

        protected override void WriteData(BinaryWriter writer, ModSaveInfo info, IdentifiedData item)
        {
            item.Write(writer, info);
        }
    }

    public class SimpleFullCustomDataPipeline<T> : FullCustomDataPipeline<T> where T : PersistedDataSet
    {
        public override string UniqueID { get; }

        public override int LatestVersion => 0;

        Func<GameV12, IEnumerable<KeyValuePair<DataIdentifier, T>>> getRelevantObjects;
        Action<GameV12, DataIdentifier, T> insertObject;
        Action<GameV12, DataIdentifier, T> removeObject;

        public SimpleFullCustomDataPipeline(string uniqueID, Func<GameV12, IEnumerable<KeyValuePair<DataIdentifier, T>>> getRelevantObjects, Action<GameV12, DataIdentifier, T> insertObject, Action<GameV12, DataIdentifier, T> removeObject)
        {
            this.UniqueID = uniqueID;
            this.getRelevantObjects = getRelevantObjects;
            this.insertObject = insertObject;
            this.removeObject = removeObject;
        }

        public override IEnumerable<KeyValuePair<DataIdentifier, T>> GetAllRelevantObjects(GameV12 game)
        {
            return getRelevantObjects(game);
        }

        public override void InsertObject(GameV12 game, DataIdentifier identifier, T obj)
        {
            insertObject(game, identifier, obj);
        }

        public override void RemoveObject(GameV12 game, DataIdentifier identifier, T obj)
        {
            removeObject(game, identifier, obj);
        }
    }
}

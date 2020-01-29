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
    public abstract class FullCustomDataPipeline : SavePipeline<IdentifiedData>
    {
        public override int PullPriority => base.PullPriority-100;

        public abstract IEnumerable<KeyValuePair<DataIdentifier, object>> GetAllRelevantObjects(GameV12 game);
        public abstract void InsertObject(GameV12 game, DataIdentifier identifier, object obj);
        public abstract void RemoveObject(GameV12 game, DataIdentifier identifier, object obj);

        public override IEnumerable<IPipelineData> Pull(ModSaveInfo mod, GameV12 data)
        {
            foreach(var g in GetAllRelevantObjects(data))
            {
                if(SaveRegistry.ModForData(g) is SRMod modData && modData.ModInfo.Id == mod.ModID)
                {

                }
            }
        }

        public override IPipelineData Read(BinaryReader reader, ModSaveInfo info)
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
                if (SaveRegistry.ModForData(g) is SRMod modData && modData.ModInfo.Id == mod.ModID)
                {
                    RemoveObject(data, DataIdentifier.GetIdentifier(data, g), g);
                }
            }
        }

        protected override void PushData(ModSaveInfo mod, GameV12 data, IdentifiedData item)
        {
            
        }

        protected override void WriteData(BinaryWriter writer, ModSaveInfo info, IdentifiedData item)
        {
            item.Write(writer, info);
        }
    }
}

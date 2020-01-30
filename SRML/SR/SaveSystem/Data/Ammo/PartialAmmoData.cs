using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Pipeline;
using SRML.SR.SaveSystem.Registry;
using SRML.SR.SaveSystem.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static SRML.SR.SaveSystem.Data.Ammo.PartialAmmoDataPipeline;

namespace SRML.SR.SaveSystem.Data.Ammo
{
    public class PartialAmmoData : VersionedPartialData<List<AmmoDataV02>>
    {
        public override int LatestVersion => 0;

        public PartialList<AmmoDataV02> ammo = new PartialList<AmmoDataV02>(x => ModdedIDRegistry.IsModdedID(x.id), PartialDataUtils.CreateSerializerForPersistedDataSet<AmmoDataV02>(), AmmoDataUtils.CreateEmpty, checkIfValid: x => ModdedIDRegistry.IsModdedID(x.id));

        public override void Pull(List<AmmoDataV02> data)
        {
            ammo.Pull(data);
        }

        public override void Push(List<AmmoDataV02> data)
        {
            ammo.Push(data);
        }

        public override void ReadData(BinaryReader reader)
        {
            ammo.Read(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            ammo.Write(writer);
        }

        static PartialAmmoData()
        {
            PartialData.RegisterPartialData<List<AmmoDataV02>, PartialAmmoData>();
            CustomChecker.RegisterCustomChecker<List<AmmoDataV02>>((x) =>
            {
                if (AmmoDataUtils.HasCustomData(x)) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
            EnumTranslator.RegisterEnumFixer((EnumTranslator translator, EnumTranslator.TranslationMode mode, AmmoDataV02 data) =>
            {
                data.id = translator.TranslateEnum(mode, data.id);
                translator.FixEnumValues(mode, data.emotionData.emotionData);
            });
            EnumTranslator.RegisterEnumFixer<PartialAmmoData>((trans,mode,obj) =>
            {
                trans.FixEnumValues(mode, obj.ammo);
            });

        }
    }
    public class PartialAmmoDataPipeline : SavePipeline<AmmoPipelineData>
    {
        public override string UniqueID => "PartialAmmoData_pipline";

        public override int PullPriority => base.PullPriority * 5;

        public override int LatestVersion => 0;

        public override IEnumerable<IPipelineData> Pull(ModSaveInfo mod, GameV12 data)
        {
            SRMod.ForceModContext(SRModLoader.GetMod(mod.ModID));
            foreach(var v in AmmoDataUtils.GetAllAmmoData(data))
            {
                if (!AmmoDataUtils.HasCustomData(v)) continue;
                if (!AmmoIdentifier.TryGetIdentifier(v, data, out var ammoIdentifier)) continue;
                var ammo = new PartialAmmoData();
                ammo.Pull(v);
                yield return new AmmoPipelineData(this)
                {
                    Ammo = ammo,
                    Identifier = ammoIdentifier
                };
            }
            SRMod.ClearModContext();
        }

        public override AmmoPipelineData ReadData(BinaryReader reader, ModSaveInfo info)
        {
            var ammo = new PartialAmmoData();
            ammo.Read(reader);
            return new AmmoPipelineData(this)
            {
                Ammo = ammo,
                Identifier = AmmoIdentifier.Read(reader) 
            };
        }

        public override void RemoveExtraModdedData(ModSaveInfo mod, GameV12 data)
        {

        }

        protected override void PushData(ModSaveInfo mod, GameV12 data, AmmoPipelineData item)
        {
            var ammoData = AmmoIdentifier.ResolveToData(item.Identifier, data);
            if (ammoData == null) return;
            item.Ammo.Push(ammoData);
        }

        protected override void WriteData(BinaryWriter writer, ModSaveInfo info, AmmoPipelineData item)
        {
            item.Ammo.Write(writer);
            AmmoIdentifier.Write(item.Identifier, writer);
        }

        public class AmmoPipelineData : PipelineData
        {
            public AmmoIdentifier Identifier;
            public PartialAmmoData Ammo;

            public AmmoPipelineData(ISavePipeline pipeline) : base(pipeline)
            {
            }

            static AmmoPipelineData()
            {
                EnumTranslator.RegisterEnumFixer<AmmoPipelineData>((trans, mode, obj) =>
                {
                    trans.FixEnumValues(mode, obj.Ammo);
                    
                });
            }
        }
    }
}

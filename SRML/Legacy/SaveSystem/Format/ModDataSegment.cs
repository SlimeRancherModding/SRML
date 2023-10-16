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
using SRML.SR.SaveSystem.Data.Ammo;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV09;
using UnityEngine;
using VanillaGadgetData = MonomiPark.SlimeRancher.Persist.PlacedGadgetV08;
using SRML.SR.SaveSystem.Data.Gadget;
using SRML.SR.SaveSystem.Registry;
using SRML.Utils;
using VanillaAmmoData = MonomiPark.SlimeRancher.Persist.AmmoDataV02;
namespace SRML.SR.SaveSystem.Format
{
    class ModDataSegment
    {
        public const int DATA_VERSION = 3;

        public int version;
        public string modid;
        public long byteLength;

        public List<IdentifiedData> identifiableData = new List<IdentifiedData>();

        public List<ExtendedDataTree> extendedData = new List<ExtendedDataTree>();

        public ModPlayerData playerData = new ModPlayerData();

        public ModPediaData pediaData = new ModPediaData();

        public ModWorldData worldData = new ModWorldData();

        public Dictionary<AmmoIdentifier, List<VanillaAmmoData>> customAmmo = new Dictionary<AmmoIdentifier, List<VanillaAmmoData>>();

        public CompoundDataPiece extendedWorldData = new CompoundDataPiece("root");

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
                pediaData.Read(reader);
                BinaryUtils.ReadDictionary(reader,customAmmo,(x)=>AmmoIdentifier.Read(x), (x) =>
                {
                    var list = new List<VanillaAmmoData>();
                    int ammoCount = x.ReadInt32();
                    for (int i = 0; i < ammoCount; i++)
                    {
                        if (x.ReadBoolean())
                        {
                            var newData = new VanillaAmmoData();
                            newData.Load(x.BaseStream);
                            list.Add(newData);
                        }
                        else
                        {
                            list.Add(null);
                        }
                    }

                    return list;
                } );
                if (version < 2) return;
                extendedWorldData = (CompoundDataPiece)DataPiece.Deserialize(reader);
                if (version < 3) return;
                worldData.Read(reader);
            }
            else
            {
                identifiableData.Clear(); // with the new enum translator system we need to make sure old id's are gone
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
            pediaData.Write(writer);
            
            BinaryUtils.WriteDictionary(writer,customAmmo,(x,y)=>AmmoIdentifier.Write(y,x), (x, y) =>
            {
                x.Write(y.Count);
                foreach (var v in y)
                {
                    x.Write(v!=null);
                    if (v != null)
                    {
                        v.Write(x.BaseStream);
                    }
                }
            });

            DataPiece.Serialize(writer,extendedWorldData);

            worldData.Write(writer);

            var cur = writer.BaseStream.Position;
            writer.BaseStream.Seek(overwritePosition, SeekOrigin.Begin);
            byteLength = cur - (start);
            writer.Write(byteLength);
            writer.BaseStream.Seek(cur, SeekOrigin.Begin);

        }

        public void FixAllValues(EnumTranslator enumTranslator, EnumTranslator.TranslationMode mode)
        {
            if (enumTranslator == null) return;
            EnumTranslator.FixEnumValues(enumTranslator,mode,identifiableData);
            EnumTranslator.FixEnumValues(enumTranslator,mode,playerData);
            EnumTranslator.FixEnumValues(enumTranslator,mode,customAmmo);
            EnumTranslator.FixEnumValues(enumTranslator, mode, worldData);
            var newDict = new Dictionary<AmmoIdentifier, List<VanillaAmmoData>>();
            long FixValue(AmmoType type, long original)
            {
                switch (type)
                {
                    case AmmoType.PLAYER:
                        return (int)enumTranslator.TranslateEnum(typeof(PlayerState.AmmoMode), mode, (PlayerState.AmmoMode)(int)original);
                    case AmmoType.LANDPLOT:
                        return (int)enumTranslator.TranslateEnum(typeof(SiloStorage.StorageType), mode, (SiloStorage.StorageType)(int)original);
                }
                return original;
            }
            foreach (var v in customAmmo)
            {
                newDict[new AmmoIdentifier(v.Key.AmmoType, FixValue(v.Key.AmmoType, v.Key.longIdentifier), v.Key.stringIdentifier, v.Key.custommodid)] = v.Value;
            }
            customAmmo = newDict;
            EnumTranslator.FixEnumValues(enumTranslator, mode, extendedData);
            EnumTranslator.FixEnumValues(enumTranslator, mode, extendedWorldData);
        }
    }

    
}

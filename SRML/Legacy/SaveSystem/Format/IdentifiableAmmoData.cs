using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRML.SR.SaveSystem.Data.Ammo;

namespace SRML.SR.SaveSystem.Format
{
    internal class IdentifiableAmmoData
    {
        public AmmoIdentifier identifier;
        public PersistentAmmoModel model = new PersistentAmmoModel();

        public void Write(BinaryWriter writer)
        {
            AmmoIdentifier.Write(identifier,writer);
            model.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            identifier=AmmoIdentifier.Read(reader);
            model.Read(reader);
        }

        static IdentifiableAmmoData()
        {
            EnumTranslator.RegisterEnumFixer<IdentifiableAmmoData>((translator, mode, data) =>
            {
                translator.FixEnumValues(mode, data.model);
                long FixValue(AmmoType type, long original)
                {
                    switch (type)
                    {
                        case AmmoType.PLAYER:
                            return (int)translator.TranslateEnum(typeof(PlayerState.AmmoMode), mode, (PlayerState.AmmoMode)(int)original);
                        case AmmoType.LANDPLOT:
                            return (int)translator.TranslateEnum(typeof(SiloStorage.StorageType), mode, (SiloStorage.StorageType)(int)original);
                    }
                    return original;
                }
                data.identifier = new AmmoIdentifier(data.identifier.AmmoType,   FixValue(data.identifier.AmmoType,data.identifier.longIdentifier), data.identifier.stringIdentifier, data.identifier.custommodid);
            });
        }
    }
}

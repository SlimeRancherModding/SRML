using SRML.Core.API;
using System;

namespace SRML.API.World
{
    public class SiloAmmoRegistry : Registry<SiloAmmoRegistry, SiloStorage.StorageType,
        global::Identifiable.Id>
    {
        public override void Initialize()
        {
        }

        public override bool IsRegistered(SiloStorage.StorageType registered, global::Identifiable.Id registered2) => 
            StorageTypeExtensions.GetContents(registered).Contains(registered2);

        public override void Register(SiloStorage.StorageType toRegister, global::Identifiable.Id toRegister2)
        {
        }

        public void Register(Predicate<SiloStorage.StorageType> typePred, global::Identifiable.Id toRegister)
        {
            foreach (SiloStorage.StorageType type in Enum.GetValues(typeof(SiloStorage.StorageType)))
            {
                if (typePred(type))
                    Register(type, toRegister);
            }
        }
    }
}

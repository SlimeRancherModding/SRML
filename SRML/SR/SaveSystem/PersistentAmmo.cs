using SRML.Console;
using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Ammo;
using System.Linq;

namespace SRML.SR.SaveSystem
{
    internal class PersistentAmmo
    {
        public AmmoIdentifier Identifier;
        public PersistentAmmoModel DataModel;

        public Identifiable.Id potentialId;
        public CompoundDataPiece potentialTag;

        public PersistentAmmo(AmmoIdentifier identifier, PersistentAmmoModel model)
        {
            this.Identifier = identifier;
            this.DataModel = model;
        }

        public void Sync(bool log = false)
        {
            DataModel.UpdateFromExistingSlots(Identifier.ResolveModel().slots,log);
        }

        public void OnDecrement(int slot, int count, Identifiable.Id id)
        {
            if (DataModel.slots[slot].Count == 0)
            {
                Sync();
                return;
            }
            for (int i = 0; i < count; i++)
            {
                CompoundDataPiece removed = DataModel.PopDataForSlot(slot);
                if (removed != null && Identifier.AmmoType == AmmoType.DRONE)
                    DronePersistentAmmoManager.OnDecrement(removed, id);
            }
            Sync();
            ClearSelected();
        }

        public void OnSelected(Identifiable.Id id, int slot)
        {
            if (DataModel.slots[slot].Count == 0) return;
            potentialId = id;
            potentialTag = DataModel.PeekDataForSlot(slot);
        }

        public void ClearSelected()
        {
            potentialId = Identifiable.Id.NONE;
            potentialTag = null;
        }
    }
}

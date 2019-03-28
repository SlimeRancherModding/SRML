using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem.Data.Ammo;

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

        public void Sync()
        {
            DataModel.UpdateFromExistingSlots(Identifier.ResolveModel().slots);
        }

        public void OnDecrement(int slot, int count)
        {
            if (DataModel.slots[slot].Count == 0) return;
            for (int i = 0; i < count; i++)
            {
                DataModel.PopDataForSlot(slot);
            }
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

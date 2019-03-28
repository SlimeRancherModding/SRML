using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Ammo
{
    internal class PersistentAmmoModel
    {
        public PersistentAmmoSlot[] slots;

        public void InitializeFromExistingSlots(global::Ammo.Slot[] ammoSlots)
        {
            slots = new PersistentAmmoSlot[ammoSlots.Length];
            for (int i = 0; i < ammoSlots.Length; i++)
            {
                slots[i] = new PersistentAmmoSlot();
                slots[i].UpdateFromExistingSlot(ammoSlots[i]);
            }
        }

        public bool HasNoData()
        {
            foreach (var v in slots)
            {
                if (!v.HasNoData()) return false;
            }
            return true;
        }

        public void UpdateFromExistingSlots(global::Ammo.Slot[] ammoSlots)
        {
            if (slots == null)
            {
                InitializeFromExistingSlots(ammoSlots);
                return;
            }
            for (int i = 0; i < ammoSlots.Length; i++)
            {
                slots[i].UpdateFromExistingSlot(ammoSlots[i]);
            }
        }

        public PersistentAmmoModel(AmmoModel model)
        {
            InitializeFromExistingSlots(model.slots);
        }

        public PersistentAmmoModel() { }

        public CompoundDataPiece PopDataForSlot(int index)
        {
            return slots[index].PopTop();
        }

        public void PushDataForSlot(int index,CompoundDataPiece data)
        {
            slots[index].PushTop(data);
        }

        public CompoundDataPiece PeekDataForSlot(int index)
        {
            return slots[index].PeekTop();
        }

        public void BalanceSlot(int index, int slotCount)
        {
            slots[index].CompensateForExternalChanges(slotCount);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(slots.Length);
            foreach (var slot in slots)
            {
                slot.Write(writer);
            }
        }

        public void Read(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            this.slots = new PersistentAmmoSlot[count];

            for (int i = 0; i < count; i++)
            {
                slots[i] = new PersistentAmmoSlot();
                slots[i].Read(reader);
            }
        }
    }
}

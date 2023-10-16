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

        public bool HasNoData()
        {
            foreach (var v in slots)
            {
                if (!v.HasNoData()) return false;
            }
            return true;
        }

        public void AdjustSlotCount(int newCount)
        {
            var slotList = slots!=null?slots.ToList():new List<PersistentAmmoSlot>();

            while (slotList.Count > newCount)
            {
                slotList.RemoveAt(slotList.Count-1);
            }

            while (slotList.Count < newCount)
            {
                slotList.Add(new PersistentAmmoSlot());
            }

            slots = slotList.ToArray();
        }

        public void UpdateFromExistingSlots(global::Ammo.Slot[] ammoSlots,bool log = false)
        {
            AdjustSlotCount(ammoSlots.Length);
            for (int i = 0; i < ammoSlots.Length; i++)
            {
                slots[i].UpdateFromExistingSlot(ammoSlots[i],log);
            }
        }

        public PersistentAmmoModel(AmmoModel model)
        {
            UpdateFromExistingSlots(model.slots);
        }

        public PersistentAmmoModel() { }

        static PersistentAmmoModel()
        {
            EnumTranslator.RegisterEnumFixer<PersistentAmmoModel>((translator, mode, loop) =>
            {
                translator.FixEnumValues(mode, loop.slots);
            });
        }

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

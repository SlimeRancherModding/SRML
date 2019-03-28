using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Ammo
{
    internal class PersistentAmmoSlot
    {
        private List<CompoundDataPiece> data= new List<CompoundDataPiece>();

        private int lastKnownAmount;

        public int Count => data.Count;

        public void UpdateFromExistingSlot(global::Ammo.Slot slot)
        {
            CompensateForExternalChanges(slot?.count ?? 0);
        }

        public CompoundDataPiece PopBottom()
        {
            var temp = data.First();
            data.Remove(temp);
            lastKnownAmount--;
            return temp;
        }

        public CompoundDataPiece PopTop()
        {
            var temp = data.Last();
            data.Remove(temp);
            lastKnownAmount--;
            return temp;
        }

        public CompoundDataPiece PeekTop()
        {
            return data.Last();
        }

        public void PushBottom(CompoundDataPiece piece)
        {
            data.Insert(0,piece);
            lastKnownAmount++;
        }

        public void PushTop(CompoundDataPiece piece)
        {
            data.Add(piece);
            lastKnownAmount++;
        }

        public void CompensateForExternalChanges(int realAmount)
        {
            if (realAmount - lastKnownAmount != 0)
            {
                Debug.Log("Compensating for a difference of "+(realAmount-lastKnownAmount));
            }

            while (realAmount > lastKnownAmount)
            {
                PushBottom(null);
            }

            while (realAmount < lastKnownAmount)
            {
                PopBottom();
            }

        }


        public void Read(BinaryReader reader)
        {
            data.Clear();
            lastKnownAmount = reader.ReadInt32();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                bool shouldbenull = reader.ReadBoolean();
                if (shouldbenull)
                {
                    data.Add(null);
                }
                else
                {
                    var piece = DataPiece.Deserialize(reader) as CompoundDataPiece;
                    ExtendedData.CullMissingModsFromData(piece);
                    data.Add(piece);
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(lastKnownAmount);
            writer.Write(data.Count);
            foreach (var piece in data)
            {
                writer.Write(piece==null);
                if (piece != null)
                {
                    piece.Serialize(writer);
                }
            }
        }

        
    }
}

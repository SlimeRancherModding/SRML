using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR
{
    public static class FashionRegistry
    {
        internal static Dictionary<Identifiable.Id, (Func<AttachFashions, bool>, Vector3)> offsetsForFashions = new Dictionary<Identifiable.Id, (Func<AttachFashions, bool>, Vector3)>();
        
        internal static Dictionary<Fashion.Slot, (string, SlimeAppearance.SlimeBone)> slotAttachPoints = new Dictionary<Fashion.Slot, (string, SlimeAppearance.SlimeBone)>();

        public static void RegisterOffsetForFashion(Identifiable.Id id, Vector3 offset, Func<AttachFashions, bool> condition) => offsetsForFashions.Add(id, (condition, offset));

        public static void RegisterSlot(Fashion.Slot slot, string path, SlimeAppearance.SlimeBone bone) => slotAttachPoints.Add(slot, (path, bone));
    }
}

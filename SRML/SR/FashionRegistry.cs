using SRML.API.Identifiable.Slime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR
{
    [Obsolete]
    public static class FashionRegistry
    {
        /// <summary>
        /// Registers an offset for a fashion.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> of the fashion to offset.</param>
        /// <param name="offset">How far the fashion gets offset.</param>
        /// <param name="condition">The condition that the fashion gets offset in.</param>
        public static void RegisterOffsetForFashion(Identifiable.Id id, Vector3 offset, Func<AttachFashions, bool> condition) =>
            FashionOffsetRegistry.Instance.Register(id, offset, new Predicate<AttachFashions>(condition));

        /// <summary>
        /// Registers a slot for a fashion to be placed in.
        /// </summary>
        /// <param name="slot">The <see cref="Fashion.Slot"/> that belongs to the slot.</param>
        /// <param name="path">The hierarchal path to the bone.</param>
        /// <param name="bone">The <see cref="SlimeAppearance.SlimeBone"/> that the slot attaches to.</param>
        public static void RegisterSlot(Fashion.Slot slot, string path, SlimeAppearance.SlimeBone bone) => 
            FashionSlotRegistry.Instance.Register(slot, bone, path);
    }
}

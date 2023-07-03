using SRML.API.Player;
using SRML.API.World;
using System;
using UnityEngine;

namespace SRML.SR
{
    [Obsolete]
    public static class AmmoRegistry
    {
        /// <summary>
        /// Register an ammo prefab to allow it to be put in a player's ammos (use <see cref="RegisterPlayerAmmo(PlayerState.AmmoMode, Identifiable.Id)"/>)
        /// </summary>
        /// <param name="mode"><see cref="PlayerState.AmmoMode"/> to register the prefab to</param>
        /// <param name="prefab"></param>
        public static void RegisterAmmoPrefab(PlayerState.AmmoMode mode, GameObject prefab) =>
            RegisterPlayerAmmo(mode, Identifiable.GetId(prefab));

        /// <summary>
        /// Allow an <paramref name="id"/> to be put into a players inventory
        /// </summary>
        /// <param name="mode">Which inventory to allow the <paramref name="id"/> into</param>
        /// <param name="id">The <see cref="Identifiable.Id"/> to allow</param>
        public static void RegisterPlayerAmmo(PlayerState.AmmoMode mode, Identifiable.Id id) =>
            PlayerAmmoRegistry.Instance.Register(mode, id);

        /// <summary>
        /// Allow an Identifiable.Id to be put into a <see cref="SiloStorage"/> inventory
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        public static void RegisterSiloAmmo(SiloStorage.StorageType typeId, Identifiable.Id id) =>
            SiloAmmoRegistry.Instance.Register(typeId, id);

        public static void RegisterSiloAmmo(Predicate<SiloStorage.StorageType> pred, Identifiable.Id id) =>
            SiloAmmoRegistry.Instance.Register(pred, id);

        /// <summary>
        /// Allow an Identifiable to be put into the Refinery
        /// </summary>
        /// <param name="id"></param>
        public static void RegisterRefineryResource(Identifiable.Id id) =>
            API.Gadget.RefineryRegistry.Instance.Register(id);
    }
}

using SRML.Core.API;
using SRML.Core.ModLoader;
using System.Collections.Generic;

namespace SRML.API.Player
{
    // TODO: Allow definition of new ammomodes
    public class PlayerAmmoRegistry : Registry<PlayerAmmoRegistry, PlayerState.AmmoMode,
        global::Identifiable.Id>
    {
        public override void Initialize()
        {
        }

        public override bool IsRegistered(PlayerState.AmmoMode registered, global::Identifiable.Id registered2)
        {
            if (SceneContext.Instance.PlayerState == null)
                return false;

            return SceneContext.Instance.PlayerState.ammoDict[registered].potentialAmmo.Contains(registered2);
        }

        public override void Register(PlayerState.AmmoMode toRegister, global::Identifiable.Id toRegister2)
        {
            _registered.AddIfDoesNotContain((toRegister, toRegister2));
            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<(PlayerState.AmmoMode, global::Identifiable.Id)>());

                registeredForMod[mod].Add((toRegister, toRegister2));
            }
        }
    }
}

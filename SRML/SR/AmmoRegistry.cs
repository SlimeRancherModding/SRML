using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR
{
    public static class AmmoRegistry
    {
        internal static Dictionary<PlayerState.AmmoMode,List<GameObject>> inventoryPrefabsToPatch = new Dictionary<PlayerState.AmmoMode, List<GameObject>>();

        public static void RegisterAmmoPrefab(PlayerState.AmmoMode mode, GameObject prefab)
        {
            if (!inventoryPrefabsToPatch.ContainsKey(mode))
            {
                inventoryPrefabsToPatch[mode] = new List<GameObject>();
            }
            inventoryPrefabsToPatch[mode].Add(prefab);
        }

        static AmmoRegistry()
        {
            SRCallbacks.OnSaveGameLoaded += ((t) =>
            {
                foreach (var v in inventoryPrefabsToPatch)
                {
                    v.Value.ForEach((x)=>t.GameModel.GetPlayerModel().RegisterPotentialAmmo(v.Key,x));
                }
               
            });
        }
    }
}

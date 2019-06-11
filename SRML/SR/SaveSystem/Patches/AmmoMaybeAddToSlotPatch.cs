using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using SRML.SR.SaveSystem.Data.Ammo;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("MaybeAddToSlot")]
    internal static class AmmoMaybeAddToSlotPatch
    {
        public static void Postfix(Ammo __instance, bool __result, Identifiable.Id id, Identifiable identifiable)
        {
            if (!__result) return;

            if (AmmoIdentifier.TryGetIdentifier(__instance, out var identifier))
            {
                
                


                var count = -1;
                for (int i = 0; i < __instance.Slots.Length; i++)
                {
                    if (__instance.Slots[i]?.id == id)
                    {
                        count = i;
                        break;
                    }
                }

                if (count == -1) throw new Exception();
                if (identifiable&&ExtendedData.IsRegistered(identifiable.gameObject))
                {
                    var ammo = PersistentAmmoManager.GetPersistentAmmoForAmmo(__instance.ammoModel);
                    ammo.DataModel.slots[count].PushTop(ExtendedData.extendedActorData[identifiable.GetActorId()]);
                    ammo.Sync();
                }
                else
                {
                    if (!PersistentAmmoManager.HasPersistentAmmo(identifier)) return;
                    var ammo = PersistentAmmoManager.GetPersistentAmmoForAmmo(__instance.ammoModel);
                    ammo.DataModel.slots[count].PushTop(null);
                    ammo.Sync();
                }
            }
        }
        
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("MaybeAddToSpecificSlot", new Type[]
        {typeof(Identifiable.Id), typeof(Identifiable), typeof(int), typeof(int), typeof(bool)})]
    internal static class AmmoMaybeAddToSpecificSlotPatch
    {
        public static void Postfix(Ammo __instance, bool __result, Identifiable.Id id, Identifiable identifiable,
            int slotIdx, int count, bool overflow)
        {
            if (!__result) return;

            if (AmmoIdentifier.TryGetIdentifier(__instance, out var identifier))
            {
                if (identifiable && ExtendedData.IsRegistered(identifiable.gameObject))
                {
                    var ammo = PersistentAmmoManager.GetPersistentAmmoForAmmo(__instance.ammoModel);
                    
                    for (int i = 0; i < count; i++)
                        ammo.DataModel.slots[slotIdx]
                            .PushTop(ExtendedData.extendedActorData[identifiable.GetActorId()]);
                    ammo.Sync();
                }
                else
                {
                    if (!PersistentAmmoManager.HasPersistentAmmo(identifier)) return;
                    var ammo = PersistentAmmoManager.PersistentAmmoData[identifier];
                    for (int i = 0; i < count; i++)
                        ammo.DataModel.slots[slotIdx].PushTop(null);
                    ammo.Sync();
                }
            }
        }
    }
}

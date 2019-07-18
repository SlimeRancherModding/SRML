using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Persist;
using rail;
using UnityEngine;
using VanillaAmmoData = MonomiPark.SlimeRancher.Persist.AmmoDataV02;
namespace SRML.SR.SaveSystem.Utils
{
    public static class AmmoDataUtils
    {
        public static void SpliceAmmoData(List<VanillaAmmoData> original, List<VanillaAmmoData> toSplice)
        {
            if (original == null || toSplice == null) return;
            for (int i = 0; i < original.Count;i++)
            {
                if (toSplice[i] != null) original[i] = toSplice[i];
            }
        }

        public static List<VanillaAmmoData> RipOutWhere(List<VanillaAmmoData> original, Predicate<VanillaAmmoData> pred,bool doRip = true)
        {
            var newData = new VanillaAmmoData[original.Count];
            for (int i = original.Count - 1; i >= 0; i--)
            {
                if (pred(original[i]))
                {
                    newData[i] = original[i];
                    if (doRip) original[i] = new AmmoDataV02()
                    {
                        count=0,
                        emotionData = new SlimeEmotionDataV02()
                        {
                            emotionData = new Dictionary<SlimeEmotions.Emotion,float>()
                        },
                        id=Identifiable.Id.NONE
                    };
                }
            }

            return newData.ToList();
        }

        public static List<VanillaAmmoData> RipOutModdedData(List<VanillaAmmoData> original)
        {
            return RipOutWhere(original, (x) => ModdedIDRegistry.IsModdedID(x.id));
        }

        public static bool HasCustomData(List<VanillaAmmoData> ammo)
        {
            return ammo.Any((x) => ModdedIDRegistry.IsModdedID(x.id));
        }

        public static List<List<VanillaAmmoData>> GetAllAmmoData(GameV11 game)
        {
            List<List<VanillaAmmoData>> ammoDataData = new List<List<VanillaAmmoData>>();

            ammoDataData.AddRange(game.player.ammo.Values);

            ammoDataData.AddRange(game.ranch.plots.SelectMany((x)=>x.siloAmmo).Select((x)=>x.Value));

            ammoDataData.AddRange(game.world.placedGadgets.Values.Select((x)=>x.ammo));

            ammoDataData.RemoveAll((x) => x == null);

            return ammoDataData;
        }

        public static Action RemoveAmmoDataWithAddBack(List<VanillaAmmoData> data,GameV11 game)
        {
            var player = game.player.ammo.FirstOrDefault(x => x.Value == data);
            if (player.Value != null)
            {
                game.player.ammo.Remove(player.Key);
                return () => game.player.ammo.Add(player.Key, player.Value);
            }

            foreach(var plot in game.ranch.plots)
            {
                var silo = plot.siloAmmo.FirstOrDefault(x => x.Value == data);
                if (silo.Value != null)
                {
                    plot.siloAmmo.Remove(silo.Key);
                    return () => plot.siloAmmo.Add(silo.Key,silo.Value);
                }
            }

            foreach(var gadget in game.world.placedGadgets)
            {
                if(data == gadget.Value.ammo)
                {
                    gadget.Value.ammo = new List<VanillaAmmoData>();
                    return () => gadget.Value.ammo = data;
                }
            }
            return () => throw new Exception();
        }
        
        static AmmoDataUtils()
        {
            
        }
    }
}

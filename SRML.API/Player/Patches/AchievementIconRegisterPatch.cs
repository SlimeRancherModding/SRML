using HarmonyLib;
using UnityEngine;

namespace SRML.API.Player.Patches
{
    [HarmonyPatch(typeof(AchievementsDirector), "GetAchievementImage")]
    internal static class AchievementIconRegisterPatch
    {
        public static bool Prefix(AchievementsDirector.Achievement achieve, ref Sprite __result)
        {
            AchievementTierRegistry reg = AchievementTierRegistry.Instance;
            
            if (reg.tiersForAchievement.ContainsKey(achieve) && reg.IsModdedTier(reg.tiersForAchievement[achieve]))
            {
                __result = reg.spritesForTiers[reg.tiersForAchievement[achieve]];
                return false;
            }

            return true;
        }
    }
}

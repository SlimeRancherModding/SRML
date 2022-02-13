using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(GordoSnare), "AttachBait")]
    internal static class SnareAttachPatch
    {
        private static void AttachBait(GordoSnare __instance, Identifiable.Id id)
        {
            __instance.ClearBait();
            __instance.model.baitTypeId = id;
            __instance.bait = Object.Instantiate(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(id), __instance.transform);
            __instance.bait.transform.position = __instance.baitPosition.transform.position;
            __instance.bait.transform.rotation = Quaternion.identity;
        }

        public static bool Prefix(GordoSnare __instance, Identifiable.Id id)
        {
            if (Identifiable.IsSlime(id))
            {
                AttachBait(__instance, id);
                __instance.RemoveComponents<Collider>(__instance.bait);
                __instance.RemoveComponent<DragFloatReactor>(__instance.bait);
                __instance.RemoveComponent<Rigidbody>(__instance.bait);
                __instance.RemoveComponent<KeepUpright>(__instance.bait);
                __instance.RemoveComponent<DontGoThroughThings>(__instance.bait);
                __instance.RemoveComponent<SECTR_PointSource>(__instance.bait);
                __instance.RemoveComponent<RegionMember>(__instance.bait);
                __instance.RemoveComponent<ChickenRandomMove>(__instance.bait);
                __instance.RemoveComponent<ChickenVampirism>(__instance.bait);
                __instance.RemoveComponent<PlaySoundOnHit>(__instance.bait);
                __instance.RemoveComponent<ResourceCycle>(__instance.bait);
                __instance.RemoveComponent<Reproduce>(__instance.bait);
                __instance.RemoveComponent<SlimeEmotions>(__instance.bait);
                __instance.RemoveComponent<SlimeFaceAnimator>(__instance.bait);
                __instance.RemoveComponent<SlimeEat>(__instance.bait);
                __instance.RemoveComponent<SlimeEatAsh>(__instance.bait);
                __instance.RemoveComponent<SlimeEatWater>(__instance.bait);
                __instance.RemoveComponent<SlimeEatTrigger>(__instance.bait);
                __instance.RemoveComponent<SlimeSubbehaviourPlexer>(__instance.bait);
                __instance.RemoveComponents<SlimeSubbehaviour>(__instance.bait);
                Animator animator = __instance.bait.GetComponentInChildren<Animator>();
                if (animator != null)
                    animator.SetBool("grounded", true);
                return false;
            }
            else if (Identifiable.IsPlort(id))
            {
                AttachBait(__instance, id);
                __instance.RemoveComponents<Collider>(__instance.bait);
                __instance.RemoveComponent<DragFloatReactor>(__instance.bait);
                __instance.RemoveComponent<Rigidbody>(__instance.bait);
                __instance.RemoveComponent<DestroyPlortAfterTime>(__instance.bait);
                __instance.RemoveComponent<PlortInvulnerability>(__instance.bait);
                __instance.RemoveComponent<PlaySoundOnHit>(__instance.bait);
                __instance.RemoveComponent<DestroyOnIgnite>(__instance.bait);
                __instance.RemoveComponent<RegionMember>(__instance.bait);
                return false;
            }
            return true;
        }
    }
}

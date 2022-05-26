using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using System;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(ActorModel), "Init")]
    internal static class ActorModelInitPatch
    {
        public static bool Prefix(ActorModel __instance, GameObject gameObj)
        {
            ActorModel.Participant[] componentsInChildren = gameObj.GetComponentsInChildren<ActorModel.Participant>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                try
                {
                    componentsInChildren[i].InitModel(__instance);
                }
                catch (Exception e)
                {
                    SRML.Console.Console.Instance.LogError("Failed to initialize model participant, removing component\n" + e);
                    UnityEngine.Object.DestroyImmediate(componentsInChildren[i] as Component);
                }
            }
            return false;
        }
    }
}

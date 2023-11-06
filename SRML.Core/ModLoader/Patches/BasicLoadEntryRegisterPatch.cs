using HarmonyLib;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;
using SRML.SR.Utils;
using System;

namespace SRML.Core.ModLoader.Patches
{
    [HarmonyPatch(typeof(GameContext), "Start")]
    internal static class BasicLoadEntryRegisterPatch
    {
        public static void Prefix()
        {
            foreach (var del in BasicLoadEntryPoint.GameContextLoad.GetInvocationList())
            {
                try
                {
                    del.Method.Invoke(del.Target, null);
                }
                catch (Exception ex)
                {
                    if (CoreLoader.Instance.infoForEntry.TryGetValue(del.Target as IEntryPoint, out IModInfo info))
                        ErrorGUI.errors.Add(info.ID, (ErrorGUI.ErrorType.LoadMod, ex));
                    else
                        ErrorGUI.errors.Add(del.Target.GetType().Assembly.GetName().Name, (ErrorGUI.ErrorType.LoadMod, ex));
                }
            }
        }
        public static void Postfix()
        {
            // TODO: reimplement permanant eatmap additions
            GameContext.Instance.SlimeDefinitions.RefreshEatmaps();
            foreach (var del in BasicLoadEntryPoint.GameContextPostLoad.GetInvocationList())
            {
                try
                {
                    del.Method.Invoke(del.Target, null);
                }
                catch (Exception ex)
                {
                    if (CoreLoader.Instance.infoForEntry.TryGetValue(del.Target as IEntryPoint, out IModInfo info))
                        ErrorGUI.errors.Add(info.ID, (ErrorGUI.ErrorType.LoadMod, ex));
                    else
                        ErrorGUI.errors.Add(del.Target.GetType().Assembly.GetName().Name, (ErrorGUI.ErrorType.LoadMod, ex));
                }
            }
        }
    }
}

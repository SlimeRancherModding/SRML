using HarmonyLib;
using SRML.Core.ModLoader.BuiltIn.EntryPoint;
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
                    if (Main.loader.infoForEntry.TryGetValue(del.Target as IEntryPoint, out IModInfo info))
                        ErrorGUI.errors.Add(info.Id, (ErrorGUI.ErrorType.LoadMod, ex));
                    else
                        ErrorGUI.errors.Add(del.Target.GetType().Assembly.GetName().Name, (ErrorGUI.ErrorType.LoadMod, ex));
                }
            }
        }
        public static void Postfix()
        {
            foreach (var del in BasicLoadEntryPoint.GameContextPostLoad.GetInvocationList())
            {
                try
                {
                    del.Method.Invoke(del.Target, null);
                }
                catch (Exception ex)
                {
                    if (Main.loader.infoForEntry.TryGetValue(del.Target as IEntryPoint, out IModInfo info))
                        ErrorGUI.errors.Add(info.Id, (ErrorGUI.ErrorType.LoadMod, ex));
                    else
                        ErrorGUI.errors.Add(del.Target.GetType().Assembly.GetName().Name, (ErrorGUI.ErrorType.LoadMod, ex));
                }
            }
        }
    }
}

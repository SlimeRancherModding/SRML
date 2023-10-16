using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(MailDirector))]
    [HarmonyPatch("MarkRead")]
    internal static class MailReadPatch
    {
        public static void Postfix(MailDirector __instance, MailDirector.Mail mail)
        {
            MailRegistry.ModdedMails.FirstOrDefault((x) => mail.key == x.MailKey)?.onReadCallback(__instance, mail);
        }
    }
}

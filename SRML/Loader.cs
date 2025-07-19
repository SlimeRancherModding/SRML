using HarmonyLib;
using SRML;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Doorstop
{
    internal class Entrypoint
    {
        public static Assembly[] resolve = Directory.GetFiles(Path.GetFullPath(@"SRML\Libs"), "*.dll").Select(x => Assembly.LoadFile(x)).ToArray();

        public static void Start() => new Task(() =>
        {
            // I really hope this number is large enough to load successfully on all PCs
            // CompanyLogoScene spends 1.5s fading in, 1s hold, 1.5s fading out
            // so this will theoretically load Doorstop as soon as the logo fades out
            Task.Delay(2750);
            Debug.Log("Doorstop has succesfully hooked.");

            AppDomain.CurrentDomain.AssemblyResolve += (x, y) => resolve.FirstOrDefault(z => z.GetName() == new AssemblyName(y.Name));

            new Harmony("SRMLInitializer").Patch(AccessTools.Method(typeof(StandaloneStartScreen), "Update"),
                new HarmonyMethod(typeof(Main), "InitializeSRMLThenBeginLoad"));
        }).Start();
    }
}

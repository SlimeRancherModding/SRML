using HarmonyLib;
using SRML.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Doorstop
{
    internal class Loader
    {
        public static Assembly[] resolve = Directory.GetFiles(Path.GetFullPath(@"SRML\Libs"), "*.dll").Select(x => Assembly.LoadFile(x)).ToArray();

        public static void Main(string[] args) => new Task(() =>
        {
            Task.Delay(5000);
            Debug.Log("Doorstop has succesfully hooked.");

            AppDomain.CurrentDomain.AssemblyResolve += (x, y) => resolve.FirstOrDefault(z => z.GetName() == new AssemblyName(y.Name));

            new Harmony("SRMLInitializer").Patch(AccessTools.Method(typeof(GameContext), "Awake"),
                new HarmonyMethod(typeof(Main), "Initialize"));
        }).Start();
    }
}

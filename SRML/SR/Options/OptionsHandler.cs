using InControl;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.Options
{
    internal static class OptionsHandler
    {
        const string KEYBIND_FILE_NAME = "keybinds";

        public static void Push(OptionsV11 options)
        {
            foreach(var mod in new HashSet<SRMod>(BindingRegistry.moddedActions.Values))
            {
                var filename = Path.Combine(FileSystem.GetConfigPath(mod), KEYBIND_FILE_NAME);
                if (!File.Exists(filename)) continue;
                var modoptions = new ModOptionsV01();
                using (var file = File.Open(filename, FileMode.Open))
                {
                    modoptions.Load(file);
                }
                options.bindings.bindings.AddRange(modoptions.bindings.bindings);
            }
        }

        public static void Pull(SavedProfile profile)
        {
            foreach (var mod in new HashSet<SRMod>(BindingRegistry.moddedActions.Values))
            {
                var filename = Path.Combine(FileSystem.GetConfigPath(mod), KEYBIND_FILE_NAME);
                var modoptions = new ModOptionsV01();
                var v = BindingRegistry.moddedActions.Where(x => x.Value == mod).Select(x => x.Key).ToList();
                foreach(var action in v)
                {
                    modoptions.bindings.bindings.Add(SRInput.ToBinding(action));
                }
                using (var file = File.Open(filename, FileMode.OpenOrCreate))
                {
                    modoptions.Write(file);
                }
            }
        }
    }
}

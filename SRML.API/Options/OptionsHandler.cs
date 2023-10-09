using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using SRML.API.Player;
using System.IO;

namespace SRML.SR.Options
{
    internal static class OptionsHandler
    {
        const string KEYBIND_FILE_NAME = "keybinds";

        public static void Push(OptionsV11 options)
        {
            var filename = Path.Combine(FileSystem.GetConfigPath(null), KEYBIND_FILE_NAME);
            if (!File.Exists(filename))
                return;

            var modoptions = new ModOptionsV01();
            using (var file = File.Open(filename, FileMode.Open))
                modoptions.Load(file);

            options.bindings.bindings.AddRange(modoptions.bindings.bindings);
        }

        public static void Pull(SavedProfile profile)
        {
            var filename = Path.Combine(FileSystem.GetConfigPath(null), KEYBIND_FILE_NAME);
            var modoptions = new ModOptionsV01();
            var v = PlayerActionRegistry.Instance.Registered;

            foreach (var action in v)
                modoptions.bindings.bindings.Add(SRInput.ToBinding(action));

            using (var file = File.Open(filename, FileMode.OpenOrCreate))
                modoptions.Write(file);
        }
    }
}

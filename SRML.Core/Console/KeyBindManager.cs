using InControl;
using MonomiPark.SlimeRancher.Persist;
using SRML.SR;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Console
{
    internal static class KeyBindManager
    {
        private static List<KeyBinding> customKeyBinds = new List<KeyBinding>();
        public static List<PlayerAction> ephemeralActions = new List<PlayerAction>();

        const string FILENAME = "consolekeybindings";

        public static ConsoleActions Actions;

        public static void Update()
        {
            foreach(var bind in customKeyBinds)
            {
                if (bind.action.WasPressed) Console.ProcessInput(bind.commandToRun,true);
            }
        }

        public static void ReadBinds()
        {

            Actions = new ConsoleActions();
            
            var path = Path.Combine(FileSystem.GetMyConfigPath(), FILENAME);
            if (!File.Exists(path)) return;
            customKeyBinds.Clear();
            using(var file = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                BinaryUtils.ReadList(file, customKeyBinds, (x) => KeyBinding.Read(x));
            }
        }

        public static void SaveBinds()
        {
            var path = Path.Combine(FileSystem.GetMyConfigPath(), FILENAME);
            using(var file = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                BinaryUtils.WriteList(file, customKeyBinds, (x, y) => y.Write(x));
            }
        }

        public static void CreateBinding(string commandToRun,Key bindingKey)
        {
            var existingAction = customKeyBinds.FirstOrDefault(x => (x.action.BindingsOfTypes(BindingSourceType.KeyBindingSource).FirstOrDefault() as KeyBindingSource)?.Control.GetInclude(0) == bindingKey);
            if (existingAction != null)
            {
                existingAction.commandToRun = commandToRun;
                return;
            }
            var action = KeyBinding.GetEphemeralPlayerAction();
            action.AddBinding(new KeyBindingSource(bindingKey));

            customKeyBinds.Add(new KeyBinding() { action = action, commandToRun = commandToRun });
        }

        public static void RemoveBinding(Key key)
        {
            var existingAction = customKeyBinds.FirstOrDefault(x => (x.action.BindingsOfTypes(BindingSourceType.KeyBindingSource).FirstOrDefault() as KeyBindingSource)?.Control.GetInclude(0) == key);
            existingAction?.action.ClearBindings();
            if (existingAction != null) customKeyBinds.Remove(existingAction);
        }

        public class ProcessAllBindings : MonoBehaviour
        {
            public void Update()
            {
                KeyBindManager.Update();
            }
        }

        public class ConsoleActions : PlayerActionSet
        {

        }
        class KeyBinding
        {
            static int latestID = 0;
            public PlayerAction action;
            public string commandToRun;
            const string PREFIX = "keybind";
            public void Write(BinaryWriter writer)
            {
                var binding = SRInput.ToBinding(action);
                binding.Write(writer.BaseStream);
                writer.Write(commandToRun);
            }

            public static PlayerAction GetEphemeralPlayerAction()
            {
                var action = new PlayerAction(PREFIX+latestID++, KeyBindManager.Actions);
                ephemeralActions.Add(action);
                return action;
            }

            public static KeyBinding Read(BinaryReader reader)
            {
                var binding = new BindingV01();
                binding.Load(reader.BaseStream);
                var action = new PlayerAction(binding.action, KeyBindManager.Actions);
                ephemeralActions.Add(action);
                if (int.TryParse(binding.action.Substring(PREFIX.Length), out var val) && val >= latestID) latestID = val+1;
                SRInput.AddOrReplaceBinding(action, binding);
                var keybind = new KeyBinding() { action = action, commandToRun = reader.ReadString() };
                return keybind;
            }
        }
    }
}

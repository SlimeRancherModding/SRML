using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static HarmonyLib.AccessTools;

namespace SRML.SR
{
    public static class BindingRegistry
    {
        internal static Dictionary<PlayerAction, SRMod> moddedActions = new Dictionary<PlayerAction, SRMod>();
        internal static List<PlayerAction> moddedBindings = new List<PlayerAction>();
        internal static List<PlayerAction> ephemeralActions = new List<PlayerAction>();

        /// <summary>
        /// Register a keybind into the options UI.
        /// </summary>
        /// <param name="action">The action associated with the keybind.</param>
        public static void RegisterBind(PlayerAction action) => moddedBindings.Add(action);

        internal static void RegisterAction(PlayerAction action, SRMod mod) => moddedActions.Add(action, mod);

        /// <summary>
        /// Creates and registers a <see cref="PlayerAction"/>.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <returns>The created action</returns>
        public static PlayerAction RegisterAction(string name)
        {
            PlayerAction act = new PlayerAction(name, SRInput.Actions);
            RegisterAction(act, SRMod.GetCurrentMod());
            return act;
        }

        /// <summary>
        /// Creates and registers a <see cref="PlayerAction"/>, then registers it into the options UI.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <returns>The created action</returns>
        public static PlayerAction RegisterBindedAction(string name)
        {
            PlayerAction act = RegisterAction(name);
            RegisterBind(act);
            return act;
        }

        /// <summary>
        /// Check if an action is modded.
        /// </summary>
        /// <param name="action">The action to check.</param>
        /// <returns>True if action belongs to a mod, otherwise false.</returns>
        public static bool IsModdedAction(PlayerAction action) => moddedActions.ContainsKey(action) || ephemeralActions.Contains(action);

        /// <summary>
        /// Register all <see cref="PlayerAction"/>'s in a Type
        /// </summary>
        /// <param name="type">Type holding the <see cref="PlayerAction"/>'s</param>
        public static void RegisterActions(Type type)
        {
            var mod = SRMod.GetCurrentMod();
            foreach (var field in type.GetFields().Where(x => x.IsStatic && typeof(PlayerAction).IsAssignableFrom(x.FieldType)))
            {
                PlayerAction action = new PlayerAction(field.Name, SRInput.Actions);
                RegisterAction(action, mod);
                RegisterBind(action);
            }
        }
    }
}

using InControl;
using SRML.API.Player;
using SRML.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
    [Obsolete]
    public static class BindingRegistry
    {
        /// <summary>
        /// Register a keybind into the options UI.
        /// </summary>
        /// <param name="action">The action associated with the keybind.</param>
        public static void RegisterBind(PlayerAction action) => PlayerActionRegistry.Instance.RegisterBindingLine(action);

        /// <summary>
        /// Creates and registers a <see cref="PlayerAction"/>.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <returns>The created action</returns>
        public static PlayerAction RegisterAction(string name)
        {
            PlayerAction act = new PlayerAction(name, SRInput.Actions);
            PlayerActionRegistry.Instance.Register(act);
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
            PlayerActionRegistry.Instance.RegisterBindingLine(act);
            return act;
        }

        /// <summary>
        /// Creates and registers a <see cref="PlayerAction"/>, adds it into the options UI, and adds a translation.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="translation">The translated name of the action.</param>
        /// <returns>The created action</returns>
        public static PlayerAction RegisterBindedTranslatedAction(string name, string translation)
        {
            PlayerAction act = RegisterBindedAction(name);
            TranslationPatcher.AddUITranslation("key." + name.ToLower(), translation);
            return act;
        }

        /// <summary>
        /// Check if an action is modded.
        /// </summary>
        /// <param name="action">The action to check.</param>
        /// <returns>True if action belongs to a mod, otherwise false.</returns>
        public static bool IsModdedAction(PlayerAction action) => PlayerActionRegistry.Instance.IsRegistered(action) || 
            KeyBindManager.ephemeralActions.Contains(action);

        /// <summary>
        /// Register all <see cref="PlayerAction"/>'s in a Type
        /// </summary>
        /// <param name="type">Type holding the <see cref="PlayerAction"/>'s</param>
        public static void RegisterActions(Type type) => PlayerActionRegistry.Instance.RegisterAllActions(type);
    }
}

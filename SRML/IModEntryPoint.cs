using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace SRML
{
    public interface IModEntryPoint
    {
        /// <summary>
        /// Called before <see cref="GameContext.Awake"/><br/>
        /// Used to register things such as enum values, and to do Harmony patching
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Called before <see cref="GameContext.Start"/><br/>
        /// Used to register things that require a loaded GameContext
        /// </summary>
        void Load();

        /// <summary>
        /// Called after every mod's Load has finished (not a registry step)<br/>
        /// Used for editing existing assets in the game
        /// </summary>
        void PostLoad();
    }

    public abstract class ModEntryPoint : IModEntryPoint
    {
        public Harmony HarmonyInstance => HarmonyPatcher.GetInstance();

        public Console.Console.ConsoleInstance ConsoleInstance { get; internal set; }

        public virtual void PreLoad()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void PostLoad()
        {
        }

        public virtual void Reload()
        {
        }

        public virtual void Unload()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }
    }
}
